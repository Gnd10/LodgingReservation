import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { RoomService } from '../../core/services/room.service';
import { RoomType } from '../../shared/models/room.model';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.css',
})
export class CatalogComponent implements OnInit {
  private roomService = inject(RoomService);
  private router = inject(Router);

  roomTypes: RoomType[] = [];
  filteredRoomTypes: RoomType[] = [];
  isLoading = true;
  errorMessage = '';

  // Form Filter
  checkInDate = '';
  checkOutDate = '';
  guestCount = 1;
  todayDate = ''; // Tanggal hari ini format YYYY-MM-DD

  ngOnInit(): void {
    // Set batas minimal tanggal pencarian ke hari ini
    const today = new Date();
    const yyyy = today.getFullYear();
    const mm = String(today.getMonth() + 1).padStart(2, '0');
    const dd = String(today.getDate()).padStart(2, '0');
    this.todayDate = `${yyyy}-${mm}-${dd}`;

    this.loadRoomTypes();
  }

  loadRoomTypes(): void {
    this.roomService.getRoomTypes().subscribe({
      next: (data) => {
        this.roomTypes = data;
        this.filteredRoomTypes = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Gagal memuat tipe kamar. Silakan coba lagi.';
      },
    });
  }

  applyFilter(): void {
    if (this.guestCount) {
      this.filteredRoomTypes = this.roomTypes.filter(
        (room) => room.capacity >= this.guestCount,
      );
    } else {
      this.filteredRoomTypes = this.roomTypes;
    }
  }

  resetFilter(): void {
    this.checkInDate = '';
    this.checkOutDate = '';
    this.guestCount = 1;
    this.filteredRoomTypes = this.roomTypes;
  }

  viewDetails(roomTypeId: number): void {
    this.router.navigate(['/catalog', roomTypeId], {
      queryParams: {
        checkIn: this.checkInDate,
        checkOut: this.checkOutDate,
        guests: this.guestCount,
      },
    });
  }
}
