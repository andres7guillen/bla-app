import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthResponse } from '../models/auth-response.model';
import { HttpClient } from '@angular/common/http';
import { RegisterRequest } from '../models/register-request.model';
import { LoginResponse } from '../models/login-response.model';
import { LoginRequest } from '../models/login-request.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:44363/api/auth';

  private readonly tokenKey = 'access_token';

  constructor(private readonly http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
      }),
    );
  }

  register(request: RegisterRequest): Observable<unknown> {
    return this.http.post(`${this.apiUrl}/register`, request);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }
}
