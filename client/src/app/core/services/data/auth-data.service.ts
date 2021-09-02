import { HttpClient } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, of, Subscription } from 'rxjs';
import { Observable } from 'rxjs';
import { delay, finalize, map, tap } from 'rxjs/operators';
import {
  ExternalAuthDto,
  IApplicationUser,
  ILoginResult,
} from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';
import { SocialAuthService } from 'angularx-social-login';
import { GoogleLoginProvider } from 'angularx-social-login';

@Injectable({
  providedIn: 'root',
})
export class AuthDataService implements OnDestroy {
  user$: Observable<IApplicationUser>;
  isAdmin$: Observable<boolean>;
  private isAdminSource = new BehaviorSubject<boolean>(null);
  private user = new BehaviorSubject<IApplicationUser>(null);
  private readonly baseURl = environment.apiUrl + ApiUrls.auth;
  private timer: Subscription;

  constructor(
    private http: HttpClient,
    private externalAuthService: SocialAuthService
  ) {
    window.addEventListener('storage', this.storageEventListener.bind(this));
    this.user$ = this.user.asObservable();
    this.isAdmin$ = this.isAdminSource.asObservable();
  }

  ngOnDestroy(): void {
    window.removeEventListener('storage', this.storageEventListener.bind(this));
  }

  login(values: any) {
    return this.http.post<ILoginResult>(`${this.baseURl}login`, values).pipe(
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

  public signInWithGoogle() {
    return this.externalAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
  public signOutExternal() {
    this.externalAuthService.signOut(true);
  }

  externalLogin(values: ExternalAuthDto) {
    return this.http
      .post<ILoginResult>(`${this.baseURl}login/google`, values)
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
    return this.http.post<ILoginResult>(`${this.baseURl}register`, values).pipe(
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

  logout(): Observable<any> {
    return this.http.post<void>(`${this.baseURl}logout`, {}).pipe(
      finalize(() => {
        this.clearLocalStorage();
        this.clearUserBehaviorSubjects();
        this.stopTokenTimer();
      })
    );
  }

  checkEmailExists(email: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseURl}emailexists?email=${email}`);
  }

  refreshToken() {
    const refreshToken = localStorage.getItem('refresh_token');
    if (!refreshToken) {
      this.clearLocalStorage();
      this.clearUserBehaviorSubjects();
      return of(null);
    }

    return this.http
      .post<ILoginResult>(`${this.baseURl}refresh-token`, { refreshToken })
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
