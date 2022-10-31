import { Routes } from '@angular/router';
import { AccountComponent } from './account/account.component';
import { NotFoundComponent } from './errors/not-found.component';
import { UnauthorizedComponent } from './errors/unauthorized.component';
import { AuthGuard } from './framework';
import { AuthAdminGuard } from './framework/auth-admin.guard';
import { AuthUploadGuard } from './framework/auth-upload.guard';
import { LoginComponent } from './login/login.component';
import { LogsComponent } from './logs/logs.component';
import { PhotosComponent } from './photos/photos.component';
import { Photos2Component } from './photos/photos2.component';
import { RegisterSuccessComponent } from './register/register-success.component';
import { RegisterComponent } from './register/register.component';
import { SettingsComponent } from './settings/settings.component';
import { TagsComponent } from './tags/tags.component';
import { UploadComponent } from './upload/upload.component';
import { UserDetailComponent } from './users/user-detail.component';
import { UsersComponent } from './users/users.component';

export const routes: Routes = [    
  { path: '', component: PhotosComponent, pathMatch: 'full', canActivate: [AuthGuard] },
  { path: 'photos', component: PhotosComponent, canActivate: [AuthGuard] },
  { path: 'photos2', component: Photos2Component, canActivate: [AuthGuard] },
  { path: 'photos/:tagName', component: PhotosComponent, canActivate: [AuthGuard] },
  { path: 'tags', component: TagsComponent, canActivate: [AuthGuard] },
  { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard, AuthAdminGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'register-success', component: RegisterSuccessComponent },
  { path: 'users', component: UsersComponent, canActivate: [AuthGuard, AuthAdminGuard] },
  { path: 'users/:userId', component: UserDetailComponent, canActivate: [AuthGuard, AuthAdminGuard] },
  { path: 'account', component: AccountComponent, canActivate: [AuthGuard] },
  { path: 'logs', component: LogsComponent, canActivate: [AuthGuard, AuthAdminGuard] },
  { path: 'upload', component: UploadComponent, canActivate: [AuthGuard, AuthUploadGuard] },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: 'notfound', component: NotFoundComponent },

  // 404
  { path: '**', redirectTo: '/notfound' }
];
