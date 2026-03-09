using AutoMapper;
using BookingTicket.Application.DTOs.Location;
using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Mappings
{
    public class LocationMappingProfile : Profile
    {
        public LocationMappingProfile()
        {
         
            CreateMap<Locations, LocationDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.LocationName));

            CreateMap<CreateLocationDto, Locations>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName));

            CreateMap<UpdateLocationDto, Locations>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName));

        }
    }
}
