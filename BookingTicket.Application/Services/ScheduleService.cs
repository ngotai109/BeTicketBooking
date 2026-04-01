using AutoMapper;
using BookingTicket.Application.DTOs.Schedule;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;

        public ScheduleService(IScheduleRepository scheduleRepository, IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto?> GetScheduleByIdAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdWithDetailsAsync(id);
            if (schedule == null) return null;
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto?> CreateScheduleAsync(CreateScheduleDto createScheduleDto)
        {
            // Parse times
            if (!TimeSpan.TryParse(createScheduleDto.DepartureTime, out var dTime) || 
                !TimeSpan.TryParse(createScheduleDto.ArrivalTime, out var aTime))
            {
                return null;
            }

            // 1. Validation: Prevent overlapping schedules for the same bus
            if (await IsOverlappingWithExistingAsync(createScheduleDto.BusId, dTime, aTime))
            {
                return null; // Return null to indicate validation failed (Overlap)
            }

            var entity = _mapper.Map<Schedules>(createScheduleDto);
            await _scheduleRepository.AddAsync(entity);
            var created = await _scheduleRepository.GetByIdWithDetailsAsync(entity.ScheduleId);
            return _mapper.Map<ScheduleDto>(created);
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(int id, CreateScheduleDto updateScheduleDto)
        {
            var existing = await _scheduleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            // Parse times
            if (!TimeSpan.TryParse(updateScheduleDto.DepartureTime, out var dTime) || 
                !TimeSpan.TryParse(updateScheduleDto.ArrivalTime, out var aTime))
            {
                return null;
            }

            // 1. Validation: Prevent overlapping schedules for the same bus (ignoring the current one)
            if (await IsOverlappingWithExistingAsync(updateScheduleDto.BusId, dTime, aTime, id))
            {
                return null;
            }

            _mapper.Map(updateScheduleDto, existing);
            await _scheduleRepository.UpdateAsync(existing);

            var updated = await _scheduleRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ScheduleDto>(updated);
        }

        private async Task<bool> IsOverlappingWithExistingAsync(int busId, TimeSpan d1, TimeSpan a1, int? excludeId = null)
        {
            var allSchedules = await _scheduleRepository.GetAllAsync();
            var activeSchedulesForBus = allSchedules.Where(s => s.BusId == busId && s.IsActive && s.ScheduleId != excludeId);

            foreach (var s in activeSchedulesForBus)
            {
                if (CheckOverlap(d1, a1, s.DepartureTime, s.ArrivalTime)) return true;
            }
            return false;
        }

        private bool CheckOverlap(TimeSpan d1, TimeSpan a1, TimeSpan d2, TimeSpan a2)
        {
            var intervals1 = GetIntervals(d1, a1);
            var intervals2 = GetIntervals(d2, a2);

            foreach (var i1 in intervals1)
            {
                foreach (var i2 in intervals2)
                {
                    // Overlap if max(start) < min(end)
                    if (i1.Item1 < i2.Item2 && i2.Item1 < i1.Item2)
                        return true;
                }
            }
            return false;
        }

        private List<(TimeSpan, TimeSpan)> GetIntervals(TimeSpan d, TimeSpan a)
        {
            var list = new List<(TimeSpan, TimeSpan)>();
            if (a > d)
            {
                list.Add((d, a));
            }
            else
            {
                // Trip crosses midnight (e.g., 23h - 05h)
                list.Add((d, TimeSpan.FromHours(24)));
                list.Add((TimeSpan.Zero, a));
            }
            return list;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null) return false;

            await _scheduleRepository.DeleteAsync(schedule);
            return true;
        }

        public async Task<ScheduleDto?> ToggleActiveScheduleAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null) return null;

            schedule.IsActive = !schedule.IsActive;
            await _scheduleRepository.UpdateAsync(schedule);

            var updated = await _scheduleRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ScheduleDto>(updated);
        }
    }
}
