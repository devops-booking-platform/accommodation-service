using AccommodationService.Common.Events;
using AccommodationService.Data;
using AccommodationService.Domain.Entities;
using AccommodationService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using StackExchange.Redis;

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
            var hosted = services.SingleOrDefault(d => d.ImplementationType == typeof(IntegrationEventsSubscriber));
            if (hosted != null)
                services.Remove(hosted);
            // Remove real DB
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(descriptor);
            services.RemoveAll<IEventBus>();
            services.AddSingleton<IEventBus, NoOpEventBus>();

            // Remove real Redis and add mock
            services.RemoveAll<IConnectionMultiplexer>();
            var mockRedisDb = new Mock<IDatabase>();
            mockRedisDb.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(false);
            mockRedisDb.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), 
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            var mockRedis = new Mock<IConnectionMultiplexer>();
            mockRedis.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(mockRedisDb.Object);
            
            services.AddSingleton<IConnectionMultiplexer>(mockRedis.Object);

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