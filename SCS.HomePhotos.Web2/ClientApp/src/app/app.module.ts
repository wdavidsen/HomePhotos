import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
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
//import { LoginComponent } from './login/login.component';

@NgModule({
  declarations: [
    AppComponent,
    
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,

    // PhotosComponent,
    // TagsComponent,
    // LoginComponent,
    // RegisterComponent,
    // RegisterSuccessComponent,
    // SettingsComponent,
    // UsersComponent,
    // UserDetailComponent,
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
    // UnauthorizedComponent,
    // NotFoundComponent,
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
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ]),
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
    FileUploadModule
  ],
  providers: [
    NgxSpinnerService,
    // SettingsService,
    // UserSettingsService,
    // PhotosService,
    // ImageService,
    BsModalRef,
    // TagService,
    // LogService,
    // LocalStorageService,
    // { provide: HTTP_INTERCEPTORS, useClass: ProgressInterceptor, multi: true },
    // { provide: HTTP_INTERCEPTORS, useClass: CsrfHeaderInterceptor, multi: true },
    // { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
