import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { of, Observable, BehaviorSubject } from 'rxjs';
import { catchError, mapTo, switchMap, tap } from 'rxjs/operators';
import { Tokens } from '../models/tokens';
import { environment } from '../../environments/environment';
import { PasswordChange, User } from '../models';

// based on: https://angular-academy.com/angular-jwt/
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private currentUserSubject: BehaviorSubject<User>;
  public currentUser: Observable<User>;

  private readonly JWT_TOKEN = 'JWT_TOKEN';
  private readonly REFRESH_TOKEN = 'REFRESH_TOKEN';
  private readonly CURRENT_USER = 'CURRENT_USER';

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem(this.CURRENT_USER)));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  getCurrentUser(): Observable<User> {
    return this.currentUser;
  }

  public get currentUserValue(): User {
    return this.currentUserSubject.value;
  }

  loadCsrfToken(): Observable<boolean> {
    return this.http.get(`${environment.apiUrl}/antiforgery`)
      .pipe(mapTo(true));
  }

  login(username, password): Observable<boolean> {
    return this.http.post<User>(`${environment.apiUrl}/auth/login`, { username, password })
      .pipe(
        tap(user => this.doLoginUser(user)),
        switchMap(() => this.loadCsrfToken()),
        mapTo(true));
  }

  loginWithPasswordChange(changeInfo: PasswordChange){
    return this.http.post<User>(`${environment.apiUrl}/auth/loginWithPasswordChange`, changeInfo)
      .pipe(
        tap(user => this.doLoginUser(user)),
        switchMap(() => this.loadCsrfToken()),
        mapTo(true));
  }

  logout() {
    return this.http.post(`${environment.apiUrl}/auth/logout`, {
      'refreshToken': this.getRefreshToken()
    })
    .pipe(
      tap(() => this.doLogoutUser()));
  }

  isLoggedIn() {
    return !!this.getJwtToken();
  }

  refreshToken() {
    return this.http.post<any>(`${environment.apiUrl}/auth/refresh`, {
        jwt: this.getJwtToken(),
        refreshToken: this.getRefreshToken()
    })
    .pipe(tap((tokens: Tokens) => {
        this.storeTokens(tokens);
        this.http.get(`${environment.apiUrl}/antiforgery`);
    }));
  }

  getJwtToken() {
    return localStorage.getItem(this.JWT_TOKEN);
  }

  storeTokens(tokens: Tokens) {
    if (tokens !== null) {
      localStorage.setItem(this.JWT_TOKEN, tokens.jwt);
      localStorage.setItem(this.REFRESH_TOKEN, tokens.refreshToken);
    }
  }

  private doLoginUser(user: User) {
    const tokens: Tokens = { jwt: user.jwt, refreshToken: user.refreshToken };
    this.storeTokens(tokens);
    this.storeUser(user);

    this.currentUserSubject.next(user);
  }

  doLogoutUser() {
    this.removeTokens();

    localStorage.removeItem(this.CURRENT_USER);
    this.currentUserSubject.next(null);
  }

  private getRefreshToken() {
    return localStorage.getItem(this.REFRESH_TOKEN);
  }

  private storeUser(user: User) {
    const userClone = {...user};
    delete userClone.jwt;
    delete userClone.refreshToken;
    localStorage.setItem(this.CURRENT_USER, JSON.stringify(userClone));
  }

  private removeTokens() {
    localStorage.removeItem(this.JWT_TOKEN);
    localStorage.removeItem(this.REFRESH_TOKEN);
  }
}
