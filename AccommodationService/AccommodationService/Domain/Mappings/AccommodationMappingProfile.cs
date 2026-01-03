using AccommodationService.Domain.DTOs;
using AccommodationService.Domain.Entities;
using AutoMapper;

namespace AccommodationService.Domain.Mappings;

public class AccommodationMappingProfile : Profile
{
    public AccommodationMappingProfile()
    {
        // Accommodation -> AccommodationResponseDTO
        CreateMap<Accommodation, GetAccommodationResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsAutoConfirm, opt => opt.MapFrom(src => src.IsAutoConfirm))
            .ForMember(dest => dest.MinimumNumberOfGuests, opt => opt.MapFrom(src => src.MinimumNumberOfGuests))
            .ForMember(dest => dest.MaximumNumberOfGuests, opt => opt.MapFrom(src => src.MaximumNumberOfGuests))
            .ForMember(dest => dest.PriceType, opt => opt.MapFrom(src => src.PriceType))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos.Select(p => p.Url)))
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Amenities))
            .ForMember(dest => dest.Availabilities, opt => opt.MapFrom(src => src.Availabilities));

        // Location -> LocationResponseDTO
        CreateMap<Location, LocationResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode));

        // Amenity -> AmenityResponseDTO
        CreateMap<Amenity, AmenityResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        // Availability -> AvailabilityResponseDTO
        CreateMap<Availability, AvailabilityResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));
    }
}