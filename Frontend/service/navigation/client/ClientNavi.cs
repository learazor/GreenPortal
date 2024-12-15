using Microsoft.AspNetCore.Components;

namespace Frontend.service.navigation.client;

public class ClientNavi
{
    private readonly NavigationManager _navigationManager;

    public ClientNavi(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    internal void GoToInstallations()
    {
        _navigationManager.NavigateTo("/client-installations");
    }

    internal void GoToOrders()
    {
        _navigationManager.NavigateTo("/client-orders");
    }

    internal void GoToDashboard()
    {
        _navigationManager.NavigateTo("/client-dashboard");
    }
    
}