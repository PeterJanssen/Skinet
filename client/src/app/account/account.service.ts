import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IAddress } from '../shared/models/address';
import { IUser } from '../shared/models/user';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseURl = environment.apiUrl;
  currentUser$: Observable<IUser>;
  private currentUserSource = new ReplaySubject<IUser>(1);

  constructor(private http: HttpClient, private router: Router) {
    this.currentUser$ = this.currentUserSource.asObservable();
  }

  loadCurrentUser(token: string): any {
    if (token === null) {
      this.currentUserSource.next(null);
      return of(null);
    }

    return this.http.get(this.baseURl + 'account').pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  login(values: any): Observable<void> {
    return this.http.post(this.baseURl + 'account/login', values).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(values: any): Observable<void> {
    return this.http.post(this.baseURl + 'account/register', values).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/');
  }

  checkEmailExists(email: string): Observable<any> {
    return this.http.get(this.baseURl + 'account/emailexists?email=' + email);
  }

  getUserAddress(): Observable<IAddress> {
    return this.http.get<IAddress>(this.baseURl + 'account/address');
  }

  updateUserAddress(address: IAddress): Observable<IAddress> {
    return this.http.put<IAddress>(this.baseURl + 'account/address', address);
  }
}
