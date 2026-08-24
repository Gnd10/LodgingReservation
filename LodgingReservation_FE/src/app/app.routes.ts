import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { BookingHistoryComponent } from './features/booking-history/booking-history.component';
import { CheckoutComponent } from './features/booking/checkout/checkout.component';
import { InvoiceComponent } from './features/booking/invoice/invoice.component';
import { CatalogComponent } from './features/catalog/catalog.component';
import { ProfileComponent } from './features/profile/profile.component';
import { RoomDetailComponent } from './features/rooms/room-detail/room-detail.component';
import { RoomListComponent } from './features/rooms/room-list/room-list.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Rute Terproteksi (Wajib Login)
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'catalog', pathMatch: 'full' },

      { path: 'catalog', component: CatalogComponent },
      { path: 'catalog/:id', component: RoomDetailComponent },
      { path: 'rooms', component: RoomListComponent },
      { path: 'rooms/:id', component: RoomDetailComponent },

      { path: 'booking', component: CheckoutComponent },
      {
        path: 'booking/:id',
        children: [{ path: 'invoice', component: InvoiceComponent }],
      },

      { path: 'profile', component: ProfileComponent },
      { path: 'bookings', component: BookingHistoryComponent },
    ],
  },

  { path: '**', redirectTo: 'login' },
];
