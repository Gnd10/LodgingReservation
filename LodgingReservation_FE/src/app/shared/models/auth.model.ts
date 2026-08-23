export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  phoneNumber?: string;
}

export interface LoginResponse {
  token: string;
  name: string;
  email: string;
}

export interface RegisterResponse {
  token: string;
  name: string;
  email: string;
}

export interface AuthState {
  token: string | null;
  name: string | null;
  userId: number | null;
  isAuthenticated: boolean;
  nama: string;
  email: string;
}

