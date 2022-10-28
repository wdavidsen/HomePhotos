// angular
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// 3rd party
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule, BsModalRef } from 'ngx-bootstrap/modal';
import { AlertModule } from 'ngx-bootstrap/alert';
import { ToastrModule } from 'ngx-toastr';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { AngularCropperjsModule } from 'angular-cropperjs';
import { FileUploadModule } from 'ng2-file-upload';

// local
import { AppComponent } from './app.component';
import { NavMenuComponent } from './main-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { PhotosComponent } from './photos/photos.component';
import { TagsComponent } from './tags/tags.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { SettingsComponent } from './settings/settings.component';
import { AlertComponent, OrganizeComponent } from './components';
import { SettingsService } from './services/settings.service';
import { AccountComponent } from './account/account.component';
import { PhotosService, TagService, PageInfoService, UserSettingsService, ImageService } from './services';
import { PageInfoComponent } from './components/page-info.component';
import { SearchComponent } from './components/search.component';
import { UsersComponent } from './users/users.component';
import { UserDetailComponent } from './users/user-detail.component';
import { ChangePasswordModalComponent } from './account/change-password-modal.component';
import { ResetPasswordModalComponent } from './users/reset-password-modal.component';
import { PhotoTaggerComponent } from './photos/photo-tagger.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { TriCheckComponent } from './components/tri-check.component';
import { LogService } from './services/log.service';
import { LogsComponent } from './logs/logs.component';
import { UploadComponent } from './upload/upload.component';
import { UploadPhotoTaggerComponent } from './upload/upload-photo-tagger.component';
import { InputDialogComponent, ConfirmDialogComponent, AlertDialogComponent } from './common-dialog';
import { LocalStorageService } from './services/local-storage.service';
import { TokenInterceptor } from './framework';
import { CsrfHeaderInterceptor } from './framework/csrf-header.interceptor';
import { UnauthorizedComponent } from './errors/unauthorized.component';
import { NotFoundComponent } from './errors/not-found.component';
import { ProgressInterceptor } from './framework/progress.interceptor';
import { RegisterSuccessComponent } from './register/register-success.component';
import { SecurePipe } from './framework/secure.pipe';
import { AccountAvatardModalComponent } from './account/account-avatar-modal.component';

import { routes } from './app.routes';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PhotosComponent,
    TagsComponent,
    LoginComponent,
    RegisterComponent,
    RegisterSuccessComponent,
    SettingsComponent,
    UsersComponent,
    UserDetailComponent,
    AlertComponent,
    PageInfoComponent,
    OrganizeComponent,
    SearchComponent,
    AccountComponent,
    ChangePasswordModalComponent,
    AccountAvatardModalComponent,
    ResetPasswordModalComponent,
    PhotoTaggerComponent,
    UserSettingsComponent,
    UploadPhotoTaggerComponent,
    InputDialogComponent,
    ConfirmDialogComponent,
    AlertDialogComponent,
    TriCheckComponent,
    LogsComponent,
    UploadComponent,
    UnauthorizedComponent,
    NotFoundComponent,
    SecurePipe
  ],
  entryComponents: [
    ChangePasswordModalComponent,
    AccountAvatardModalComponent,
    ResetPasswordModalComponent,
    PhotoTaggerComponent,
    UserSettingsComponent,
    UploadPhotoTaggerComponent,
    InputDialogComponent,
    ConfirmDialogComponent,
    AlertDialogComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AngularFontAwesomeModule,
    RouterModule.forRoot(routes),
    BrowserAnimationsModule,
    NgxSpinnerModule,
    TypeaheadModule.forRoot(),
    PaginationModule.forRoot(),
    ModalModule.forRoot(),
    AlertModule.forRoot(),
    TypeaheadModule.forRoot(),
    ToastrModule.forRoot({
      timeOut: 4000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true
    }),
    FileUploadModule,
    AngularCropperjsModule
  ],
  providers: [
    NgxSpinnerService,
    SettingsService,
    UserSettingsService,
    PhotosService,
    ImageService,
    BsModalRef,
    TagService,
    LogService,
    LocalStorageService,
    { provide: HTTP_INTERCEPTORS, useClass: ProgressInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: CsrfHeaderInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
