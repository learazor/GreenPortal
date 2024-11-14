using GreenPortal.repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddDbContext<GreenPortalContext>(options =>
    options.UseNpgsql("Host=abstractly-awake-ouzel.data-1.euc1.tembo.io;Database=postgres;Username=postgres;Password=AQm5ISoHY3fjnw67;Port=5432"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CompanyRepository>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GreenPortalContext>();
    context.Database.EnsureCreated();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
