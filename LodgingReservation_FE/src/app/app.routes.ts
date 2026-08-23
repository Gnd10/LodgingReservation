import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { authGuard } from './core/guards/auth.guard';
import { RoomListComponent } from './features/rooms/room-list/room-list.component';
import { RoomDetailComponent } from './features/rooms/room-detail/room-detail.component';
import { CheckoutComponent } from './features/booking/checkout/checkout.component';
import { InvoiceComponent } from './features/booking/invoice/invoice.component';

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
  { path: '**', redirectTo: 'rooms' }
];
