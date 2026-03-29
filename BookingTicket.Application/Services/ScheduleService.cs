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

        public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createScheduleDto)
        {
            var entity = _mapper.Map<Schedules>(createScheduleDto);
            await _scheduleRepository.AddAsync(entity);
            var created = await _scheduleRepository.GetByIdWithDetailsAsync(entity.ScheduleId);
            return _mapper.Map<ScheduleDto>(created);
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(int id, CreateScheduleDto updateScheduleDto)
        {
            var existing = await _scheduleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(updateScheduleDto, existing);
            await _scheduleRepository.UpdateAsync(existing);

            var updated = await _scheduleRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ScheduleDto>(updated);
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
