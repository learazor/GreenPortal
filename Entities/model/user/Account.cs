namespace Entities.model.user;

using Microsoft.AspNetCore.Identity;


public abstract class Account : IdentityUser
{
    public bool IsActive { get; set; } = true;
}