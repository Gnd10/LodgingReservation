using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
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
            public long? PromotionIdToSave { get; set; } 
            public List<ReservationAddOn> AddOns { get; set; } = new();
        }

        public async Task<CalculationResult> CalculateAsync(
            CreateReservation request,
            List<Room> rooms, 
            IRepository<ExtraService> extraServiceRepo,
            IRepository<Promotion> promotionRepo)
        {
            var result = new CalculationResult();

            if (request.CheckOutDate.Date <= request.CheckInDate.Date)
            {
                throw new ArgumentException("Tanggal check-out harus setelah tanggal check-in.");
            }
            result.TotalNights = (request.CheckOutDate.Date - request.CheckInDate.Date).Days;

            decimal totalRoomNightCost = 0;
            foreach (var room in rooms)
            {
                decimal roomPrice = room.RoomType?.BasePrice ?? 0;
                totalRoomNightCost += roomPrice * result.TotalNights;
            }
            result.RoomSubtotal = totalRoomNightCost;

            if (request.AddOns != null && request.AddOns.Any())
            {
                foreach (var item in request.AddOns)
                {
                    var extraService = await extraServiceRepo.GetByIdAsync(item.ExtraServiceId);
                    if (extraService != null)
                    {
                        decimal subTotalAddOn = 0;

                        if (extraService.Type == UnitType.NIGHT)
                        {
                            subTotalAddOn = extraService.Price * item.Quantity * result.TotalNights;
                        }
                        else
                        {
                            subTotalAddOn = extraService.Price * item.Quantity;
                        }

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

            if (request.PromotionId.HasValue && request.PromotionId.Value > 0)
            {
                var promotion = await promotionRepo.GetByIdAsync(request.PromotionId.Value);
                if (promotion != null && promotion.IsActive && promotion.ValidUntil.Date >= DateTime.UtcNow.Date)
                {
                    result.PromotionIdToSave = promotion.Id;
                    
                    decimal calculatedDiscount = result.RoomSubtotal * (promotion.DiscountPercentage / 100);

                    result.PromoDiscount = calculatedDiscount > promotion.MaxDiscountCap 
                        ? promotion.MaxDiscountCap 
                        : calculatedDiscount;
                }
            }

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