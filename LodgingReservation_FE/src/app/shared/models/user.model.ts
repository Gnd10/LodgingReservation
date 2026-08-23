export interface UserProfile {
  id: number;
  name: string;
  email: string;
  phoneNumber?: string;
}

export interface UpdateProfileRequest {
  Name: string;
  PhoneNumber?: string;
}
