using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Repositories;

namespace LodgingReservation_BE.Services
{
    public class ExtraService : IExtraServiceService
    {
        private readonly IRepository<ExtraService> _extraServiceRepository;

        public ExtraServiceService(IRepository<ExtraService> extraServiceRepository)
        {
            _extraServiceRepository = extraServiceRepository;
        }

        public async Task<IEnumerable<ExtraServiceResponseDto>> GetAllAsync(bool activeOnly = true)
        {
            var services = await _extraServiceRepository.GetAllAsync();

            if (activeOnly)
            {
                services = services.Where(s => s.IsActive).ToList();
            }

            return services.Select(s => new ExtraServiceResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                IsActive = s.IsActive
            });
        }

        public async Task<ExtraServiceResponseDto?> GetByIdAsync(long id)
        {
            var service = await _extraServiceRepository.GetByIdAsync(id);
            if (service == null) return null;

            return new ExtraServiceResponseDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                IsActive = service.IsActive
            };
        }
    }
}