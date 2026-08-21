import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { delay, tap } from 'rxjs/operators';
import { LoginRequest, LoginResponse } from '../../shared/models/auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  
  login(credentials: LoginRequest): Observable<LoginResponse> {
    const { email, password } = credentials;

    if (email === 'admin@stayhub.com' && password === 'password123') {
      const mockAdmin: LoginResponse = {
        token: 'mock-jwt-token-admin-12345',
        role: 'Admin',
        fullName: 'Admin StayHub',
        userId: 1,
      };
      return of(mockAdmin).pipe(
        delay(800),
        tap((res) => this.saveSession(res)),
      );
    }

    if (email === 'customer@stayhub.com' && password === 'password123') {
      const mockCustomer: LoginResponse = {
        token: 'mock-jwt-token-customer-67890',
        role: 'Customer',
        fullName: 'Budi Santoso',
        userId: 2,
      };
      return of(mockCustomer).pipe(
        delay(800),
        tap((res) => this.saveSession(res)),
      );
    }

    return throwError(() => ({
      error: {
        message:
          'Email atau password salah (Gunakan admin@stayhub.com / customer@stayhub.com & password123)',
      },
    })).pipe(delay(800));
  }

  private saveSession(res: LoginResponse): void {
    localStorage.setItem('token', res.token);
    localStorage.setItem('user_role', res.role);
    localStorage.setItem('user_name', res.fullName);
    localStorage.setItem('user_id', res.userId.toString());
  }

  logout(): void {
    localStorage.clear();
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getRole(): 'Admin' | 'Customer' | null {
    return localStorage.getItem('user_role') as 'Admin' | 'Customer' | null;
  }

  getUserName(): string | null {
    return localStorage.getItem('user_name');
  }
}
