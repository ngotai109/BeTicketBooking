using AutoMapper;
using BookingTicket.Application.DTOs.Schedule;
using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Mappings
{
    public class ScheduleMappingProfile : Profile
    {
        public ScheduleMappingProfile()
        {
            // Entity -> DTO
            CreateMap<Schedules, ScheduleDto>()
                .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Route != null ? src.Route.RouteName : null))
                .ForMember(dest => dest.BusPlate, opt => opt.MapFrom(src => src.Bus != null ? src.Bus.PlateNumber : null))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime.ToString(@"hh\:mm")))
                .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.ArrivalTime.ToString(@"hh\:mm")));

            // DTO -> Entity
            CreateMap<CreateScheduleDto, Schedules>();
        }
    }
}
