using AccommodationService.Services.Interfaces;

namespace AccommodationService.IntegrationTests.Helpers;

public class TestCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
    public string? Role { get; set; } = "Host";
    public bool IsAuthenticated { get; set; } = true;
}