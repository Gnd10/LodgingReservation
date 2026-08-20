using LodgingReservation_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LodgingReservation_BE.Data
{
    public class LodgingReservationDbContext : DbContext
    {
        public LodgingReservationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ExtraService> ExtraServices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationAddOn> ReservationAddOns { get; set; }
        public DbSet<ReservationRoom> ReservationRooms { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email)
                    .IsUnique();

            // ===== PROMOTION =====
            modelBuilder.Entity<Promotion>()
                .HasIndex(p => p.PromoCode)
                .IsUnique();

            // ===== ROOM =====
            modelBuilder.Entity<Room>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            // RoomType (1) -> Room (N)
            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== EXTRA SERVICE =====
            modelBuilder.Entity<ExtraService>()
                .Property(e => e.Type)
                .HasConversion<string>();

            // ===== RESERVATION =====
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.BookingCode)
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();

            // User (1) -> Reservation (N)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Promotion (1) -> Reservation (N)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Promotion)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.PromotionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== RESERVATION ROOM (pivot Reservation <-> Room) =====
            // Reservation (1) -> ReservationRoom (N)
            modelBuilder.Entity<ReservationRoom>()
                .HasOne(rr => rr.Reservation)
                .WithMany(r => r.ReservationRooms)
                .HasForeignKey(rr => rr.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room (1) -> ReservationRoom (N)
            modelBuilder.Entity<ReservationRoom>()
                .HasOne(rr => rr.Room)
                .WithMany(room => room.ReservationRooms)
                .HasForeignKey(rr => rr.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== RESERVATION ADD-ONS (pivot Reservation <-> ExtraService) =====
            // Reservation (1) -> ReservationAddOns (N)
            modelBuilder.Entity<ReservationAddOn>()
                .HasOne(ra => ra.Reservation)
                .WithMany(r => r.ReservationAddOns)
                .HasForeignKey(ra => ra.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExtraService (1) -> ReservationAddOns (N)
            modelBuilder.Entity<ReservationAddOn>()
                .HasOne(ra => ra.ExtraService)
                .WithMany(es => es.ReservationAddOns)
                .HasForeignKey(ra => ra.ExtraServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== PAYMENT =====
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.Method)
                .HasConversion<string>();

            // Reservation (1) -> Payment (N)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Reservation)
                .WithMany(r => r.Payments)
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
