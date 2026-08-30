import { Injectable } from '@angular/core';
import Swal, { SweetAlertResult } from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  success(message: string): void {
    Swal.fire({
      icon: 'success',
      title: 'Success',
      text: message,
      confirmButtonText: 'OK',
    });
  }

  error(message: string): void {
    Swal.fire({
      icon: 'error',
      title: 'Error',
      text: message,
      confirmButtonText: 'OK',
    });
  }

  warning(message: string): void {
    Swal.fire({
      icon: 'warning',
      title: 'Warning',
      text: message,
      confirmButtonText: 'OK',
    });
  }

  async confirm(title: string, text: string): Promise<SweetAlertResult> {
    return Swal.fire({
      icon: 'warning',
      title,
      text,
      showCancelButton: true,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No',
    });
  }
}
