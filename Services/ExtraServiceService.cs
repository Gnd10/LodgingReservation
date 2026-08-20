using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Repositories;

namespace LodgingReservation_BE.Services
{
    public class ExtraServiceService : IExtraServiceService
    {
        private readonly IRepository<ExtraService> _repository;

        public ExtraServiceService(IRepository<ExtraService> repository)
        {
            _repository = repository;
        }

        public async Task<List<ExtraServiceResponse>> GetAllAsync(string? search = null)
        {
            var items = await _repository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = items.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return items.Select(ToResponse).ToList();
        }

        public async Task<ExtraServiceResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : ToResponse(entity);
        }

        public async Task<ExtraServiceResponse> CreateAsync(ExtraServiceRequest request)
        {
            var entity = new ExtraService
            {
                Name = request.Name.Trim(),
                Price = request.Price,
                Type = request.Type
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToResponse(entity);
        }

        public async Task<ExtraServiceResponse?> UpdateAsync(long id, ExtraServiceRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Name = request.Name.Trim();
            entity.Price = request.Price;
            entity.Type = request.Type;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return ToResponse(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            try
            {
                _repository.Delete(entity);
                await _repository.SaveChangesAsync();
                return true;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                throw new InvalidOperationException("Extra service tidak dapat dihapus karena sudah digunakan pada reservasi.");
            }
        }

        private static ExtraServiceResponse ToResponse(ExtraService entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            Type = entity.Type
        };
    }
}
