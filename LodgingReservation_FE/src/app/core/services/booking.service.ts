import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Reservation } from '../../shared/models/reservation.model';

@Injectable({
  providedIn: 'root',
})
export class BookingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/reservations`;

  getMyHistory(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.apiUrl}/my-history`);
  }
  cancelBooking(id: number): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/cancel`, {});
  }
}
