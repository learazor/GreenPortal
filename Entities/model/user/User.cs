namespace Entities.model.user;

using Microsoft.AspNetCore.Identity;


public abstract class User : IdentityUser
{
    public bool IsActive { get; set; } = true;
    public string AccountType { get; set; }
}