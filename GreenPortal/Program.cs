using Entities.model.user;
using GreenPortal.repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddDbContext<GreenPortalContext>(options =>
    options.UseNpgsql("Host=abstractly-awake-ouzel.data-1.euc1.tembo.io;Database=postgres;Username=postgres;Password=AQm5ISoHY3fjnw67;Port=5432"));

// Add Identity services
builder.Services.AddIdentity<Account, IdentityRole>()
    .AddEntityFrameworkStores<GreenPortalContext>()
    .AddDefaultTokenProviders();

// Configure Identity options (Password settings, etc.)
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

// Configure cookie-based authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/accounts/login"; // Redirect to this path if not authenticated
    options.AccessDeniedPath = "/accounts/accessdenied"; // Redirect if access is denied
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CompanyRepository>();

var app = builder.Build();
// Seed initial data (e.g., admin user)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<GreenPortalContext>();
        context.Database.EnsureCreated();
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        // Log errors or handle any startup exceptions
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
