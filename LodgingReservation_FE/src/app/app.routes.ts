import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { RoomListComponent } from './features/rooms/room-list/room-list.component';
import { RoomDetailComponent } from './features/rooms/room-detail/room-detail.component';
import { CheckoutComponent } from './features/booking/checkout/checkout.component';
import { InvoiceComponent } from './features/booking/invoice/invoice.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { BookingHistoryComponent } from './features/booking-history/booking-history.component';
import { CatalogComponent } from './features/catalog/catalog.component';
import { ProfileComponent } from './features/profile/profile.component';

export const routes: Routes = [
  { path: '', redirectTo: 'rooms', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: 'rooms', component: RoomListComponent },
      { path: 'rooms/:id', component: RoomDetailComponent },
      {
        path: 'booking/:id',
        children: [
          { path: 'invoice', component: InvoiceComponent }
        ]
      },
      { path: 'booking', component: CheckoutComponent }
    ]
  },
  { path: '**', redirectTo: 'rooms' },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'catalog', component: CatalogComponent, canActivate: [authGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
  { path: 'bookings', component: BookingHistoryComponent, canActivate: [authGuard]}
];

