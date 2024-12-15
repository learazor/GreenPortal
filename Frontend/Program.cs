using Frontend.Components;
using Frontend.service;
using Frontend.service.navigation.admin;
using Frontend.service.navigation.client;
using Frontend.service.navigation.company;
using Frontend.service.navigation.login;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddHttpClient();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ClientNavi>();
builder.Services.AddScoped<CompanyNavi>();
builder.Services.AddScoped<AdminNavi>();
builder.Services.AddScoped<LoginNavi>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();