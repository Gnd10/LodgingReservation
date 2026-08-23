import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { RoomType } from '../../../shared/models/room.model';
import { ExtraService } from '../../../shared/models/extra-service.model';
import { Promotion, ValidatePromoResponse } from '../../../shared/models/promotion.model';
import { dateRangeValidator, futureOrTodayValidator } from '../../../shared/validators/custom-validators';
import { RupiahPipe } from '../../../shared/pipes/rupiah.pipe';

interface PriceSummary {
  nights: number;
  roomBase: number;
  tierRate: number;
  roomSubtotal: number;
  addOnsTotal: number;
  promoDiscount: number;
  lateCheckoutFee: number;
  grandTotal: number;
}

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RupiahPipe],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(ApiService);

  room?: RoomType;
  extras: ExtraService[] = [];
  promotions: Promotion[] = [];
  roomId?: number;
  loading = true;
  submitting = false;
  promoLoading = false;
  promoResult?: ValidatePromoResponse;
  error = '';
  summary: PriceSummary = { nights: 0, roomBase: 0, tierRate: 0, roomSubtotal: 0, addOnsTotal: 0, promoDiscount: 0, lateCheckoutFee: 0, grandTotal: 0 };

  form = this.fb.group({
    checkIn: ['', [Validators.required, futureOrTodayValidator()]],
    checkOut: ['', Validators.required],
    checkOutTime: ['12:00', Validators.required],
    roomId: [0, [Validators.min(1)]],
    promoCode: [''],
    addOns: this.fb.array([])
  }, { validators: dateRangeValidator() });

  get addOns(): FormArray { return this.form.get('addOns') as FormArray; }

  ngOnInit(): void {
    const roomTypeId = Number(this.route.snapshot.queryParamMap.get('roomTypeId'));
    this.roomId = Number(this.route.snapshot.queryParamMap.get('roomId')) || undefined;
    if (this.roomId) this.form.patchValue({ roomId: this.roomId });

    this.api.getRoomType(roomTypeId).subscribe({
      next: room => {
        this.room = room;
        if (!this.roomId && room.rooms?.length) {
          this.roomId = room.rooms[0].id;
          this.form.patchValue({ roomId: this.roomId });
        }
        this.loading = false;
        this.rebuildAddOns();
        this.recalculate();
      },
      error: err => { this.error = err.error?.message || 'Kamar tidak ditemukan.'; this.loading = false; }
    });

    this.api.getExtraServices().subscribe({ next: data => { this.extras = data; this.rebuildAddOns(); } });
    this.api.getPromotions().subscribe({ next: data => this.promotions = data });

    this.form.valueChanges.subscribe(() => this.recalculate());
  }

  private rebuildAddOns(): void {
    if (!this.extras.length || this.addOns.length) return;
    this.extras.forEach(() => this.addOns.push(this.fb.control(0, [Validators.min(0), Validators.max(99)])));
  }

  syncRoomId(): void { this.roomId = Number(this.form.get('roomId')?.value) || undefined; }

  recalculate(): void {
    if (!this.room) return;

    const checkIn = this.form.get('checkIn')?.value;
    const checkOut = this.form.get('checkOut')?.value;
    const nights = this.getNights(checkIn, checkOut);
    const tierRate = nights >= 7 ? .12 : nights >= 3 ? .05 : 0;
    const roomBase = this.room.basePrice * nights;
    const roomSubtotal = roomBase * (1 - tierRate);

    let addOnsTotal = 0;
    this.extras.forEach((extra, i) => {
      const quantity = Number(this.addOns.at(i)?.value || 0);
      const rawUnit = extra.type ?? extra.unitType ?? '';
      const isNight = rawUnit === 0 || String(rawUnit).toUpperCase() === 'NIGHT';
      addOnsTotal += extra.price * quantity * (isNight ? nights : 1);
    });

    const lateCheckoutFee = this.calculateLateCheckoutFee(this.form.get('checkOutTime')?.value || '12:00');
    const promoDiscount = this.promoResult?.isValid ? this.promoResult.discountAmount : 0;
    const grandTotal = Math.max(0, roomSubtotal + addOnsTotal + lateCheckoutFee - promoDiscount);

    this.summary = { nights, roomBase, tierRate, roomSubtotal, addOnsTotal, promoDiscount, lateCheckoutFee, grandTotal };
  }

  getNights(checkIn?: string | null, checkOut?: string | null): number {
    if (!checkIn || !checkOut) return 0;
    const start = new Date(`${checkIn}T00:00:00`);
    const end = new Date(`${checkOut}T00:00:00`);
    const diff = Math.round((end.getTime() - start.getTime()) / 86400000);
    return diff > 0 ? diff : 0;
  }

  calculateLateCheckoutFee(time: string): number {
    const [h, m] = time.split(':').map(Number);
    const minutes = h * 60 + m - 720;
    if (minutes <= 15) return 0;
    const hours = Math.ceil(minutes / 60);
    return Math.min(hours * 50000, 250000);
  }

  applyPromo(): void {
    const code = (this.form.get('promoCode')?.value || '').trim();
    if (!code) { this.promoResult = undefined; this.recalculate(); return; }
    if (!this.summary.roomSubtotal && !this.summary.addOnsTotal) return;

    this.promoLoading = true;
    this.api.validatePromo(code, this.summary.roomSubtotal + this.summary.addOnsTotal).subscribe({
      next: result => {
        this.promoResult = result;
        this.promoLoading = false;
        this.recalculate();
      },
      error: err => {
        this.promoResult = err.error || { isValid: false, message: 'Kode promo tidak valid.', discountPercentage: 0, discountAmount: 0, finalAmount: this.summary.grandTotal };
        this.promoLoading = false;
        this.recalculate();
      }
    });
  }

  clearPromo(): void {
    this.form.patchValue({ promoCode: '' });
    this.promoResult = undefined;
    this.recalculate();
  }

  submit(): void {
    this.syncRoomId();
    if (this.form.invalid || !this.roomId || this.summary.nights <= 0) {
      this.form.markAllAsTouched();
      if (!this.roomId) this.error = 'Room ID belum tersedia dari backend. Pilih nomor kamar pada detail kamar.';
      return;
    }

    this.submitting = true;
    this.error = '';

    const promotion = this.promotions.find(p => p.promoCode.toLowerCase() === (this.form.get('promoCode')?.value || '').trim().toLowerCase());
    const payload = {
      promotionId: this.promoResult?.isValid ? promotion?.id ?? null : null,
      roomIds: [this.roomId],
      checkInDate: `${this.form.get('checkIn')?.value}T00:00:00`,
      checkOutDate: `${this.form.get('checkOut')?.value}T${this.form.get('checkOutTime')?.value}:00`,
      lateCheckoutFee: this.summary.lateCheckoutFee,
      addOns: this.extras.map((extra, i) => ({
        extraServiceId: extra.id,
        quantity: Number(this.addOns.at(i)?.value || 0)
      })).filter(x => x.quantity > 0)
    };

    this.api.createReservation(payload).subscribe({
      next: reservation => this.router.navigate(['/booking', reservation.id, 'invoice']),
      error: err => { this.submitting = false; this.error = err.error?.message || 'Booking gagal diproses.'; },
      complete: () => this.submitting = false
    });
  }
}
