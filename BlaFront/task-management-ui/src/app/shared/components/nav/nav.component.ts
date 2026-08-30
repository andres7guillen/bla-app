import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-nav',
  imports: [],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.scss',
})
export class NavComponent {
  constructor(
    public readonly authService: AuthService,
    private readonly router: Router,
    private readonly notificationService: NotificationService,
  ) {}

  logout(): void {
    this.authService.logout();

    this.notificationService.success('You have been logged out successfully.');

    this.router.navigate(['/login']);
  }
}
