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
            .FirstAsync();

        accommodation.Name.Should().Be("Test House");
    }
}