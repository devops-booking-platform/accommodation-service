using System.Net;
using System.Net.Http.Json;
using AccommodationService.Data;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AccommodationService.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccommodationService.IntegrationTests.Controllers;

public class AvailabilityControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly Guid _testUserId;

    public AvailabilityControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _testUserId = factory.TestUserId;

        // Ensure DB seeded for tests
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!db.Set<Accommodation>().Any(a => a.HostId == _testUserId))
        {
            db.Set<Accommodation>().Add(new Accommodation
            {
                Id = Guid.NewGuid(),
                Name = "Test House",
                Description = "Nice place",
                HostId = _testUserId
            });
            db.SaveChanges();
        }
    }

    private Guid GetTestAccommodationId()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return db.Set<Accommodation>().First(a => a.HostId == _testUserId).Id;
    }

    private static readonly DateTimeOffset FixedNow = new(2025, 11, 30, 18, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateOrUpdate_ShouldReturn200_WhenValidRequest()
    {
        // Arrange
        var request = new AvailabilityRequest
        {
            AccommodationId = GetTestAccommodationId(),
            StartDate = FixedNow,
            EndDate = FixedNow.AddDays(5)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/availability", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var availability = await db.Set<Availability>()
            .FirstOrDefaultAsync(a => a.AccommodationId == request.AccommodationId &&
                                      a.StartDate == request.StartDate);
        availability.Should().NotBeNull();
        availability.EndDate.Should().Be(request.EndDate);
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldReturn400_WhenOverlapping()
    {
        // Arrange
        var accommodationId = GetTestAccommodationId();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Set<Availability>().Add(new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodationId,
            StartDate = FixedNow,
            EndDate = FixedNow.AddDays(5)
        });
        await db.SaveChangesAsync();

        var request = new AvailabilityRequest
        {
            AccommodationId = accommodationId,
            StartDate = FixedNow.AddDays(3),
            EndDate = FixedNow.AddDays(6),
            Price = 20
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/availability", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldReturn401_WhenNotHost()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var request = new AvailabilityRequest
        {
            AccommodationId = GetTestAccommodationId(),
            StartDate = FixedNow,
            EndDate = FixedNow.AddDays(5)
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/availability", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldUpdate_WhenIdProvided()
    {
        // Arrange
        var accommodationId = GetTestAccommodationId();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodationId,
            Price = 20,
            StartDate = FixedNow,
            EndDate = FixedNow.AddDays(5)
        };
        db.Set<Availability>().Add(availability);
        await db.SaveChangesAsync();

        var request = new AvailabilityRequest
        {
            Id = availability.Id,
            AccommodationId = accommodationId,
            StartDate = FixedNow.AddDays(15),
            EndDate = FixedNow.AddDays(17),
            Price = 30
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/availability", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await db.Entry(availability).ReloadAsync();

        // Fetch fresh entity to ensure changes persisted
        var updated = await db.Set<Availability>().FindAsync(availability.Id);
        updated!.StartDate.Should().BeCloseTo(request.StartDate, TimeSpan.FromSeconds(1));
        updated.EndDate.Should().BeCloseTo(request.EndDate, TimeSpan.FromSeconds(1));
    }
}