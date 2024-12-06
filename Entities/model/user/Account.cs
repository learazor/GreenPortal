namespace Entities.model.user;

public abstract class Account
{
    public string Email { get; set; } = string.Empty; // Primary Key
    public string Password { get; set; } = string.Empty; // Hashed password
}