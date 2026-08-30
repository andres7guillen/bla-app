import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NotificationService } from '../../../shared/services/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-login.component',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);
  loading = false;
  form = this.fb.nonNullable.group({
    email: ['andres7guillen@gmail.com', [Validators.required, Validators.email]],
    password: [
      'Y0k0gawA_1992',
      [Validators.required, Validators.minLength(6), Validators.maxLength(100)],
    ],
  });
  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { email, password } = this.form.getRawValue();
    this.loading = true;
    this.authService.login({ email, password }).subscribe({
      next: () => {
        this.loading = false;
        this.notificationService.success('Login successful.');
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
    return error.error?.message ?? error.error?.error ?? 'Invalid email or password.';
  }
}
