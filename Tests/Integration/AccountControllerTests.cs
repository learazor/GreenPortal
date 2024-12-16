using FluentAssertions;

namespace Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using Entities.model.dto.user;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class AccountControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder => { }).CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_ForValidCredentials()
    {
        var loginDto = new LoginDto
        {
            Email = "testuser@example.com",
            Password = "securepassword"
        };
        
        var response = await _client.PostAsJsonAsync("/accounts/login", loginDto);

        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("Login successful");
    }

    [Fact]
    public async Task CreateClientAccount_ShouldReturnOk_ForValidDetails()
    {
        var createClientDto = new CreateClientAccountDto
        {
            Email = "newclient@example.com",
            Password = "clientpassword123",
            PhoneNumber = "123456789"
        };
        
        var response = await _client.PostAsJsonAsync("/accounts/clients", createClientDto);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("Client account created successfully");
    }

    [Fact]
    public async Task Logout_ShouldReturnOk()
    {
        var response = await _client.PostAsync("/accounts/logout", null);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("Logout successful");
    }
}