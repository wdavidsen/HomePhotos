import { Routes } from '@angular/router';
import { AccountComponent } from './account/account.component';
import { NotFoundComponent } from './errors/not-found.component';
import { UnauthorizedComponent } from './errors/unauthorized.component';
import { LoginComponent } from './login/login.component';
import { LogsComponent } from './logs/logs.component';
import { PhotosComponent } from './photos/photos.component';
import { RegisterSuccessComponent } from './register/register-success.component';
import { RegisterComponent } from './register/register.component';
import { SettingsComponent } from './settings/settings.component';
import { TagsComponent } from './tags/tags.component';
import { UploadComponent } from './upload/upload.component';
import { UserDetailComponent } from './users/user-detail.component';
import { UsersComponent } from './users/users.component';

export const routes: Routes = [    
    { path: '', component: PhotosComponent, pathMatch: 'full' },
    { path: 'photos', component: PhotosComponent },
    { path: 'photos/:tagName', component: PhotosComponent },
    { path: 'tags', component: TagsComponent },
    { path: 'settings', component: SettingsComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'register-success', component: RegisterSuccessComponent },
    { path: 'users', component: UsersComponent },
    { path: 'users/:userId', component: UserDetailComponent },
    { path: 'account', component: AccountComponent },
    { path: 'logs', component: LogsComponent },
    { path: 'upload', component: UploadComponent },
    { path: 'unauthorized', component: UnauthorizedComponent },
    { path: 'notfound', component: NotFoundComponent },

    // 404
    { path: '**', redirectTo: '/notfound' }
];
