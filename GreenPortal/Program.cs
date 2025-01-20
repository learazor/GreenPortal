using EfcRepositories;
using Entities.model.user;
using GreenPortal.repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

//db connection config
        builder.Services.AddDbContext<GreenPortalContext>(options =>
            options.UseNpgsql(
                "Host=abstractly-awake-ouzel.data-1.euc1.tembo.io;Database=postgres;Username=postgres;Password=AQm5ISoHY3fjnw67;Port=5432;SSL Mode=VerifyFull;RootCertificate=cert/root.crt"));
        
//identity services
        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<GreenPortalContext>()
            .AddDefaultTokenProviders();

//identity options
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        });

//cookie-based authentication
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/accounts/login";
            options.AccessDeniedPath = "/accounts/accessdenied";
        });

        builder.Services.AddAuthentication("Cookies")
            .AddCookie("Cookies", options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<CompanyRepository>();
        builder.Services.AddScoped<OrderRepository>();

//session services
        builder.Services.AddDistributedMemoryCache(); //in-memory storage for sessions
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ClientOnly", policy =>
                policy.RequireClaim("AccountType", "Client"));
        });

        var app = builder.Build();

//data seed, needed anymore??
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseSession();

        app.Run();
    }
}