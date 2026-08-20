using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LodgingReservation_BE.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IRepository<Promotion> _repository;

        public PromotionService(IRepository<Promotion> repository)
        {
            _repository = repository;
        }

        public async Task<List<PromotionResponse>> GetAllAsync(bool? active = null)
        {
            var items = await _repository.GetAllAsync();
            if (active.HasValue)
                items = items.Where(x => x.IsActive == active.Value).ToList();

            return items.OrderByDescending(x => x.Id).Select(ToResponse).ToList();
        }

        public async Task<PromotionResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : ToResponse(entity);
        }

        public async Task<PromotionResponse?> GetByCodeAsync(string code)
        {
            var items = await _repository.GetAllAsync();
            var entity = items.FirstOrDefault(x => x.PromoCode.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
            return entity == null ? null : ToResponse(entity);
        }

        public async Task<PromotionResponse> CreateAsync(PromotionRequest request)
        {
            var code = request.PromoCode.Trim().ToUpperInvariant();
            var existing = await GetByCodeAsync(code);
            if (existing != null)
                throw new InvalidOperationException($"Promo code '{code}' sudah digunakan.");

            Validate(request);

            var entity = new Promotion
            {
                PromoCode = code,
                DiscountPercentage = request.DiscountPercentage,
                MaxDiscountCap = request.MaxDiscountCap,
                ValidUntil = request.ValidUntil,
                IsActive = request.IsActive
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToResponse(entity);
        }

        public async Task<PromotionResponse?> UpdateAsync(long id, PromotionRequest request)
        {
            Validate(request);
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var code = request.PromoCode.Trim().ToUpperInvariant();
            var all = await _repository.GetAllAsync();
            if (all.Any(x => x.Id != id && x.PromoCode.Equals(code, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Promo code '{code}' sudah digunakan.");

            entity.PromoCode = code;
            entity.DiscountPercentage = request.DiscountPercentage;
            entity.MaxDiscountCap = request.MaxDiscountCap;
            entity.ValidUntil = request.ValidUntil;
            entity.IsActive = request.IsActive;

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
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("Promotion tidak dapat dihapus karena sudah digunakan pada reservasi.");
            }
        }

        private static void Validate(PromotionRequest request)
        {
            if (request.DiscountPercentage < 0 || request.DiscountPercentage > 100)
                throw new ArgumentException("DiscountPercentage harus antara 0 sampai 100.");
            if (request.MaxDiscountCap < 0)
                throw new ArgumentException("MaxDiscountCap tidak boleh negatif.");
            if (request.ValidUntil <= DateTime.UtcNow)
                throw new ArgumentException("ValidUntil harus berupa tanggal di masa depan.");
        }

        private static PromotionResponse ToResponse(Promotion entity) => new()
        {
            Id = entity.Id,
            PromoCode = entity.PromoCode,
            DiscountPercentage = entity.DiscountPercentage,
            MaxDiscountCap = entity.MaxDiscountCap,
            ValidUntil = entity.ValidUntil,
            IsActive = entity.IsActive
        };
    }
}
