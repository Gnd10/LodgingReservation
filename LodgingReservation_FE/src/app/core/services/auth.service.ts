import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
} from '../../shared/models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly tokenKey = 'lodging_token';
  private readonly nameKey = 'lodging_user_name';
  private readonly emailKey = 'lodging_user_email';

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(tap((res) => this.saveSession(res)));
  }

  register(userData: RegisterRequest): Observable<RegisterResponse> {
    const payLoad = {
      Name: userData.name,
      Email: userData.email,
      Password: userData.password,
      PhoneNumber: userData.phoneNumber,
    };
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, payLoad);
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => this.saveSession(response))
    );
  }

  private saveSession(res: LoginResponse): void {
    localStorage.setItem('token', res.token);
    localStorage.setItem('user_name', res.name);
    localStorage.setItem('user_email', res.email);
    try {
      const tokenParts = res.token.split('.');
      if (tokenParts.length === 3) {
        const payload = JSON.parse(atob(tokenParts[1]));
        const userId =
          payload[
            'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
          ];
        if (userId) {
          localStorage.setItem('user_id', userId.toString());
        }
      }
    } catch (e) {
      console.error('Gagal mengambil userId dari JWT Token', e);
    }
  saveSession(response: LoginResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.nameKey, response.nama);
    localStorage.setItem(this.emailKey, response.email);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserName(): string | null {
    return localStorage.getItem(this.nameKey);
  }

  getUserEmail(): string | null {
    return localStorage.getItem(this.emailKey);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.nameKey);
    localStorage.removeItem(this.emailKey);
  }
}
