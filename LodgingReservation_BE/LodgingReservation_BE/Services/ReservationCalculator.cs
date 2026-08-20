using LodgingReservation_BE.Models;
using LodgingReservation_BE.Repositories;
using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public class ReservationCalculator
    {
        public class CalculationResult
        {
            public int TotalNights { get; set; }
            public decimal RoomSubtotal { get; set; }
            public decimal AddOnsTotal { get; set; }
            public decimal PromoDiscount { get; set; }
            public decimal GrandTotal { get; set; }
            public long PromotionIdToSave { get; set; }
            public List<ReservationAddOn> AddOns { get; set; } = new();
        }

        public async Task<CalculationResult> CalculateAsync(
            CreateReservation request,
            Room room,
            IRepository<ExtraService> extraServiceRepo,
            IRepository<Promotion> promotionRepo)
        {
            var result = new CalculationResult();

            // 1. Hitung Total Malam
            if (request.CheckOutDate.Date <= request.CheckInDate.Date)
            {
                throw new ArgumentException("Tanggal check-out harus setelah tanggal check-in.");
            }
            result.TotalNights = (request.CheckOutDate.Date - request.CheckInDate.Date).Days;

            // 2. Hitung Subtotal Kamar
            decimal pricePerNight = room.RoomType?.BasePrice ?? 0;
            result.RoomSubtotal = pricePerNight * result.TotalNights;

            // 3. Hitung Add-Ons
            if (request.AddOns != null && request.AddOns.Any())
            {
                foreach (var item in request.AddOns)
                {
                    var extraService = await extraServiceRepo.GetByIdAsync(item.ExtraServiceId);
                    if (extraService != null)
                    {
                        decimal subTotalAddOn = extraService.Price * item.Quantity;
                        result.AddOnsTotal += subTotalAddOn;

                        result.AddOns.Add(new ReservationAddOn
                        {
                            ExtraServiceId = extraService.Id,
                            Quantity = item.Quantity,
                            UnitPrice = extraService.Price,
                            SubTotal = subTotalAddOn
                        });
                    }
                }
            }

            // 4. Kalkulasi Diskon Promo (Dengan MaxDiscountCap)
            if (request.PromotionId.HasValue && request.PromotionId.Value > 0)
            {
                var promotion = await promotionRepo.GetByIdAsync(request.PromotionId.Value);
                if (promotion != null && promotion.IsActive && promotion.ValidUntil >= DateTime.UtcNow)
                {
                    result.PromotionIdToSave = promotion.Id;
                    decimal calculatedDiscount = result.RoomSubtotal * (promotion.DiscountPercentage / 100);

                    // Batasi dengan MaxDiscountCap
                    result.PromoDiscount = calculatedDiscount > promotion.MaxDiscountCap ? promotion.MaxDiscountCap : calculatedDiscount;
                }
            }

            // 5. Hitung Grand Total
            if (request.LateCheckoutFee < 0)
            {
                throw new ArgumentException("Late checkout fee tidak boleh bernilai negatif.");
            }
            decimal lateCheckoutFee = request.LateCheckoutFee;
            result.GrandTotal = (result.RoomSubtotal + result.AddOnsTotal + lateCheckoutFee) - result.PromoDiscount;
            if (result.GrandTotal < 0) result.GrandTotal = 0;

            return result;
        }
    }
}
