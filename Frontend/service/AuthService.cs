using Microsoft.AspNetCore.Components;

namespace Frontend.service;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public AuthService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    internal async Task HandleLogout()
    {
        try
        {
            // Call logout endpoint
            await _httpClient.PostAsync("https://localhost:7066/accounts/logout", null);

            // Navigate to the main page
            _navigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout failed: {ex.Message}");
        }
    }
}