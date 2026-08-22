using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Repositories;

namespace LodgingReservation_BE.Services
{
    public class ExtraServiceService : IExtraServiceService
    {
        private readonly IRepository<Models.ExtraService> _extraServiceRepository;

        public ExtraServiceService(IRepository<Models.ExtraService> extraServiceRepository)
        {
            _extraServiceRepository = extraServiceRepository;
        }

        public async Task<IEnumerable<ExtraServiceResponseDto>> GetAllAsync()
        {
            var services = await _extraServiceRepository.GetAllAsync();

            return services.Select(s => new ExtraServiceResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price
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
                Price = service.Price
            };
        }
    }
}