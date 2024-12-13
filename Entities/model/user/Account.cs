namespace Entities.model.user;

using Microsoft.AspNetCore.Identity;


public abstract class Account : IdentityUser
{
    public bool IsActive { get; set; } = true;
    public string AccountType { get; set; }
    public string? CompanyCode { get; set; }
}