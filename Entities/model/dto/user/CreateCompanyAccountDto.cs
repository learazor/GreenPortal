namespace Entities.model.dto.user;

public class CreateCompanyAccountDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string CompanyName { get; set; }
    public string ContactPerson { get; set; }
    public string Address { get; set; }
    public string CompanyCode { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public double PricePerDistanceUnit { get; set; }
}