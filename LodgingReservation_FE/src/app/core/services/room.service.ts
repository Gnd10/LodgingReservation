import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RoomType } from '../../shared/models/room.model';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/room-types`;

  getRoomTypes(): Observable<RoomType[]> {
    return this.http.get<RoomType[]>(this.apiUrl);
  }

  getRoomtypeDetails(id: number): Observable<RoomType> {
    return this.http.get<RoomType>(`${this.apiUrl}/${id}`);
  }
}