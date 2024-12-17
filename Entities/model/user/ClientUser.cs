namespace Entities.model.user;

public class ClientUser : User
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}