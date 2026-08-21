export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  role: 'Admin' | 'Customer';
  fullName: string;
  userId: number;
}

export interface AuthState {
  token: string | null;
  role: 'Admin' | 'Customer' | null;
  fullName: string | null;
  userId: number | null;
  isAuthenticated: boolean;
}