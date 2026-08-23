import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { RoomType } from '../../../shared/models/room.model';
import { ExtraService } from '../../../shared/models/extra-service.model';
import { RupiahPipe } from '../../../shared/pipes/rupiah.pipe';

@Component({
  selector: 'app-room-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, RupiahPipe],
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.css']
})
export class RoomDetailComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(ApiService);

  room?: RoomType;
  extras: ExtraService[] = [];
  loading = true;
  error = '';
  selectedRoomId?: number;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.getRoomType(id).subscribe({
      next: room => { this.room = room; this.loading = false; },
      error: err => { this.error = err.error?.message || 'Detail kamar gagal dimuat.'; this.loading = false; }
    });
    this.api.getExtraServices().subscribe({ next: extras => this.extras = extras });
  }

  isAvailable(status: string | number): boolean {
    return status === 'AVAILABLE' || status === 0;
  }

  checkout(): void {
    if (!this.room) return;
    this.router.navigate(['/booking'], {
      queryParams: { roomTypeId: this.room.id, roomId: this.selectedRoomId || '' }
    });
  }
}
