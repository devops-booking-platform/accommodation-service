using AccommodationService.Common.Events;
using AccommodationService.Common.Exceptions;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AccommodationService.Repositories.Interfaces;
using AccommodationService.Services;
using AccommodationService.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace AccommodationService.UnitTests.Services;

public class AvailabilityServiceTests
{
    private readonly Mock<IRepository<Availability>> _availabilityRepoMock;
    private readonly Mock<IRepository<Accommodation>> _accommodationRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEventBus> _eventBusMock;

    private readonly AvailabilityService _service;

    public AvailabilityServiceTests()
    {
        _availabilityRepoMock = new Mock<IRepository<Availability>>();
        _accommodationRepoMock = new Mock<IRepository<Accommodation>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _eventBusMock = new Mock<IEventBus>();

        _service = new AvailabilityService(
            _availabilityRepoMock.Object,
            _accommodationRepoMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object,
            _eventBusMock.Object
        );
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldThrow_WhenUserNotAuthenticated()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);
        var request = new AvailabilityRequest();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateOrUpdate(request));
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldThrow_WhenAccommodationNotFound()
    {
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _accommodationRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Accommodation?)null);

        var request = new AvailabilityRequest { AccommodationId = Guid.NewGuid() };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateOrUpdate(request));
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldThrow_WhenAccommodationNotOwnedByUser()
    {
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var accommodation = new Accommodation { Id = Guid.NewGuid(), HostId = Guid.NewGuid() }; // different host
        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodation.Id)).ReturnsAsync(accommodation);

        var request = new AvailabilityRequest { AccommodationId = accommodation.Id };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateOrUpdate(request));
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldThrow_WhenOverlapping()
    {
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var accommodation = new Accommodation { Id = Guid.NewGuid(), HostId = userId };
        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodation.Id)).ReturnsAsync(accommodation);

        var existing = new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodation.Id,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(5)
        };

        var availabilities = new List<Availability> { existing }.AsAsyncQueryable();
        _availabilityRepoMock.Setup(x => x.Query()).Returns(availabilities);

        var request = new AvailabilityRequest
        {
            AccommodationId = accommodation.Id,
            StartDate = DateTime.UtcNow.AddDays(3), // overlaps
            EndDate = DateTime.UtcNow.AddDays(6)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrUpdate(request));
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldAddNewAvailability_WhenValid()
    {
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var accommodation = new Accommodation { Id = Guid.NewGuid(), HostId = userId };
        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodation.Id)).ReturnsAsync(accommodation);

        _availabilityRepoMock.Setup(x => x.Query()).Returns(new List<Availability>().AsAsyncQueryable());

        var request = new AvailabilityRequest
        {
            AccommodationId = accommodation.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };

        await _service.CreateOrUpdate(request);

        _availabilityRepoMock.Verify(x => x.AddAsync(It.Is<Availability>(a =>
            a.AccommodationId == request.AccommodationId &&
            a.StartDate == request.StartDate &&
            a.EndDate == request.EndDate
        )), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrUpdate_ShouldUpdateExistingAvailability_WhenIdProvided()
    {
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var accommodation = new Accommodation { Id = Guid.NewGuid(), HostId = userId };
        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodation.Id)).ReturnsAsync(accommodation);

        var existing = new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodation.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3),
            Price = 50
        };
        _availabilityRepoMock.Setup(x => x.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _availabilityRepoMock.Setup(x => x.Query()).Returns(new List<Availability>().AsAsyncQueryable());

        var request = new AvailabilityRequest
        {
            Id = existing.Id,
            AccommodationId = accommodation.Id,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(4),
            Price = 100
        };

        await _service.CreateOrUpdate(request);

        existing.StartDate.Should().BeCloseTo(request.StartDate, TimeSpan.FromSeconds(1));
        existing.EndDate.Should().BeCloseTo(request.EndDate, TimeSpan.FromSeconds(1));
        existing.Price.Should().Be(request.Price);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}