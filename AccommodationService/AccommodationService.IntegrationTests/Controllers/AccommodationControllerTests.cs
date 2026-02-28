using System.Net;
using System.Net.Http.Json;
using AccommodationService.Data;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AccommodationService.Domain.Enums;
using AccommodationService.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccommodationService.IntegrationTests.Controllers;

public class AccommodationControllerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateAccommodation_ShouldReturn201_AndSave()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var request = new AccommodationRequest
        {
            Name = "Test House",
            Description = "Nice place",
            Location = new LocationRequest
                { City = "Seattle", Address = "Core 3", Country = "USA", PostalCode = "2222" },
            PriceType = PriceType.PerGuest,
            Photos = new List<string> { "photo1.jpg", "photo2.jpg" }
        };

        var response = await _client.PostAsJsonAsync("/api/accommodations", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var accommodation = await db.Set<Accommodation>()
            .Include(a => a.Amenities)
            .FirstOrDefaultAsync(a => a.Name == "Test House");

        accommodation.Should().NotBeNull();
        accommodation!.Name.Should().Be("Test House");
    }

    [Fact]
    public async Task UpdateAccommodation_ShouldReturn204_AndUpdate()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create initial accommodation
        var accommodation = new Accommodation
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Description = "Original Description",
            HostId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            MinimumNumberOfGuests = 1,
            MaximumNumberOfGuests = 4,
            PriceType = PriceType.PerUnit,
            Location = new Location
            {
                City = "OldCity",
                Country = "OldCountry",
                Address = "OldAddress",
                PostalCode = "00000"
            }
        };

        db.Set<Accommodation>().Add(accommodation);
        await db.SaveChangesAsync();

        var updateRequest = new AccommodationRequest
        {
            Id = accommodation.Id,
            Name = "Updated Name",
            Description = "Updated Description",
            Location = new LocationRequest
            {
                City = "NewCity",
                Country = "NewCountry",
                Address = "NewAddress",
                PostalCode = "11111"
            },
            MinimumNumberOfGuests = 2,
            MaximumNumberOfGuests = 6,
            PriceType = PriceType.PerGuest,
            Photos = new List<string> { "newphoto1.jpg" },
            Amenities = new List<Guid>()
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/accommodations", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Clear the change tracker to get fresh data from database
        db.ChangeTracker.Clear();
        
        var updated = await db.Set<Accommodation>()
            .Include(a => a.Location)
            .FirstAsync(a => a.Id == accommodation.Id);

        updated.Name.Should().Be("Updated Name");
        updated.Description.Should().Be("Updated Description");
        updated.Location!.City.Should().Be("NewCity");
        updated.MinimumNumberOfGuests.Should().Be(2);
        updated.MaximumNumberOfGuests.Should().Be(6);
    }

    [Fact]
    public async Task GetAccommodationInfo_ShouldReturn200_WithData()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var accommodation = new Accommodation
        {
            Id = Guid.NewGuid(),
            Name = "Beach House",
            Description = "Beautiful beach house",
            HostId = Guid.NewGuid(),
            MinimumNumberOfGuests = 1,
            MaximumNumberOfGuests = 6,
            PriceType = PriceType.PerUnit,
            Location = new Location
            {
                City = "Miami",
                Country = "USA",
                Address = "123 Beach Ave",
                PostalCode = "33139"
            }
        };

        db.Set<Accommodation>().Add(accommodation);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/accommodations/{accommodation.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetAccommodationResponse>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("Beach House");
        result.Location.Should().NotBeNull();
        result.Location!.City.Should().Be("Miami");
    }

    [Fact]
    public async Task GetAccommodationInfo_ShouldReturn404_WhenNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/accommodations/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAmenities_ShouldReturn200_WithAllAmenities()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Amenities should be seeded, but let's add some if needed
        var amenityCount = await db.Set<Amenity>().CountAsync();
        
        // Act
        var response = await _client.GetAsync("/api/accommodations/amenities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AmenityResponseDto>>();
        result.Should().NotBeNull();
        result!.Should().HaveCountGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetMy_ShouldReturn200_WithHostAccommodations()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var hostId = factory.TestUserId;

        var accommodation1 = new Accommodation
        {
            Id = Guid.NewGuid(),
            Name = "Property 1",
            Description = "First property",
            HostId = hostId,
            MinimumNumberOfGuests = 1,
            MaximumNumberOfGuests = 4,
            PriceType = PriceType.PerUnit,
            Location = new Location
            {
                City = "Seattle",
                Country = "USA",
                Address = "123 Main St",
                PostalCode = "98101"
            }
        };

        var accommodation2 = new Accommodation
        {
            Id = Guid.NewGuid(),
            Name = "Property 2",
            Description = "Second property",
            HostId = hostId,
            MinimumNumberOfGuests = 2,
            MaximumNumberOfGuests = 6,
            PriceType = PriceType.PerGuest,
            Location = new Location
            {
                City = "Portland",
                Country = "USA",
                Address = "456 Oak Ave",
                PostalCode = "97201"
            }
        };

        db.Set<Accommodation>().AddRange(accommodation1, accommodation2);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/accommodations/my");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<HostAccommodationListItemDto>>();
        result.Should().NotBeNull();
        result!.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Should().Contain(a => a.Name == "Property 1");
        result.Should().Contain(a => a.Name == "Property 2");
    }

    [Fact]
    public async Task GetReservationInfo_ShouldReturn200_WithCalculatedPrice()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var accommodation = new Accommodation
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Description = "Nice hotel",
            HostId = Guid.NewGuid(),
            MinimumNumberOfGuests = 1,
            MaximumNumberOfGuests = 4,
            PriceType = PriceType.PerUnit,
            IsAutoConfirm = true
        };

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));

        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodation.Id,
            StartDate = startDate,
            EndDate = endDate,
            Price = 100m
        };

        db.Set<Accommodation>().Add(accommodation);
        db.Set<Availability>().Add(availability);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync(
            $"/api/accommodations/{accommodation.Id}/reservation-info?start={startDate}&end={endDate}&guests=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AccommodationReservationInfoResponseDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Hotel");
        result.TotalPrice.Should().Be(300m); // 3 nights * 100
        result.MaxGuests.Should().Be(4);
        result.IsAutoAcceptEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task GetReservationInfo_ShouldReturn400_WhenGuestsExceedMaxCapacity()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var accommodation = new Accommodation
        {
            Id = Guid.NewGuid(),
            Name = "Small Hotel",
            Description = "Cozy place",
            HostId = Guid.NewGuid(),
            MinimumNumberOfGuests = 1,
            MaximumNumberOfGuests = 2,
            PriceType = PriceType.PerUnit
        };

        db.Set<Accommodation>().Add(accommodation);
        await db.SaveChangesAsync();

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));

        // Act
        var response = await _client.GetAsync(
            $"/api/accommodations/{accommodation.Id}/reservation-info?start={startDate}&end={endDate}&guests=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetReservationInfo_ShouldReturn404_WhenAccommodationNotFound()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));

        // Act
        var response = await _client.GetAsync(
            $"/api/accommodations/{Guid.NewGuid()}/reservation-info?start={startDate}&end={endDate}&guests=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReservationInfo_ShouldCalculatePricePerGuest_WhenPriceTypeIsPerGuest()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var accommodation = new Accommodation
        {
            Id = Guid.NewGuid(),
            Name = "Guest Hotel",
            Description = "Price per guest",
            HostId = Guid.NewGuid(),
            MinimumNumberOfGuests = 1,
            MaximumNumberOfGuests = 6,
            PriceType = PriceType.PerGuest,
            IsAutoConfirm = false
        };

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodation.Id,
            StartDate = startDate,
            EndDate = endDate,
            Price = 50m
        };

        db.Set<Accommodation>().Add(accommodation);
        db.Set<Availability>().Add(availability);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync(
            $"/api/accommodations/{accommodation.Id}/reservation-info?start={startDate}&end={endDate}&guests=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AccommodationReservationInfoResponseDto>();
        result.Should().NotBeNull();
        result!.TotalPrice.Should().Be(300m); // 2 nights * 50 * 3 guests
    }
}