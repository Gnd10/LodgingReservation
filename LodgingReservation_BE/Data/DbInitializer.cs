using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace LodgingReservation_BE.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(LodgingReservationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.RoomTypes.AnyAsync())
            {
                var roomTypes = new List<RoomType>
                {
                    new RoomType
                    {
                        Name = "Standard Room",
                        BasePrice = 350000,
                        Capacity = 2,
                        Description = "Kamar nyaman dengan fasilitas dasar lengkap, cocok untuk pelancong tunggal atau pasangan. Dilengkapi AC, TV, WiFi gratis, dan kamar mandi dalam.",
                        ImageUrl = "https://images.unsplash.com/photo-1598928506311-c55ded91a20c?q=80&w=600&auto=format&fit=crop"
                    },
                    new RoomType
                    {
                        Name = "Deluxe Room",
                        BasePrice = 550000,
                        Capacity = 3,
                        Description = "Kamar yang lebih luas dengan tempat tidur king-size, sofa kecil, kulkas mini, fasilitas pembuat kopi/teh, dan pemandangan luar yang indah.",
                        ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?q=80&w=600&auto=format&fit=crop"
                    },
                    new RoomType
                    {
                        Name = "Executive Suite",
                        BasePrice = 950000,
                        Capacity = 4,
                        Description = "Kamar premium mewah dengan ruang tamu terpisah, tempat tidur ekstra nyaman, bathtub di kamar mandi, mini-bar gratis, dan layanan sarapan khusus.",
                        ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?q=80&w=600&auto=format&fit=crop"
                    }
                };

                await context.RoomTypes.AddRangeAsync(roomTypes);
                await context.SaveChangesAsync();
            }

            if (!await context.Rooms.AnyAsync())
            {
                var standardType = await context.RoomTypes.FirstAsync(rt => rt.Name == "Standard Room");
                var deluxeType = await context.RoomTypes.FirstAsync(rt => rt.Name == "Deluxe Room");
                var executiveType = await context.RoomTypes.FirstAsync(rt => rt.Name == "Executive Suite");

                var rooms = new List<Room>
                {
                    new Room { RoomNumber = "101", RoomTypeId = standardType.Id, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "102", RoomTypeId = standardType.Id, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "103", RoomTypeId = standardType.Id, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "201", RoomTypeId = deluxeType.Id, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "202", RoomTypeId = deluxeType.Id, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "203", RoomTypeId = deluxeType.Id, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "301", RoomTypeId = executiveType.Id, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "302", RoomTypeId = executiveType.Id, Status = RoomStatus.AVAILABLE }
                };

                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
            }

            if (!await context.ExtraServices.AnyAsync())
            {
                var extraServices = new List<ExtraService>
                {
                    new ExtraService { Name = "Sarapan Pagi (Breakfast)", Price = 50000, Type = UnitType.PERSON },
                    new ExtraService { Name = "Kasur Tambahan (Extra Bed)", Price = 100000, Type = UnitType.NIGHT },
                    new ExtraService { Name = "Jemputan Bandara (Airport Shuttle)", Price = 150000, Type = UnitType.TRIP },
                    new ExtraService { Name = "Late Check-out", Price = 75000, Type = UnitType.TRIP }
                };

                await context.ExtraServices.AddRangeAsync(extraServices);
                await context.SaveChangesAsync();
            }

            if (!await context.Promotions.AnyAsync())
            {
                var promotions = new List<Promotion>
                {
                    new Promotion
                    {
                        PromoCode = "STAYNEW",
                        DiscountPercentage = 10,
                        ValidUntil = DateTime.UtcNow.AddMonths(3),
                        IsActive = true,
                        MaxDiscountCap = 50000
                    },
                    new Promotion
                    {
                        PromoCode = "PROMO20",
                        DiscountPercentage = 20,
                        ValidUntil = DateTime.UtcNow.AddMonths(1),
                        IsActive = true,
                        MaxDiscountCap = 100000
                    }
                };

                await context.Promotions.AddRangeAsync(promotions);
                await context.SaveChangesAsync();
            }
        }
    }
}