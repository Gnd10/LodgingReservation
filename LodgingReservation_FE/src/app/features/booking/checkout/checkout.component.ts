import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { ExtraService } from '../../../shared/models/extra-service.model';
import {
  Promotion,
  ValidatePromoResponse,
} from '../../../shared/models/promotion.model';
import { RoomType } from '../../../shared/models/room.model';
import { RupiahPipe } from '../../../shared/pipes/rupiah.pipe';
import {
  dateRangeValidator,
  futureOrTodayValidator,
} from '../../../shared/validators/custom-validators';

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
  styleUrls: ['./checkout.component.css'],
})
export class CheckoutComponent implements OnInit {
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
  summary: PriceSummary = {
    nights: 0,
    roomBase: 0,
    tierRate: 0,
    roomSubtotal: 0,
    addOnsTotal: 0,
    promoDiscount: 0,
    lateCheckoutFee: 0,
    grandTotal: 0,
  };

  form = this.fb.group(
    {
      checkIn: ['', [Validators.required, futureOrTodayValidator()]],
      checkOut: ['', Validators.required],
      guestCount: [1, [Validators.required, Validators.min(1)]],
      roomId: [null as number | null, Validators.required],
      promoCode: [''],
      addOns: this.fb.array([]),
    },
    { validators: dateRangeValidator() },
  );

  get addOns(): FormArray {
    return this.form.get('addOns') as FormArray;
  }

  ngOnInit(): void {
    const roomTypeId = Number(
      this.route.snapshot.queryParamMap.get('roomTypeId'),
    );

    this.api.getRoomType(roomTypeId).subscribe({
      next: (room) => {
        this.room = room;

        // Batasi dinamis jumlah tamu maksimal sesuai kapasitas tipe kamar
        this.form
          .get('guestCount')
          ?.setValidators([
            Validators.required,
            Validators.min(1),
            Validators.max(room.capacity),
          ]);
        this.form.get('guestCount')?.updateValueAndValidity();

        // Alokasikan ID kamar kosong pertama yang didapat dari database secara otomatis
        if (room.rooms && room.rooms.length > 0) {
          this.roomId = room.rooms[0].id;
          this.form.patchValue({ roomId: this.roomId });
        } else {
          this.error =
            'Tidak ada kamar kosong yang tersedia untuk tipe kamar ini saat ini.';
        }

        this.loading = false;
        this.rebuildAddOns();
        this.recalculate();
      },
      error: (err) => {
        this.error = err.error?.message || 'Kamar tidak ditemukan.';
        this.loading = false;
      },
    });

    this.api.getExtraServices().subscribe({
      next: (data) => {
        // Saring layanan agar mengabaikan "Late Check-out" (karena sudah ditagih offline di lokasi)
        this.extras = data.filter(
          (extra) =>
            !extra.name.toLowerCase().includes('late check-out') &&
            !extra.name.toLowerCase().includes('late checkout'),
        );
        this.rebuildAddOns();
        this.recalculate();
      },
    });

    this.api
      .getPromotions()
      .subscribe({ next: (data) => (this.promotions = data) });

    this.form.valueChanges.subscribe(() => this.recalculate());
  }

  // Cek apakah layanan bertipe kuantitas angka (Kasur Tambahan / Extra Bed)
  isNumberAddon(extra: ExtraService): boolean {
    const name = extra.name.toLowerCase();
    return name.includes('kasur') || name.includes('bed');
  }

  private rebuildAddOns(): void {
    if (!this.extras.length || this.addOns.length) return;
    this.extras.forEach((extra) => {
      if (this.isNumberAddon(extra)) {
        // Input angka kasur dibatasi maksimal 2 unit
        this.addOns.push(
          this.fb.control(0, [Validators.min(0), Validators.max(2)]),
        );
      } else {
        this.addOns.push(this.fb.control(false));
      }
    });
  }

  recalculate(): void {
    if (!this.room) return;

    const checkIn = this.form.get('checkIn')?.value;
    const checkOut = this.form.get('checkOut')?.value;
    const nights = this.getNights(checkIn, checkOut);
    const tierRate = nights >= 7 ? 0.12 : nights >= 3 ? 0.05 : 0;
    const roomBase = this.room.basePrice * nights;
    const roomSubtotal = roomBase * (1 - tierRate);

    let addOnsTotal = 0;
    const guestCount = Number(this.form.get('guestCount')?.value || 1);

    this.extras.forEach((extra, i) => {
      const value = this.addOns.at(i)?.value;
      let quantity = 0;

      if (this.isNumberAddon(extra)) {
        quantity = Number(value || 0);
      } else {
        const isChecked = !!value;
        const rawUnit = extra.type ?? extra.unitType ?? '';
        const isPerson =
          rawUnit === 1 || String(rawUnit).toUpperCase() === 'PERSON';
        // Jumlah sarapan otomatis mengikuti jumlah tamu, add-ons lain bernilai 1 jika dicentang
        quantity = isChecked ? (isPerson ? guestCount : 1) : 0;
      }

      const rawUnit = extra.type ?? extra.unitType ?? '';
      const isNight =
        rawUnit === 0 || String(rawUnit).toUpperCase() === 'NIGHT';
      addOnsTotal += extra.price * quantity * (isNight ? nights : 1);
    });

    const lateCheckoutFee = 0; // Selalu 0 di awal (Skenario 2)
    const promoDiscount = this.promoResult?.isValid
      ? this.promoResult.discountAmount
      : 0;
    const grandTotal = Math.max(
      0,
      roomSubtotal + addOnsTotal + lateCheckoutFee - promoDiscount,
    );

    this.summary = {
      nights,
      roomBase,
      tierRate,
      roomSubtotal,
      addOnsTotal,
      promoDiscount,
      lateCheckoutFee,
      grandTotal,
    };
  }

  getNights(checkIn?: string | null, checkOut?: string | null): number {
    if (!checkIn || !checkOut) return 0;
    const start = new Date(`${checkIn}T00:00:00`);
    const end = new Date(`${checkOut}T00:00:00`);
    const diff = Math.round((end.getTime() - start.getTime()) / 86400000);
    return diff > 0 ? diff : 0;
  }

  applyPromo(): void {
    const code = (this.form.get('promoCode')?.value || '').trim();
    if (!code) {
      this.promoResult = undefined;
      this.recalculate();
      return;
    }
    if (!this.summary.roomSubtotal && !this.summary.addOnsTotal) return;

    this.promoLoading = true;
    this.api
      .validatePromo(code, this.summary.roomSubtotal + this.summary.addOnsTotal)
      .subscribe({
        next: (result) => {
          this.promoResult = result;
          this.promoLoading = false;
          this.recalculate();
        },
        error: (err) => {
          this.promoResult = err.error || {
            isValid: false,
            message: 'Kode promo tidak valid.',
            discountPercentage: 0,
            discountAmount: 0,
            finalAmount: this.summary.grandTotal,
          };
          this.promoLoading = false;
          this.recalculate();
        },
      });
  }

  clearPromo(): void {
    this.form.patchValue({ promoCode: '' });
    this.promoResult = undefined;
    this.recalculate();
  }

  submit(): void {
    if (this.form.invalid || !this.roomId || this.summary.nights <= 0) {
      this.form.markAllAsTouched();
      if (!this.roomId)
        this.error = 'Tidak ada kamar kosong yang bisa dialokasikan.';
      return;
    }

    this.submitting = true;
    this.error = '';

    const promotion = this.promotions.find(
      (p) =>
        p.promoCode.toLowerCase() ===
        (this.form.get('promoCode')?.value || '').trim().toLowerCase(),
    );
    const guestCount = Number(this.form.get('guestCount')?.value || 1);

    const payload = {
      promotionId: this.promoResult?.isValid ? (promotion?.id ?? null) : null,
      roomIds: [this.roomId],
      checkInDate: `${this.form.get('checkIn')?.value}T00:00:00`,
      // Paksa jam check-out ke jam 12:00 siang standar agar backend menghitung denda = 0
      checkOutDate: `${this.form.get('checkOut')?.value}T12:00:00`,
      lateCheckoutFee: 0,
      addOns: this.extras
        .map((extra, i) => {
          const value = this.addOns.at(i)?.value;
          let quantity = 0;
          if (this.isNumberAddon(extra)) {
            quantity = Number(value || 0);
          } else {
            const isChecked = !!value;
            const rawUnit = extra.type ?? extra.unitType ?? '';
            const isPerson =
              rawUnit === 1 || String(rawUnit).toUpperCase() === 'PERSON';
            quantity = isChecked ? (isPerson ? guestCount : 1) : 0;
          }
          return {
            extraServiceId: extra.id,
            quantity: quantity,
          };
        })
        .filter((x) => x.quantity > 0),
    };

    this.api.createReservation(payload).subscribe({
      next: (reservation) =>
        this.router.navigate(['/booking', reservation.id, 'invoice']),
      error: (err) => {
        this.submitting = false;
        this.error = err.error?.message || 'Booking gagal diproses.';
      },
      complete: () => (this.submitting = false),
    });
  }
}
