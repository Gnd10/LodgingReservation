import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  profileForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    email: [{ value: '', disabled: true }],
    phoneNumber: ['', [Validators.pattern(/^[0-9]+$/)]],
  });

  isLoading = true;
  isSaving = false;
  isDeleting = false;
  successMessage = '';
  errorMessage = '';

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.userService.getProfile().subscribe({
      next: (profile) => {
        this.profileForm.patchValue({
          name: profile.name,
          email: profile.email,
          phoneNumber: profile.phoneNumber,
        });
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Gagal memuat profil Anda.';
        this.cdr.detectChanges();
      },
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.successMessage = '';
    this.errorMessage = '';

    const payload = {
      Name: this.profileForm.getRawValue().name,
      PhoneNumber: this.profileForm.getRawValue().phoneNumber,
    };

    this.userService.updateProfile(payload).subscribe({
      next: (updatedProfile) => {
        this.isSaving = false;
        this.successMessage = 'Berhasil diperbarui!';
        // Update user_name di localStorage agar navbar ikut berubah
        localStorage.setItem('user_name', updatedProfile.name);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = err.error?.message || 'Gagal memperbarui.';
        this.cdr.detectChanges();
      },
    });
  }

  onDeleteAccount(): void {
    const confirmDelete = confirm(
      'Apakah Anda yakin ingin menonaktifkan akun Anda?',
    );
    if (!confirmDelete) return;

    this.isDeleting = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.userService.deleteAccount().subscribe({
      next: () => {
        this.isDeleting = false;
        alert('Akun Anda berhasil dinonaktifkan.');
        this.authService.logout();
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isDeleting = false;
        this.errorMessage = err.error?.message || 'Gagal menghapus akun.';
        this.cdr.detectChanges();
      },
    });
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
