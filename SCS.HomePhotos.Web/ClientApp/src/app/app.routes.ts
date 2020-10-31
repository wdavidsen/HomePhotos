import { Routes } from '@angular/router';
import { AccountComponent } from './account/account.component';
import { AuthGuard } from './pipeline';
import { LoginComponent } from './login/login.component';
import { LogsComponent } from './logs/logs.component';
import { PhotosComponent } from './photos/photos.component';
import { RegisterComponent } from './register/register.component';
import { SettingsComponent } from './settings/settings.component';
import { TagsComponent } from './tags/tags.component';
import { UploadComponent } from './upload/upload.component';
import { UserDetailComponent } from './users/user-detail.component';
import { UsersComponent } from './users/users.component';

export const routes: Routes = [
    { path: '', component: PhotosComponent, pathMatch: 'full', canActivate: [AuthGuard] },
    { path: 'photos/:tagName', component: PhotosComponent, canActivate: [AuthGuard] },
    { path: 'tags', component: TagsComponent, canActivate: [AuthGuard] },
    { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'users', component: UsersComponent, canActivate: [AuthGuard] },
    { path: 'users/:userId', component: UserDetailComponent, canActivate: [AuthGuard] },
    { path: 'account', component: AccountComponent, canActivate: [AuthGuard] },
    { path: 'logs', component: LogsComponent, canActivate: [AuthGuard] },
    { path: 'upload', component: UploadComponent, canActivate: [AuthGuard] },

    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];
