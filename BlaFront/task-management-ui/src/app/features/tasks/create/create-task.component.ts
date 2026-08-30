import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { Router } from '@angular/router';
import { NotificationService } from '../../../shared/services/notification.service';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-task.component',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create-task.component.html',
  styleUrl: './create-task.component.scss',
})
export class CreateTaskComponent {
  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);
  loading = false;
  form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(1000)]],
    dueDate: ['', [Validators.required]],
  });
  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { title, description, dueDate } = this.form.getRawValue();
    this.loading = true;
    this.taskService.createTask({ title, description, dueDate }).subscribe({
      next: () => {
        this.loading = false;
        this.notificationService.success('Task created successfully.');
        this.cdr.detectChanges();
        this.router.navigate(['/tasks']);
      },
      error: (error: HttpErrorResponse) => {
        this.loading = false;
        this.notificationService.error(error.error?.message ?? 'Unable to create task.');
        this.cdr.detectChanges();
      },
    });
  }
}
