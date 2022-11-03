// angular
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// 3rd party
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ToastrModule } from 'ngx-toastr';
import { FileUploadModule } from 'ng2-file-upload';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule, BsModalRef } from 'ngx-bootstrap/modal';
import { AlertModule } from 'ngx-bootstrap/alert';
import { AngularCropperjsModule } from 'angular-cropperjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { SwiperModule } from 'swiper/angular';

// local
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { PhotosComponent } from './photos/photos.component';
import { SettingsComponent } from './settings/settings.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { TagsComponent } from './tags/tags.component';
import { NotFoundComponent } from './errors/not-found.component';
import { UnauthorizedComponent } from './errors/unauthorized.component';
import { RegisterSuccessComponent } from './register/register-success.component';
import { UserDetailComponent } from './users/user-detail.component';
import { UsersComponent } from './users/users.component';
import { ProgressInterceptor, TokenInterceptor } from './framework';
import { ImageService, PhotosService, TagService, UserSettingsService } from './services';
import { LogService } from './services/log.service';
import { LocalStorageService } from './services/local-storage.service';
import { SettingsService } from './services/settings.service';
import { PhotoTaggerComponent } from './photos/photo-tagger.component';
import { AlertComponent, OrganizeComponent, PageInfoComponent, SearchComponent } from './components';
import { ResetPasswordModalComponent } from './users/reset-password-modal.component';
import { AccountAvatardModalComponent } from './account/account-avatar-modal.component';
import { AlertDialogComponent, ConfirmDialogComponent, InputDialogComponent } from './common-dialog';
import { UploadPhotoTaggerComponent } from './upload/upload-photo-tagger.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { TriCheckComponent } from './components/tri-check.component';
import { LogsComponent } from './logs/logs.component';
import { SecurePipe } from './framework/secure.pipe';
import { UploadComponent } from './upload/upload.component';
import { AccountComponent } from './account/account.component';
import { ChangePasswordModalComponent } from './account/change-password-modal.component';

import { routes } from './app.routes';

@NgModule({
    declarations: [
        AppComponent,
        // pages/routes
        HomeComponent,
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
        AccountComponent,
        LogsComponent,
        UploadComponent,
        // controls
        NavMenuComponent,
        SearchComponent,
        OrganizeComponent,
        PageInfoComponent,
        TriCheckComponent,
        AlertComponent,
        // dialogs
        PhotoTaggerComponent,
        ChangePasswordModalComponent,
        AccountAvatardModalComponent,
        ResetPasswordModalComponent,
        ConfirmDialogComponent,
        AlertDialogComponent,
        InputDialogComponent,
        UserDetailComponent,
        UserSettingsComponent,
        UploadPhotoTaggerComponent,
        // other        
        SecurePipe
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        FontAwesomeModule,
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
        AngularCropperjsModule,
        SwiperModule
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
        { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }