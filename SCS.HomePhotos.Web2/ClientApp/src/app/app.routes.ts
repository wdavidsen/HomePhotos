import { Routes } from '@angular/router';

export const routes: Routes = [
    //{ path: '', component: PhotosComponent, pathMatch: 'full', canActivate: [AuthGuard] },
    // { path: 'photos', component: PhotosComponent, canActivate: [AuthGuard] },
    // { path: 'photos/:tagName', component: PhotosComponent, canActivate: [AuthGuard] },
    // { path: 'tags', component: TagsComponent, canActivate: [AuthGuard] },
    // { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard, AuthAdminGuard] },
    //{ path: 'login', component: LoginComponent },
    //{ path: 'register', component: RegisterComponent },
    // { path: 'register-success', component: RegisterSuccessComponent },
    // { path: 'users', component: UsersComponent, canActivate: [AuthGuard, AuthAdminGuard] },
    // { path: 'users/:userId', component: UserDetailComponent, canActivate: [AuthGuard, AuthAdminGuard] },
    // { path: 'account', component: AccountComponent, canActivate: [AuthGuard] },
    // { path: 'logs', component: LogsComponent, canActivate: [AuthGuard, AuthAdminGuard] },
    // { path: 'upload', component: UploadComponent, canActivate: [AuthGuard, AuthUploadGuard] },
    // { path: 'unauthorized', component: UnauthorizedComponent },
    // { path: 'notfound', component: NotFoundComponent },

    // 404
    { path: '**', redirectTo: '/notfound' }
];
