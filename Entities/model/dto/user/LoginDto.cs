namespace Entities.model.dto.user;

public record LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}