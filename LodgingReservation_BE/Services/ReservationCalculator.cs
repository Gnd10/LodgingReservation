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
            public decimal LateCheckoutFee { get; set; }
            public decimal TierDiscount { get; set; }
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

            // Tiered Long-Stay Discount logic:
            // 3-6 nights -> 5% (0.05)
            // >= 7 nights -> 12% (0.12)
            decimal tierDiscount = 0;
            if (result.TotalNights >= 7)
            {
                tierDiscount = 0.12m;
            }
            else if (result.TotalNights >= 3)
            {
                tierDiscount = 0.05m;
            }
            result.TierDiscount = tierDiscount;

            // Calculate Room Subtotal with the tier discount
            decimal totalRoomNightCost = 0;
            foreach (var room in rooms)
            {
                decimal roomPrice = room.RoomType?.BasePrice ?? 0;
                totalRoomNightCost += roomPrice * result.TotalNights * (1 - tierDiscount);
            }
            result.RoomSubtotal = totalRoomNightCost;

            if (request.AddOns != null && request.AddOns.Any())
            {
                foreach (var item in request.AddOns)
                {
                    var extraService = await extraServiceRepo.GetByIdAsync(item.ExtraServiceId);
                    if (extraService != null)
                    {
                        decimal subTotalAddOn = (extraService.Type == UnitType.NIGHT || extraService.Type == UnitType.PERSON) 
                            ? extraService.Price * item.Quantity * result.TotalNights 
                            : extraService.Price * item.Quantity;

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
                    decimal calculatedDiscount = (result.RoomSubtotal + result.AddOnsTotal) * (promotion.DiscountPercentage / 100);
                    result.PromoDiscount = calculatedDiscount > promotion.MaxDiscountCap ? promotion.MaxDiscountCap : calculatedDiscount;
                }
            }

            // Late checkout fee calculation (standard 12:00, 15m grace, 50k/hour, max cap 250k)
            decimal lateCheckoutFee = 0;
            var checkoutTime = request.CheckOutDate.TimeOfDay;
            var standardCheckout = new TimeSpan(12, 0, 0);
            if (checkoutTime > standardCheckout)
            {
                var delay = checkoutTime - standardCheckout;
                if (delay.TotalMinutes > 15)
                {
                    int hoursToCharge = (int)Math.Ceiling(delay.TotalHours);
                    lateCheckoutFee = Math.Min(hoursToCharge * 50000m, 250000m);
                }
            }
            result.LateCheckoutFee = lateCheckoutFee;

            result.GrandTotal = (result.RoomSubtotal + result.AddOnsTotal + lateCheckoutFee) - result.PromoDiscount;
            if (result.GrandTotal < 0) result.GrandTotal = 0;

            return result;
        }
    }
}
