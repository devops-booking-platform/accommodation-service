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
}