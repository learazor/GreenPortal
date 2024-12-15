using Microsoft.AspNetCore.Components;

namespace Frontend.service.navigation.company;

public class CompanyNavi
{
    private readonly NavigationManager _navigationManager;

    public CompanyNavi(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    internal void GoToOrders()
    {
        // Logic to navigate to Orders page
        _navigationManager.NavigateTo("/company-orders");
    }
}