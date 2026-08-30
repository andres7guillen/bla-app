import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NotificationService } from '../../../shared/services/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-register.component',
  imports: [ReactiveFormsModule, RouterLink, NgIf, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);
  loading = false;
  form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
    confirmPassword: ['', [Validators.required]],
  });
  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { firstName, lastName, email, password, confirmPassword } = this.form.getRawValue();
    if (password !== confirmPassword) {
      this.notificationService.error('Passwords do not match.');
      return;
    }
    this.loading = true;
    this.authService.register({ firstName, lastName, email, password }).subscribe({
      next: () => {
        this.loading = false;
        this.notificationService.success('User registered successfully.');
        this.cdr.detectChanges();
        this.router.navigate(['/tasks']);
      },
      error: (error: HttpErrorResponse) => {
        this.loading = false;
        this.notificationService.error(this.getErrorMessage(error));
        this.cdr.detectChanges();
      },
    });
  }
  private getErrorMessage(error: HttpErrorResponse): string {
    if (typeof error.error === 'string') {
      return error.error;
    }
    return error.error?.message ?? error.error?.error ?? 'Unable to register user.';
  }
}
