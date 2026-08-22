import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../../shared/models/auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/auth`;

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((res) => this.saveSession(res))
    );
  }

  private saveSession(res: LoginResponse): void {
    localStorage.setItem('token', res.token);
    localStorage.setItem('user_name', res.fullName);
    localStorage.setItem('user_id', res.userId.toString());
  }

  logout(): void {
    localStorage.clear();
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getUserName(): string | null {
    return localStorage.getItem('user_name');
  }
}
