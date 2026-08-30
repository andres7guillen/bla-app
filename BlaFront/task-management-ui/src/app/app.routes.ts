import { Routes } from '@angular/router';
import { EditTaskComponent } from './features/tasks/edit/edit-task.component';
import { DetailTaskComponent } from './features/tasks/detail/detail-task.component';
import { CreateTaskComponent } from './features/tasks/create/create-task.component';
import { TaskListComponent } from './features/tasks/list/list-task.component';
import { authGuard } from './core/guards/auth.guard';
import { RegisterComponent } from './features/auth/register/register.component';
import { LoginComponent } from './features/auth/login/login.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },

  {
    path: 'login',
    component: LoginComponent,
  },

  {
    path: 'register',
    component: RegisterComponent,
  },

  {
    path: 'tasks',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        component: TaskListComponent,
      },

      {
        path: 'create',
        component: CreateTaskComponent,
      },

      {
        path: ':id',
        component: DetailTaskComponent,
      },

      {
        path: ':id/edit',
        component: EditTaskComponent,
      },
    ],
  },

  {
    path: '**',
    redirectTo: 'login',
  },
];
