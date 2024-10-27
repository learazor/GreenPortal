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

        CompanyInstallation? companyInstallation = installations.Find(i => i.Type == request.Type);
        // Simulate an offer calculation based on the retrieved data
        

        if (companyInstallation != null)
        {
            var offer = new InstallationOffer(companyInstallation.CompanyCode, CalculatePrice(companyInstallation.PricePerUnit),
                companyInstallation.SettingUpTimePerUnit, companyInstallation.Output);

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
        // Define the request parameters
        string country1 = "USA";
        string postalCode1 = "10001";
        string country2 = "Canada";
        string postalCode2 = "H2X 1X4";

        // Create the HttpClient instance
        using var httpClient = new HttpClient();

        // Build the request URI with query parameters
        var requestUri = $"http://localhost:5000/api/distance?country1={country1}&postalCode1={postalCode1}&country2={country2}&postalCode2={postalCode2}";

        // Send the POST request
        var response = await httpClient.PostAsync(requestUri, null);

        // Output the response
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Request successful:");
            Console.WriteLine(responseBody);
            return double.Parse(responseBody);
        }
        throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
    }
}