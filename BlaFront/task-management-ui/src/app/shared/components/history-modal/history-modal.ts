import { ChangeDetectorRef, Component, Input } from '@angular/core';
import { TaskHistory } from '../../../core/models/task-history.model';
import { TaskService } from '../../../core/services/task.service';
import { NotificationService } from '../../services/notification.service';
import { HttpErrorResponse } from '@angular/common/http';
import { DatePipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-history-modal',
  standalone: true,
  imports: [DatePipe, NgFor, NgIf],
  templateUrl: './history-modal.html',
  styleUrl: './history-modal.scss',
})
export class HistoryModalComponent {
  private _taskId: string | null = null;

  @Input()
  set taskId(value: string | null) {
    this._taskId = value;

    if (value) {
      this.loadHistory();
    }
  }

  get taskId(): string | null {
    return this._taskId;
  }

  history: TaskHistory[] = [];

  loading = false;

  constructor(
    private readonly taskService: TaskService,
    private readonly notificationService: NotificationService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  private loadHistory(): void {
    if (!this._taskId) {
      return;
    }

    this.loading = true;

    this.history = [];

    this.cdr.detectChanges();

    this.taskService.getHistory(this._taskId).subscribe({
      next: (history: TaskHistory[]) => {
        this.history = history;

        this.loading = false;

        this.cdr.detectChanges();
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

    return error.error?.message ?? 'Unable to load task history.';
  }
}
