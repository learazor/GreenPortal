using System.Security.Claims;
using EfcRepositories;
using Entities.model.dto.user;
using Entities.model.user;
using GreenPortal.model;
using GreenPortal.repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GreenPortal.controllers;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly GreenPortalContext _context;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, GreenPortalContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (user == null || !passwordCheck)
        {
            return Unauthorized("Invalid email or password.");
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("AccountType", user.AccountType) // Add AccountType claim
        };

        var identity = new ClaimsIdentity(claims, "ApplicationCookie");
        var principal = new ClaimsPrincipal(identity);

        await _signInManager.SignInAsync(user, isPersistent: false);

        return Ok("Login successful. Logged as: "+user.AccountType);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync("Cookies");
        await _signInManager.SignOutAsync();
        return Ok("Logout successful.");
    }

    [HttpPost("clients")]
    public async Task<IActionResult> CreateClientAccount(CreateClientAccountDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
        {
            return BadRequest("An account with this email already exists.");
        }
        
        var client = new ClientUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
        };
        
        var result = await _userManager.CreateAsync(client, dto.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("Client account created successfully.");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("companies")]
    public async Task<IActionResult> CreateCompanyAccount(CreateCompanyAccountDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return BadRequest("An account with this email already exists.");
            }
            
            var existingCompanyInfo = await _context.companyinfo.FindAsync(dto.CompanyCode);
            if (existingCompanyInfo != null)
            {
                return BadRequest($"CompanyCode '{dto.CompanyCode}' already exists in companyinfo.");
            }

            //entry creation in companyinfo
            var companyInfo = new CompanyInfo
            {
                company_code = dto.CompanyCode,
                name = dto.CompanyName,
                country = dto.Country,
                postal_code = dto.PostalCode,
                price_per_distance_unit = dto.PricePerDistanceUnit
            };
            await _context.companyinfo.AddAsync(companyInfo);
            await _context.SaveChangesAsync();

            //account creation in AspNetUsers
            var company = new CompanyUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                CompanyName = dto.CompanyName,
                ContactPerson = dto.ContactPerson,
                Address = dto.Address,
                CompanyCode = dto.CompanyCode,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(company, dto.Password);
            if (!result.Succeeded)
            {
                //in case of failure rollback
                await transaction.RollbackAsync();
                return BadRequest(result.Errors);
            }

            //transaction commit after both operations succeed
            await transaction.CommitAsync();

            return Ok("Company account created successfully.");
        }
        catch (Exception ex)
        {
            //transaction rollback in case of exceptions
            await transaction.RollbackAsync();
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
