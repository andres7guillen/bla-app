import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TaskHistory } from '../models/task-history.model';
import { Task } from '../models/task.model';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private readonly apiUrl = 'https://localhost:44363/api/Tasks';

  constructor(private readonly http: HttpClient) {}

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl);
  }

  getTaskById(taskId: string): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${taskId}`);
  }

  createTask(task: unknown): Observable<string> {
    return this.http.post<string>(this.apiUrl, task);
  }

  updateTask(taskId: string, task: unknown): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${taskId}`, task);
  }

  deleteTask(taskId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${taskId}`);
  }

  startTask(taskId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${taskId}/start`, {});
  }

  completeTask(taskId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${taskId}/complete`, {});
  }

  cancelTask(taskId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${taskId}/cancel`, {});
  }

  getHistory(taskId: string): Observable<TaskHistory[]> {
    debugger;
    return this.http.get<TaskHistory[]>(`${this.apiUrl}/${taskId}/history`);
  }
}
