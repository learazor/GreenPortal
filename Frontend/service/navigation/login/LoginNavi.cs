using Microsoft.AspNetCore.Components;

namespace Frontend.service.navigation.login;

public class LoginNavi
{
    private readonly NavigationManager _navigationManager;

    public LoginNavi(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    internal void GoToCreateAccount()
    {
        _navigationManager.NavigateTo("/create-client");
    }

    internal void GoToLogin()
    {
        _navigationManager.NavigateTo("/");
    }
}