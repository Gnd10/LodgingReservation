import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { BookingService } from '../../core/services/booking.service';
import { Reservation } from '../../shared/models/reservation.model';

@Component({
  selector: 'app-booking-history',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './booking-history.component.html',
  styleUrl: './booking-history.component.css',
})
export class BookingHistoryComponent {
  private bookingService = inject(BookingService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  bookings: Reservation[] = [];
  isLoading = true;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.bookingService.getMyHistory().subscribe({
      next: (data) => {
        // Mengurutkan dari pesanan terbaru (berdasarkan ID terbesar)
        this.bookings = data.sort((a, b) => (b.id ?? 0) - (a.id ?? 0));
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Gagal memuat riwayat booking Anda.';
        this.cdr.detectChanges();
      },
    });
  }

  onCancelBooking(id: number): void {
    const confirmCancel = confirm('Anda yakin ingin membatalkan pesanan?');
    if (!confirmCancel) return;
    this.successMessage = '';
    this.errorMessage = '';
    this.cdr.detectChanges();
    this.bookingService.cancelBooking(id).subscribe({
      next: () => {
        this.successMessage = 'Pesanan berhasil dibatalkan.';
        this.loadBookings(); // Reload data setelah batal
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Gagal membatalkan.';
        this.cdr.detectChanges();
      },
    });
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
