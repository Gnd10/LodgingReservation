import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { RoomType } from '../../../shared/models/room.model';
import { RupiahPipe } from '../../../shared/pipes/rupiah.pipe';

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CommonModule, RouterLink, RupiahPipe],
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.css']
})
export class RoomListComponent {
  private api = inject(ApiService);
  rooms: RoomType[] = [];
  loading = true;

  ngOnInit(): void {
    this.api.getRoomTypes().subscribe({
      next: data => { this.rooms = data; this.loading = false; },
      error: () => this.loading = false
    });
  }
}
