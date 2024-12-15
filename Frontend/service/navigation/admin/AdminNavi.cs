using Microsoft.AspNetCore.Components;

namespace Frontend.service.navigation.admin;

public class AdminNavi
{
    private readonly NavigationManager _navigationManager;

    public AdminNavi(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }


    public void GoToCreateCompany()
    {
        _navigationManager.NavigateTo("/admin-create-company");
    }

    public void GoToDashboard()
    {
        _navigationManager.NavigateTo("/admin-dashboard");
    }
}