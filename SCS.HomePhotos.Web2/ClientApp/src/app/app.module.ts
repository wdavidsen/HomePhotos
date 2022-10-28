import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ToastrModule } from 'ngx-toastr';
import { FileUploadModule } from 'ng2-file-upload';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule, BsModalRef } from 'ngx-bootstrap/modal';
import { AlertModule } from 'ngx-bootstrap/alert';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularCropperjsModule } from 'angular-cropperjs';
import { PhotosComponent } from './photos/photos.component';

import { routes } from './app.routes';
import { SettingsComponent } from './settings/settings.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { TagsComponent } from './tags/tags.component';
import { NotFoundComponent } from './errors/not-found.component';
import { UnauthorizedComponent } from './errors/unauthorized.component';
import { RegisterSuccessComponent } from './register/register-success.component';
import { UserDetailComponent } from './users/user-detail.component';
import { UsersComponent } from './users/users.component';
import { CsrfHeaderInterceptor, ProgressInterceptor, TokenInterceptor } from './framework';
import { ImageService, PhotosService, TagService, UserSettingsService } from './services';
import { LogService } from './services/log.service';
import { LocalStorageService } from './services/local-storage.service';
import { SettingsService } from './services/settings.service';

@NgModule({
  declarations: [
    AppComponent,
    
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,

    // pages/routes
    UnauthorizedComponent,
    NotFoundComponent,
    PhotosComponent,
    TagsComponent,
    SettingsComponent,
    LoginComponent,
    RegisterComponent,
    RegisterSuccessComponent,    
    UsersComponent,
    UserDetailComponent,

    // AlertComponent,
    // PageInfoComponent,
    // OrganizeComponent,
    // SearchComponent,
    // AccountComponent,
    // ChangePasswordModalComponent,
    // AccountAvatardModalComponent,
    // ResetPasswordModalComponent,
    // PhotoTaggerComponent,
    // UserSettingsComponent,
    // UploadPhotoTaggerComponent,
    // InputDialogComponent,
    // ConfirmDialogComponent,
    // AlertDialogComponent,
    // TriCheckComponent,
    // LogsComponent,
    // UploadComponent,
    // SecurePipe
  ],
  entryComponents: [
    // ChangePasswordModalComponent,
    // AccountAvatardModalComponent,
    // ResetPasswordModalComponent,
    // PhotoTaggerComponent,
    // UserSettingsComponent,
    // UploadPhotoTaggerComponent,
    // InputDialogComponent,
    // ConfirmDialogComponent,
    // AlertDialogComponent
  ],
  imports: [    
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    //AngularFontAwesomeModule,
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
