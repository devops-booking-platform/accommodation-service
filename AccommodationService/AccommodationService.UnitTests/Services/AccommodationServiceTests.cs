using AccommodationService.Common.Events;
using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AccommodationService.Domain.Enums;
using AccommodationService.Repositories.Interfaces;
using AccommodationService.Services.Interfaces;
using AutoMapper;
using Moq;

namespace AccommodationService.UnitTests.Services;

public class AccommodationServiceTests
{
    private readonly Mock<IRepository<Accommodation>> _accommodationRepoMock;
    private readonly Mock<IRepository<Amenity>> _amenityRepoMock;
	private readonly Mock<IRepository<Availability>> _availabilityRepoMock;
	private readonly Mock<IRepository<Photo>> _photoRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IMapper> _mapper;

    private readonly AccommodationService.Services.AccommodationService _service;

    public AccommodationServiceTests()
    {
        _accommodationRepoMock = new Mock<IRepository<Accommodation>>();
        _amenityRepoMock = new Mock<IRepository<Amenity>>();
        _availabilityRepoMock = new Mock<IRepository<Availability>>();
        _photoRepoMock = new Mock<IRepository<Photo>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _eventBusMock = new Mock<IEventBus>();
        _mapper = new Mock<IMapper>();


        _service = new AccommodationService.Services.AccommodationService(
            _accommodationRepoMock.Object,
            _amenityRepoMock.Object,
			_photoRepoMock.Object,
			_availabilityRepoMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object,
            _mapper.Object,
            _eventBusMock.Object
        );
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenUserNotAuthenticated()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);
        var request = new AccommodationRequest();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Create(request));
    }

    [Fact]
    public async Task Create_ShouldAddAccommodationAndPhotos_WhenValidRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var amenity1 = new Amenity { Id = Guid.NewGuid(), Name = "Pool" };
        var amenity2 = new Amenity { Id = Guid.NewGuid(), Name = "WiFi" };
        var amenities = new List<Amenity> { amenity1, amenity2 }.AsAsyncQueryable();

        var request = new AccommodationRequest
        {
            Name = "Test House",
            Description = "Nice place",
            Photos = new List<string> { "photo1.jpg", "photo2.jpg" },
            Amenities = amenities.Select(a => a.Id).ToList(),
            Location = new LocationRequest {Country = "USA", City = "Seattle", Address = "Home", PostalCode = "12321"}
        };

        // Mock amenity repository
        _amenityRepoMock.Setup(x => x.Query()).Returns(amenities);

        // Act
        await _service.Create(request);

        // Assert Accommodation added
        _accommodationRepoMock.Verify(x => x.AddAsync(It.Is<Accommodation>(a =>
            a.Name == request.Name &&
            a.Description == request.Description &&
            a.HostId == userId &&
            a.Amenities.Count == 2
        )), Times.Once);

        // Assert Photos added
        _photoRepoMock.Verify(x => x.AddRangeAsync(It.Is<List<Photo>>(photos =>
            photos.Count == request.Photos.Count
        )), Times.Once);

        // Assert SaveChanges called
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldOnlyIncludeRequestedAmenities()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var amenity1 = new Amenity { Id = Guid.NewGuid(), Name = "Pool", Description = "Desc Pool"};
        var amenity2 = new Amenity { Id = Guid.NewGuid(), Name = "WiFi", Description =  "Desc WiFi" };
        var allAmenities = new List<Amenity> { amenity1, amenity2 }.AsAsyncQueryable();

        var request = new AccommodationRequest
        {
            Name = "House",
            Description = "Nice",
            Photos = new List<string>(),
            PriceType = PriceType.PerGuest,
            Amenities = new List<Guid> { amenity2.Id },
            Location = new LocationRequest {Country = "USA", City = "Seattle", Address = "Home", PostalCode = "12321"}
        };

        _amenityRepoMock.Setup(x => x.Query()).Returns(allAmenities);

        // Act
        await _service.Create(request);

        // Assert
        _accommodationRepoMock.Verify(x => x.AddAsync(It.Is<Accommodation>(a =>
            a.Amenities.Count == 1 &&
            a.Amenities.First().Id == amenity2.Id
        )), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldThrow_WhenUserNotAuthenticated()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);
        var request = new AccommodationRequest();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Update(request));
    }

    [Fact]
    public async Task Update_ShouldThrow_WhenIdNotProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        var request = new AccommodationRequest { Id = null };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Update(request));
    }
    
    [Fact]
    public async Task GetReservationInfoAsync_ShouldThrow_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var accommodationId = Guid.NewGuid();
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var end = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
            _service.GetReservationInfoAsync(accommodationId, start, end, 2, CancellationToken.None));
    }

    [Fact]
    public async Task GetReservationInfoAsync_ShouldThrow_WhenGuestsNotPositive()
    {
        // Arrange
        var accommodationId = Guid.NewGuid();
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var end = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
            _service.GetReservationInfoAsync(accommodationId, start, end, 0, CancellationToken.None));
    }

    [Fact]
    public async Task GetReservationInfoAsync_ShouldThrow_WhenAccommodationNotFound()
    {
        // Arrange
        var accommodationId = Guid.NewGuid();
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var end = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodationId))
            .ReturnsAsync((Accommodation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Common.Exceptions.NotFoundException>(() => 
            _service.GetReservationInfoAsync(accommodationId, start, end, 2, CancellationToken.None));
    }

    [Fact]
    public async Task GetReservationInfoAsync_ShouldThrow_WhenGuestsExceedMaxCapacity()
    {
        // Arrange
        var accommodationId = Guid.NewGuid();
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var end = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        var accommodation = new Accommodation 
        { 
            Id = accommodationId, 
            MaximumNumberOfGuests = 4 
        };

        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodationId))
            .ReturnsAsync(accommodation);

        // Act & Assert
        await Assert.ThrowsAsync<Common.Exceptions.ConflictException>(() => 
            _service.GetReservationInfoAsync(accommodationId, start, end, 5, CancellationToken.None));
    }

    [Fact]
    public async Task GetReservationInfoAsync_ShouldCalculatePrice_WhenAvailabilityExists()
    {
        // Arrange
        var accommodationId = Guid.NewGuid();
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var end = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));

        var accommodation = new Accommodation 
        { 
            Id = accommodationId,
            Name = "Test Hotel",
            HostId = Guid.NewGuid(),
            MaximumNumberOfGuests = 4,
            IsAutoConfirm = true,
            PriceType = PriceType.PerUnit
        };

        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodationId,
            StartDate = start,
            EndDate = end,
            Price = 100m
        };

        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodationId))
            .ReturnsAsync(accommodation);

        _availabilityRepoMock.Setup(x => x.Query())
            .Returns(new List<Availability> { availability }.AsAsyncQueryable());

        // Act
        var result = await _service.GetReservationInfoAsync(accommodationId, start, end, 2, CancellationToken.None);

        // Assert
        Assert.Equal("Test Hotel", result.Name);
        Assert.Equal(accommodation.HostId, result.HostId);
        Assert.Equal(300m, result.TotalPrice); // 3 nights * 100
    }

    [Fact]
    public async Task GetReservationInfoAsync_ShouldCalculatePricePerGuest_WhenPriceTypeIsPerGuest()
    {
        // Arrange
        var accommodationId = Guid.NewGuid();
        var start = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var end = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        var accommodation = new Accommodation 
        { 
            Id = accommodationId,
            Name = "Test Hotel",
            HostId = Guid.NewGuid(),
            MaximumNumberOfGuests = 4,
            IsAutoConfirm = true,
            PriceType = PriceType.PerGuest
        };

        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            AccommodationId = accommodationId,
            StartDate = start,
            EndDate = end,
            Price = 50m
        };

        _accommodationRepoMock.Setup(x => x.GetByIdAsync(accommodationId))
            .ReturnsAsync(accommodation);

        _availabilityRepoMock.Setup(x => x.Query())
            .Returns(new List<Availability> { availability }.AsAsyncQueryable());

        // Act
        var result = await _service.GetReservationInfoAsync(accommodationId, start, end, 2, CancellationToken.None);

        // Assert
        Assert.Equal(200m, result.TotalPrice); // 2 nights * 50 * 2 guests
    }

    [Fact]
    public async Task GetMyAsync_ShouldThrow_WhenUserNotAuthenticated()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.GetMyAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetMyAsync_ShouldReturnHostAccommodations()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var accommodations = new List<Accommodation>
        {
            new Accommodation 
            { 
                Id = Guid.NewGuid(), 
                Name = "Hotel 1", 
                HostId = userId,
                MinimumNumberOfGuests = 1,
                MaximumNumberOfGuests = 4,
                Location = new Location { City = "Seattle", Country = "USA", Address = "123 Main St" }
            },
            new Accommodation 
            { 
                Id = Guid.NewGuid(), 
                Name = "Hotel 2", 
                HostId = userId,
                MinimumNumberOfGuests = 2,
                MaximumNumberOfGuests = 6,
                Location = null
            }
        };

        _accommodationRepoMock.Setup(x => x.Query())
            .Returns(accommodations.AsAsyncQueryable());

        // Act
        var result = await _service.GetMyAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Hotel 1", result[0].Name);
        Assert.Equal("Seattle, USA, 123 Main St", result[0].Address);
        Assert.Equal("", result[1].Address);
    }

    [Fact]
    public async Task DeleteHostAccommodationsAsync_ShouldNotPublishEvent_WhenNoAccommodations()
    {
        // Arrange
        var hostId = Guid.NewGuid();

        _accommodationRepoMock.Setup(x => x.Query())
            .Returns(new List<Accommodation>().AsAsyncQueryable());

        // Act
        await _service.DeleteHostAccommodationsAsync(hostId, CancellationToken.None);

        // Assert
        _eventBusMock.Verify(x => x.PublishAsync(
            It.IsAny<Common.Events.Published.HostAccommodationsDeletedIntegrationEvent>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}