using AutoMapper;
using BookingTicket.Application.DTOs.Route;
using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Mappings
{
    public class RouteMappingProfile : Profile
    {
        public RouteMappingProfile()
        {
            // Map từ Entity sang DTO
            CreateMap<Routes, RouteDto>()
                .ForMember(dest => dest.DepartureOfficeName, opt => opt.MapFrom(src => src.DepartureOffice.OfficeName))
                .ForMember(dest => dest.ArrivalOfficeName, opt => opt.MapFrom(src => src.ArrivalOffice.OfficeName));

            // Map từ DTO tạo mới sang Entity
            CreateMap<CreateRouteDto, Routes>();
        }
    }
}
