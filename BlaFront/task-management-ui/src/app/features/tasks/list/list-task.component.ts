import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { Task, TaskStatus } from '../../../core/models/task.model';
import { TaskService } from '../../../core/services/task.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule, NgClass, NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HistoryModalComponent } from '../../../shared/components/history-modal/history-modal';
import { TaskHistory } from '../../../core/models/task-history.model';

@Component({
  selector: 'app-list-task.component',
  standalone: true,
  imports: [CommonModule, RouterLink, HistoryModalComponent, NgClass, NgFor, NgIf],
  templateUrl: './list-task.component.html',
  styleUrl: './list-task.component.scss',
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  selectedTaskId: string | null = null;
  history: TaskHistory[] = [];
  showHistoryModal = false;
  taskStatus = TaskStatus;
  constructor(
    private readonly taskService: TaskService,
    private readonly notificationService: NotificationService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe({
      next: (tasks: Task[]) => {
        this.tasks = tasks;

        this.cdr.detectChanges();
      },

      error: (error: HttpErrorResponse) => {
        this.notificationService.error(this.getErrorMessage(error));

        this.cdr.detectChanges();
      },
    });
  }

  openHistory(taskId: string): void {
    this.selectedTaskId = taskId;

    this.taskService.getHistory(taskId).subscribe({
      next: (history) => {
        this.history = history;
        this.showHistoryModal = true;

        this.cdr.detectChanges();
      },
      error: (error: HttpErrorResponse) => {
        this.notificationService.error(error.error?.message ?? 'Unable to load task history.');

        this.cdr.detectChanges();
      },
    });
  }

  closeHistory(): void {
    this.showHistoryModal = false;
    this.selectedTaskId = null;
    this.history = [];

    this.cdr.detectChanges();
  }

  canStart(task: Task): boolean {
    return task.status === 'Pending';
  }

  canComplete(task: Task): boolean {
    return task.status === 'InProgress';
  }

  canCancel(task: Task): boolean {
    return task.status === 'Pending' || task.status === 'InProgress';
  }

  canEdit(task: Task): boolean {
    return task.status === 'Pending';
  }

  canDelete(task: Task): boolean {
    return task.status === 'Pending';
  }

  start(task: Task): void {
    this.taskService.startTask(task.id).subscribe({
      next: () => {
        this.notificationService.success('Task started successfully.');

        this.loadTasks();
      },

      error: (error: HttpErrorResponse) => {
        this.notificationService.error(this.getErrorMessage(error));

        this.cdr.detectChanges();
      },
    });
  }

  complete(task: Task): void {
    this.taskService.completeTask(task.id).subscribe({
      next: () => {
        this.notificationService.success('Task completed successfully.');

        this.loadTasks();
      },

      error: (error: HttpErrorResponse) => {
        this.notificationService.error(this.getErrorMessage(error));

        this.cdr.detectChanges();
      },
    });
  }

  async cancel(task: Task): Promise<void> {
    const result = await this.notificationService.confirm(
      'Cancel task?',
      'This action cannot be undone.',
    );

    if (!result.isConfirmed) {
      return;
    }

    this.taskService.cancelTask(task.id).subscribe({
      next: () => {
        this.notificationService.success('Task cancelled successfully.');

        this.loadTasks();
      },

      error: (error: HttpErrorResponse) => {
        this.notificationService.error(this.getErrorMessage(error));

        this.cdr.detectChanges();
      },
    });
  }

  async delete(task: Task): Promise<void> {
    const result = await this.notificationService.confirm(
      'Delete task?',
      'This action cannot be undone.',
    );

    if (!result.isConfirmed) {
      return;
    }

    this.taskService.deleteTask(task.id).subscribe({
      next: () => {
        this.notificationService.success('Task deleted successfully.');

        this.loadTasks();
      },

      error: (error: HttpErrorResponse) => {
        this.notificationService.error(this.getErrorMessage(error));

        this.cdr.detectChanges();
      },
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Pending':
        return 'bg-warning text-dark';

      case 'InProgress':
        return 'bg-primary';

      case 'Completed':
        return 'bg-success';

      case 'Cancelled':
        return 'bg-danger';

      default:
        return 'bg-secondary';
    }
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    if (typeof error.error === 'string') {
      return error.error;
    }

    return error.error?.message ?? 'An unexpected error occurred.';
  }
}
