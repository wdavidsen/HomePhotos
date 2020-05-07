import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { PhotosComponent } from './photos/photos.component';
import { TagsComponent } from './tags/tags.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { SettingsComponent } from './settings/settings.component';
import { AlertComponent, OrganizeComponent } from './components';
import { JwtInterceptor, ErrorInterceptor, AuthGuard } from './helpers';
import { SettingsService } from './services/settings.service';
import { AccountComponent } from './account/account.component';
import { PhotosService, TagService, PageInfoService } from './services';
import { PageInfoComponent } from './components/page-info.component';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { SearchComponent } from './components/search.component';
import { UsersComponent } from './users/users.component';
import { UserDetailComponent } from './users/user-detail.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule, BsModalRef } from 'ngx-bootstrap/modal';
import { AlertModule } from 'ngx-bootstrap/alert';
import { ToastrModule } from 'ngx-toastr';
import { ModalContentComponent } from './users/change-password-modal.component';
import { PhotoTaggerComponent } from './photos/photo-tagger.component';
import { TriCheckComponent } from './components/tri-check.component';
import { LogService } from './services/log.service';
import { LogsComponent } from './logs/logs.component';
import { UploadComponent } from './upload/upload.component';
import { FileUploadModule } from 'ng2-file-upload';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PhotosComponent,
    TagsComponent,
    LoginComponent,
    RegisterComponent,
    SettingsComponent,
    UsersComponent,
    UserDetailComponent,
    AlertComponent,
    PageInfoComponent,
    OrganizeComponent,
    SearchComponent,
    AccountComponent,
    ModalContentComponent,
    PhotoTaggerComponent,
    TriCheckComponent,
    LogsComponent,
    UploadComponent
  ],
  entryComponents: [
    ModalContentComponent,
    PhotoTaggerComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AngularFontAwesomeModule,
    RouterModule.forRoot([
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
    ]),
    BrowserAnimationsModule,
    TypeaheadModule.forRoot(),
    PaginationModule.forRoot(),
    ModalModule.forRoot(),
    AlertModule.forRoot(),
    ModalModule.forRoot(),
    TypeaheadModule.forRoot(),
    ToastrModule.forRoot({
      timeOut: 4000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true
    }),
    FileUploadModule
  ],
  providers: [
    SettingsService,
    PhotosService,
    BsModalRef,
    TagService,
    LogService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
