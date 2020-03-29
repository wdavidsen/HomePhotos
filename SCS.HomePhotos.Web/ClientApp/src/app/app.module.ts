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
import { AlertComponent } from './components';
import { JwtInterceptor, ErrorInterceptor, AuthGuard } from './helpers';
import { SettingsService } from './services/settings.service';
import { AccountComponent } from './account/account.component';
import { PhotosService, TagService, PageInfoService } from './services';
import { PageInfoComponent } from './components/page-info.component';

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
    AlertComponent,
    PageInfoComponent,
    AccountComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: PhotosComponent, pathMatch: 'full', canActivate: [AuthGuard] },
      { path: 'photos/:tagName', component: PhotosComponent, canActivate: [AuthGuard] },
      { path: 'tags', component: TagsComponent, canActivate: [AuthGuard] },
      { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard] },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },

      // otherwise redirect to home
      { path: '**', redirectTo: '' }
    ])
  ],
  providers: [
    SettingsService,
    PhotosService,
    TagService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
