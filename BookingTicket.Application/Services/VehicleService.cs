using AutoMapper;
using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Application.DTOs.Route;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BookingTicket.Application.Services

{
    public class VehicleService : IVehicalService
    {
        private readonly IVehicalRepository _vehicalRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IBusTypeRepository _busTypeRepository;
        private readonly IMapper _mapper;

        public VehicleService(
            IVehicalRepository vehicalRepository, 
            ISeatRepository seatRepository,
            IBusTypeRepository busTypeRepository,
            IMapper mapper)
        {
            _vehicalRepository = vehicalRepository;
            _seatRepository = seatRepository;
            _busTypeRepository = busTypeRepository;
            _mapper = mapper;
        }

        public async Task<BusDTO> CreateBus(CreateBusDTO createBusDTO)
        {
            var bus = _mapper.Map<Buses>(createBusDTO);
            await _vehicalRepository.AddAsync(bus);

            // Tự động sinh ghế dựa trên BusType
            var busType = await _busTypeRepository.GetByIdAsync(bus.BusTypeId);
            if (busType != null)
            {
                var seats = GenerateDefaultSeats(bus.BusId, busType.DefaultSeats);
                foreach (var seat in seats)
                {
                    await _seatRepository.AddAsync(seat);
                }
            }
            
            var result = await _vehicalRepository.GetByIdWithDetailsAsync(bus.BusId);
            return _mapper.Map<BusDTO>(result);
        }

        private List<Seats> GenerateDefaultSeats(int busId, int count)
        {
            var seats = new List<Seats>();
            
            if (count == 16)
            {
                for (int i = 1; i <= 16; i++)
                {
                    seats.Add(new Seats { BusId = busId, SeatNumber = i.ToString("D2"), Floor = 1, IsActive = true });
                }
            }
            else if (count == 22)
            {
                // Tầng 1: D1-D5, C1-C5
                for (int i = 1; i <= 5; i++) {
                    seats.Add(new Seats { BusId = busId, SeatNumber = "D" + i, Floor = 1, IsActive = true });
                    seats.Add(new Seats { BusId = busId, SeatNumber = "C" + i, Floor = 1, IsActive = true });
                }
                // Tầng 2: A1-A6, B1-B5
                for (int i = 1; i <= 6; i++) seats.Add(new Seats { BusId = busId, SeatNumber = "A" + i, Floor = 2, IsActive = true });
                for (int i = 1; i <= 5; i++) seats.Add(new Seats { BusId = busId, SeatNumber = "B" + i, Floor = 2, IsActive = true });
            }
            else if (count == 34)
            {
                // Tầng 1
                string[] f1 = { "C2", "B2", "A2", "C4", "B4", "A4", "C6", "B6", "A6", "C8", "B8", "A10", "C12", "B10", "A12", "N1" };
                foreach(var n in f1) seats.Add(new Seats { BusId = busId, SeatNumber = n, Floor = 1, IsActive = true });
                // Tầng 2
                string[] f2 = { "C3", "B3", "A1", "C5", "B5", "A3", "C7", "B7", "A5", "C9", "B9", "A7", "C11", "N2", "A9", "A11" };
                foreach(var n in f2) seats.Add(new Seats { BusId = busId, SeatNumber = n, Floor = 2, IsActive = true });
            }
            else // Mặc định hoặc 40
            {
                // Tầng 1
                string[] f1 = { "C2", "B2", "A2", "C4", "B4", "A4", "C6", "B6", "A6", "C8", "B8", "A8", "C10", "B10", "A10", "D2", "D6", "D10", "D4", "D3", "L1", "L2" };
                foreach(var n in f1) seats.Add(new Seats { BusId = busId, SeatNumber = n, Floor = 1, IsActive = true });
                // Tầng 2
                string[] f2 = { "C1", "B1", "A1", "C3", "B3", "A3", "C5", "B5", "A5", "C7", "B7", "A7", "C9", "B9", "A9", "D1", "D5", "D9", "D8", "D7", "L3", "L4" };
                foreach(var n in f2) seats.Add(new Seats { BusId = busId, SeatNumber = n, Floor = 2, IsActive = true });
            }

            return seats;
        }

        public async Task<IEnumerable<BusDTO>> GetAllBusesAsync()
        {
            var buses = await _vehicalRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<BusDTO>>(buses);
        }

        public async Task<BusDTO> ToggleActiveVehicalAsync(int id, string status)
        {
            var bus =await _vehicalRepository.ToggleActiveVehicalAsync(id, status);
            if(bus == null)
            {
                return null;
            }
            var busDTO = _mapper.Map<BusDTO>(bus);
            return busDTO;
        }

        public async Task<BusDTO> UpdateBus(int id, UpdateBusDTO updateBusDTO)
        {
            var exitingVehiclle = await _vehicalRepository.GetByIdAsync(id);
            if (exitingVehiclle == null)
            {
                return null;
            }
            _mapper.Map(updateBusDTO, exitingVehiclle);
            await _vehicalRepository.UpdateAsync(exitingVehiclle);
            
            var result = await _vehicalRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<BusDTO>(result);
        }

        public async Task<BusDTO?> GetBusByIdAsync(int id)
        {
            var bus = await _vehicalRepository.GetByIdWithDetailsAsync(id);
            if (bus == null) return null;
            return _mapper.Map<BusDTO>(bus);
        }

        public async Task<bool> DeleteBusAsync(int id)
        {
            var bus = await _vehicalRepository.GetByIdAsync(id);
            if (bus == null) return false;

            await _vehicalRepository.DeleteAsync(bus);
            return true;
        }

        public async Task<IEnumerable<BusDTO>> GetAllActiveBusesAsync()
        {
            var buses = await _vehicalRepository.GetAllActiveBusesAsync();
            return _mapper.Map<IEnumerable<BusDTO>>(buses);
        }
    }
}
