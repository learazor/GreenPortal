using System.Net.Sockets;
using System.Text;
using GreenPortal.model;
using GreenPortal.repository;

namespace GreenPortal.controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/portal")]
public class InstallationController : ControllerBase
{
    
    private readonly CompanyRepository _repository;

    public InstallationController(CompanyRepository repository)
    {
        _repository = repository;
    }
    
    [HttpPost("offer")]
    public async Task<ActionResult<InstallationOffer>> GetInstallationOffer([FromBody] ClientRequest request)
    {
        
        var installations = await _repository.GetInstallationsByType(request.Type);

        if (installations == null || installations.Count == 0)
        {
            return NotFound("No installations found for the specified type.");
        }

        CompanyInstallation? companyInstallation = installations.Find(i => i.type == request.Type); //Return only one e.g. try to use Single here
        // Simulate an offer calculation based on the retrieved data
        

        if (companyInstallation != null)
        {
            //CalculatePrice(companyInstallation.PricePerUnit)
            var offer = new InstallationOffer(companyInstallation.company_code, CalculatePrice(companyInstallation.price_per_unit),
                companyInstallation.setting_up_time_per_unit, companyInstallation.output);

            return Ok(offer);
        }
        return NotFound("No installations found for the specified type.");
    }
    
    [HttpGet("companyinfo")]
    public async Task<ActionResult<List<CompanyInfo>>> GetAllCompanyInfo()
    {
        var companyInfoList = await _repository.GetAllCompanyInfo();
        return Ok(companyInfoList);
    }

    private double CalculatePrice(double pricePerUnit)
    {
        double distance = CalculateDistance().Result;
        return pricePerUnit + pricePerUnit * 12 * distance;
    }

    private async Task<double> CalculateDistance()
    {
                using (var client = new TcpClient())
                {
                    Console.WriteLine("Connecting to server...");
                    await client.ConnectAsync("127.0.0.1", 5001);
                    Console.WriteLine("Connected to server.");
        
                    using (var networkStream = client.GetStream())
                    {
                        //format: "Country1|PostalCode1|Country2|PostalCode2"
                        string message = "Netherlands|3067|Denmark|8700";
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        
                        await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                        Console.WriteLine("Message sent.");
                        
                        byte[] buffer = new byte[1024];
                        int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        
                        Console.WriteLine("Response from server:");
                        Console.WriteLine(response);
                        return Double.Parse(response);
                    }
                }
    }
}