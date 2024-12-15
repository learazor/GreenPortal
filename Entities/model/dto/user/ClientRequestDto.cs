namespace Entities.model.dto.user;

public class ClientRequestDto
{
    public string Type { get; set; } = string.Empty;
    public double? MinOutput { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public double? MaxPrice { get; set; }
    public int? MaxTime { get; set; }
}