using AccommodationService.Data;
using AccommodationService.Domain.Entities;
using AccommodationService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AccommodationService.IntegrationTests.Helpers;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    public Guid TestUserId { get; set; } = Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove real DB
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(descriptor);

            // Add in-memory DB
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDB"));

            // Replace CurrentUserService
            services.RemoveAll<ICurrentUserService>();
            services.AddSingleton<ICurrentUserService>(new TestCurrentUserService
            {
                UserId = TestUserId,
                IsAuthenticated = true,
                Role = "Host"
            });

            // Add Test Authentication
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            services.AddAuthorization(options => { options.AddPolicy("Host", policy => policy.RequireRole("Host")); });

            // Build provider and seed DB
            using var sp = services.BuildServiceProvider().CreateScope();
            var db = sp.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            SeedAmenities(db);
            db.SaveChanges();
        });
    }

    private void SeedAmenities(ApplicationDbContext db)
    {
        for (int i = 1; i <= 10; i++)
        {
            db.Set<Amenity>().Add(new Amenity($"Amenity {i}", $"Description {i}"));
        }
    }
}