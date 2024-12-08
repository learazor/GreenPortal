namespace Entities.model.user;

public class Company : Account
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CompanyCode { get; set; } = string.Empty; // Reference to CompanyInfo
}