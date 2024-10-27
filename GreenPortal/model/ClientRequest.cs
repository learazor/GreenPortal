namespace GreenPortal.model;

public record ClientRequest(
    string Type,
    double? Output = null,
    string? Country = null,
    string? PostalCode = null,
    double? MaxPrice = null,
    int? MaxTime = null
);