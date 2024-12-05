using System.Globalization;
using System.Net.Sockets;
using System.Text;
using GreenPortal.model;
using GreenPortal.repository;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace GreenPortal.controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("portal")]
public class InstallationController : ControllerBase
{
    
    private readonly CompanyRepository _repository;

    public InstallationController(CompanyRepository repository)
    {
        _repository = repository;
    }
    
    [HttpPost("offer")]
    public async Task<ActionResult<InstallationOffer>> PrepareInstallationOffers([FromBody] ClientRequest request)
    {
        var installations = await _repository.GetInstallationsByType(request.Type);

        if (installations == null || installations.Count == 0)
        {
            return NotFound("No installations found for the specified type.");
        }

        List<CompanyInstallation> companyInstallations = installations
            .Where(i => i.type == request.Type)
            .Where(i => i.price_per_unit <= request.MaxPrice) //MaxPrice without transportation cost
            .Where(i => i.setting_up_time_per_unit <= request.MaxTime)
            .ToList();
        
        

        if (!companyInstallations.Count.Equals(0))
        {
            List<InstallationOffer> installationOffers = new List<InstallationOffer>();
            foreach (var installation in companyInstallations)
            {
                var companyInfo = await _repository.GetCompanyInfo(installation.company_code);

                var maxUnitsInPriceRange = request.MaxPrice/installation.price_per_unit;
                var maxUnitsInTimeRange = request.MaxTime/installation.setting_up_time_per_unit;
                var unitsNeededForRequestedOutput = request.MinOutput/installation.output;

                double? smallestUnitNumber =
                    new[] { maxUnitsInPriceRange, maxUnitsInTimeRange, unitsNeededForRequestedOutput }.Min();
                
                var noOfUnits = (int)Math.Floor((double)smallestUnitNumber); //For now picking just the smallest possible limit 
                
                var calculatedPrice = CalculatePrice(installation.price_per_unit, request.PostalCode,
                    request.Country, companyInfo.postal_code, companyInfo.country, companyInfo.price_per_distance_unit,
                    noOfUnits);
                var offer = new InstallationOffer(installation.company_code, calculatedPrice,
                    installation.setting_up_time_per_unit, installation.output);
                
                installationOffers.Add(offer);
            }
            
            return Ok(installationOffers);
        }
        return NotFound("No installations found for the specified type.");
    }
    
    [HttpGet("companyinfo")]
    public async Task<ActionResult<List<CompanyInfo>>> GetAllCompanyInfo()
    {
        var companyInfoList = await _repository.GetAllCompanyInfo();
        return Ok(companyInfoList);
    }

    private double CalculatePrice(double pricePerUnit, string clientPostalCode, string clientCountry,
        string companyPostalCode, string companyCountry, double pricePerDistanceUnit, double noOfUnits)
    {
        var distance = CalculateDistance(clientPostalCode, clientCountry, companyPostalCode, companyCountry).Result;
        return pricePerUnit + noOfUnits * pricePerDistanceUnit * distance;
    }

    private async Task<double> CalculateDistance(string clientPostalCode, string clientCountry, string companyPostalCode, string companyCountry)
    {
                using (var client = new TcpClient())
                {
                    Console.WriteLine("Connecting to server...");
                    await client.ConnectAsync("127.0.0.1", 5001);
                    Console.WriteLine("Connected to server.");
        
                    using (var networkStream = client.GetStream())
                    {
                        //format: "Country1|PostalCode1|Country2|PostalCode2"
                        string message = $"{clientCountry}|{clientPostalCode}|{companyCountry}|{companyPostalCode}";
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        
                        await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                        Console.WriteLine("Message sent.");
                        
                        byte[] buffer = new byte[1024];
                        int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        
                        Console.WriteLine("Response from server:");
                        Console.WriteLine(response);
                        return Double.Parse(response.Substring(0, 7));
                    }
                }
    }
}