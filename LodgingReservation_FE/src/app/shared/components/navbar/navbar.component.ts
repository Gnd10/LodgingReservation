import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
  auth = inject(AuthService);
  private router = inject(Router);

  isLoggedIn(): boolean {
    return this.auth.isLoggedIn();
  }

  getUserName(): string | null {
    return this.auth.getUserName();
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  showNavbar(): boolean {
    const url = this.router.url;
    return !url.includes('/login') && !url.includes('/register');
  }
}
