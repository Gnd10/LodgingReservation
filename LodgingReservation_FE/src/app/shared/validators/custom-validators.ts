import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function strongPasswordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) return null;
    const valid = /[A-Z]/.test(value) && /[a-z]/.test(value) && /[0-9]/.test(value);
    return valid ? null : { passwordStrength: true };
  };
}

export function dateRangeValidator(checkInKey = 'checkIn', checkOutKey = 'checkOut'): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const checkIn = group.get(checkInKey)?.value;
    const checkOut = group.get(checkOutKey)?.value;
    if (!checkIn || !checkOut) return null;

    const start = new Date(`${checkIn}T00:00:00`);
    const end = new Date(`${checkOut}T00:00:00`);
    return end > start ? null : { invalidDateRange: true };
  };
}

export function futureOrTodayValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const value = new Date(`${control.value}T00:00:00`);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return value >= today ? null : { pastDate: true };
  };
}
