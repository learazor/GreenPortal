namespace GreenPortal.model;

public record InstallationOffer(
    string CompanyCode,
    double InstallationCost,
    double TransportationCost,
    double TotalCost,
    int Time,
    double Output
);