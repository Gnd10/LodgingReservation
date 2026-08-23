import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { ReservationResponse } from '../../../shared/models/reservation.model';
import { RupiahPipe } from '../../../shared/pipes/rupiah.pipe';

@Component({
  selector: 'app-invoice',
  standalone: true,
  imports: [CommonModule, RouterLink, RupiahPipe],
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.css']
})
export class InvoiceComponent {
  private route = inject(ActivatedRoute);
  private api = inject(ApiService);

  reservation?: ReservationResponse;
  loading = true;
  error = '';

  ngOnInit(): void {
    const id = Number(this.route.parent?.snapshot.paramMap.get('id') || this.route.snapshot.paramMap.get('id'));
    this.api.getReservation(id).subscribe({
      next: data => { this.reservation = data; this.loading = false; },
      error: err => { this.error = err.error?.message || 'Invoice tidak dapat dimuat.'; this.loading = false; }
    });
  }

  print(): void { window.print(); }
}
