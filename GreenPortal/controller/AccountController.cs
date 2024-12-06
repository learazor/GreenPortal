using Entities.model.dto.user;
using Entities.model.user;
using GreenPortal.repository;

namespace GreenPortal.controllers;

using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly GreenPortalContext _context;

    public AccountController(GreenPortalContext context)
    {
        _context = context;
    }

    // Create a Client Account
    [HttpPost("clients")]
    public async Task<IActionResult> CreateClientAccount(CreateClientAccountDto dto)
    {
        if (_context.Accounts.Any(a => a.Email == dto.Email))
        {
            return BadRequest("An account with this email already exists.");
        }

        var hashedPassword = BCrypt.HashPassword(dto.Password);

        var client = new Client
        {
            Email = dto.Email,
            Password = hashedPassword,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return Ok("Client account created successfully.");
    }

    // Create a Company Account
    [HttpPost("companies")]
    public async Task<IActionResult> CreateCompanyAccount(CreateCompanyAccountDto dto)
    {
        if (_context.Accounts.Any(a => a.Email == dto.Email))
        {
            return BadRequest("An account with this email already exists.");
        }

        var hashedPassword = BCrypt.HashPassword(dto.Password);

        var company = new Company
        {
            Email = dto.Email,
            Password = hashedPassword,
            CompanyName = dto.CompanyName,
            ContactPerson = dto.ContactPerson,
            Address = dto.Address,
            CompanyCode = dto.CompanyCode
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return Ok("Company account created successfully.");
    }
}
