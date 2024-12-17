using Business.Services;
using DAL.Databases;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();

        // Configure DbContext
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        // Add Event Management Repositories
        builder.Services.AddScoped<IEventRepository, EventRepository>();
        builder.Services.AddScoped<IEventTicketRepository, EventTicketRepository>();

        // Add services
        builder.Services.AddScoped<IUserBalanceService, UserBalanceService>();

        // Add Event Management Services
        builder.Services.AddScoped<IEventTicketingService, EventTicketingService>();

        // Swagger and other configurations
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Build the app
        var app = builder.Build();

        // Database seeding
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            SeedDatabase(services);
        }

        // Other app configurations
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();

        // Database seeding method
        void SeedDatabase(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()
            );

            // Ensure the database is created
            context.Database.EnsureCreated();
        }
    }
}
