import { Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, Subscription } from 'rxjs';
import { map, tap, delay, finalize } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IApplicationUser, ILoginResult } from '../shared';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements OnDestroy {
  user$: Observable<IApplicationUser>;
  isAdmin$: Observable<boolean>;
  private isAdminSource = new BehaviorSubject<boolean>(null);
  private user = new BehaviorSubject<IApplicationUser>(null);
  private readonly baseURl = environment.apiUrl;
  private timer: Subscription;

  constructor(private router: Router, private http: HttpClient) {
    window.addEventListener('storage', this.storageEventListener.bind(this));
    this.user$ = this.user.asObservable();
    this.isAdmin$ = this.isAdminSource.asObservable();
  }

  ngOnDestroy(): void {
    window.removeEventListener('storage', this.storageEventListener.bind(this));
  }

  login(values: any) {
    return this.http
      .post<ILoginResult>(`${this.baseURl}Auth/login`, values)
      .pipe(
        map((loginResult: ILoginResult) => {
          this.user.next({
            email: loginResult.email,
            username: loginResult.username,
            displayName: loginResult.displayName,
            role: loginResult.role,
            originalUserName: loginResult.originalUserName,
          });
          this.isAdminSource.next(this.isAdmin(loginResult.accessToken));
          this.setLocalStorage(loginResult);
          this.startTokenTimer();
          return loginResult;
        })
      );
  }

  register(values: any) {
    return this.http
      .post<ILoginResult>(`${this.baseURl}Auth/register`, values)
      .pipe(
        map((loginResult: ILoginResult) => {
          this.user.next({
            email: loginResult.email,
            username: loginResult.username,
            displayName: loginResult.displayName,
            role: loginResult.role,
            originalUserName: loginResult.originalUserName,
          });
          this.setLocalStorage(loginResult);
          this.startTokenTimer();
          return loginResult;
        })
      );
  }

  logout() {
    this.http
      .post<unknown>(`${this.baseURl}Auth/logout`, {})
      .pipe(
        finalize(() => {
          this.clearLocalStorage();
          this.clearUserBehaviorSubjects();
          this.stopTokenTimer();
          this.router.navigate(['account/login']);
        })
      )
      .subscribe();
  }

  checkEmailExists(email: string): Observable<any> {
    return this.http.get(`${this.baseURl}auth/emailexists?email=${email}`);
  }

  refreshToken() {
    const refreshToken = localStorage.getItem('refresh_token');
    if (!refreshToken) {
      this.clearLocalStorage();
      this.clearUserBehaviorSubjects();
      return of(null);
    }

    return this.http
      .post<ILoginResult>(`${this.baseURl}Auth/refresh-token`, { refreshToken })
      .pipe(
        map((loginResult: ILoginResult) => {
          this.user.next({
            email: loginResult.email,
            username: loginResult.username,
            displayName: loginResult.displayName,
            role: loginResult.role,
            originalUserName: loginResult.originalUserName,
          });
          this.setLocalStorage(loginResult);
          this.isAdminSource.next(this.isAdmin(loginResult.accessToken));
          this.startTokenTimer();
          return loginResult;
        })
      );
  }

  setLocalStorage(loginResult: ILoginResult) {
    localStorage.setItem('access_token', loginResult.accessToken);
    localStorage.setItem('refresh_token', loginResult.refreshToken);
    localStorage.setItem('login-event', 'login' + Math.random());
  }

  clearLocalStorage() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.setItem('logout-event', 'logout' + Math.random());
  }

  clearUserBehaviorSubjects() {
    this.user.next(null);
    this.isAdminSource.next(null);
  }

  isAdmin(token: string): boolean {
    if (token) {
      const decodedToken = JSON.parse(atob(token.split('.')[1]));
      if (decodedToken.role.indexOf('Admin') > -1) {
        return true;
      }
    }
  }

  private getTokenRemainingTime() {
    const accessToken = localStorage.getItem('access_token');
    if (!accessToken) {
      return 0;
    }
    const jwtToken = JSON.parse(atob(accessToken.split('.')[1]));
    const expires = new Date(jwtToken.exp * 1000);
    return expires.getTime() - Date.now();
  }

  private startTokenTimer() {
    const timeout = this.getTokenRemainingTime();
    this.timer = of(true)
      .pipe(
        delay(timeout),
        tap(() => this.refreshToken().subscribe())
      )
      .subscribe();
  }

  private stopTokenTimer() {
    this.timer?.unsubscribe();
  }

  private storageEventListener(event: StorageEvent) {
    if (event.storageArea === localStorage) {
      if (event.key === 'logout-event') {
        this.stopTokenTimer();
        this.user.next(null);
        this.isAdminSource.next(null);
      }
      if (event.key === 'login-event') {
        this.stopTokenTimer();
        this.http
          .get<ILoginResult>(`${this.baseURl}Account`)
          .subscribe((loginResult: ILoginResult) => {
            this.user.next({
              email: loginResult.email,
              username: loginResult.username,
              displayName: loginResult.displayName,
              role: loginResult.role,
              originalUserName: loginResult.originalUserName,
            });
            this.isAdminSource.next(this.isAdmin(loginResult.accessToken));
          });
      }
    }
  }
}
