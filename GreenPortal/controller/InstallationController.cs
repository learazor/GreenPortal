using EfcRepositories;
using Entities.model.portal;
using Entities.model.user;
using GreenPortal.model;
using GreenPortal.repository;
using GreenPortal.session;
using GreenPortal.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GreenPortal.controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("installations")]
public class InstallationController : ControllerBase
{
    private readonly CompanyRepository _repository;
    private readonly UserManager<Account> _userManager;


    public InstallationController(CompanyRepository repository, UserManager<Account> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    [HttpPost("offer")]
    public async Task<ActionResult<InstallationOrder>> PrepareInstallationOffers([FromBody] ClientRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized("You must be logged in to access this endpoint.");
        }

        if (user.AccountType != "Client")
        {
            return Forbid("This endpoint is restricted to Client users only.");
        }


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
            List<InstallationOrder> installationOffers = new List<InstallationOrder>();
            foreach (var installation in companyInstallations)
            {
                var companyInfo = await _repository.GetCompanyInfo(installation.company_code);

                var maxUnitsInPriceRange = request.MaxPrice / installation.price_per_unit;
                var maxUnitsInTimeRange = request.MaxTime / installation.setting_up_time_per_unit;
                var unitsNeededForRequestedOutput = request.MinOutput / installation.output;

                double? smallestUnitNumber =
                    new[] { maxUnitsInPriceRange, maxUnitsInTimeRange, unitsNeededForRequestedOutput }.Min();
                var noOfUnits =
                    (int)Math.Floor((double)smallestUnitNumber); //For now picking just the smallest possible limit 

                var installationCost = noOfUnits * installation.price_per_unit;
                var transportCost = CalculateTransportCost(request.PostalCode,
                    request.Country, companyInfo.postal_code, companyInfo.country, companyInfo.price_per_distance_unit,
                    noOfUnits);
                var totalCost = transportCost + installationCost;

                var offer = new InstallationOrder.Builder()
                    .SetCompanyCode(installation.company_code)
                    .SetInstallationCost(installationCost)
                    .SetTransportationCost(transportCost)
                    .SetTotalCost(totalCost)
                    .SetTime(installation.setting_up_time_per_unit)
                    .SetOutput(installation.output)
                    .SetClientEmail(user.Email)
                    .SetStatus(OrderStatus.OFFERED)
                    .Build();

                installationOffers.Add(offer);
            }

            HttpContext.Session.SetObjectAsJson("Offers", installationOffers);
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

    private double CalculateTransportCost(string clientPostalCode, string clientCountry,
        string companyPostalCode, string companyCountry, double pricePerDistanceUnit, double noOfUnits)
    {
        var distance = DistanceCalc
            .CalculateDistance(clientPostalCode, clientCountry, companyPostalCode, companyCountry).Result;
        return noOfUnits * pricePerDistanceUnit * distance;
    }
}