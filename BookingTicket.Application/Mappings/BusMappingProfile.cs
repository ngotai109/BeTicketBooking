using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingTicket.Application.DTOs.Province;
using BookingTicket.Domain.Entities;
using BookingTicket.Application.DTOs.Bus;

namespace BookingTicket.Application.Mappings
{
     public class BusMappingProfile : Profile
    {
        public BusMappingProfile()
        {
            CreateMap<Buses, BusDTO>()
                .ForMember(dest => dest.BusTypeName, opt => opt.MapFrom(src => src.BusType.TypeName));
            CreateMap<CreateBusDTO, Buses>();
            CreateMap<UpdateBusDTO, Buses>();

            CreateMap<BusTypes, BusTypeDto>().ReverseMap();
            CreateMap<CreateBusTypeDto, BusTypes>();
        }
    }
}
