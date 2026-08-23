import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { RoomType } from '../../shared/models/room.model';
import { ExtraService } from '../../shared/models/extra-service.model';
import { Promotion, ValidatePromoResponse } from '../../shared/models/promotion.model';
import { ReservationRequest, ReservationResponse } from '../../shared/models/reservation.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getRoomType(id: number): Observable<RoomType> {
    return this.http.get<RoomType>(`${this.base}/room-types/${id}`);
  }

  getRoomTypes(): Observable<RoomType[]> {
    return this.http.get<RoomType[]>(`${this.base}/room-types`);
  }

  getExtraServices(): Observable<ExtraService[]> {
    return this.http.get<ExtraService[]>(`${this.base}/extra-services`);
  }

  getPromotions(): Observable<Promotion[]> {
    return this.http.get<Promotion[]>(`${this.base}/promotions/active`);
  }

  validatePromo(promoCode: string, totalAmount: number): Observable<ValidatePromoResponse> {
    return this.http.post<ValidatePromoResponse>(`${this.base}/promotions/validate`, {
      promoCode,
      totalAmount
    });
  }

  createReservation(payload: ReservationRequest): Observable<ReservationResponse> {
    return this.http.post<ReservationResponse>(`${this.base}/reservations`, payload);
  }

  getReservation(id: number): Observable<ReservationResponse> {
    return this.http.get<ReservationResponse>(`${this.base}/reservations/${id}`);
  }
}
