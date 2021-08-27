import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IAddress } from '../shared';

@Injectable({
  providedIn: 'root',
})
export class AddressService {
  baseURl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUserAddress(): Observable<IAddress> {
    return this.http.get<IAddress>(this.baseURl + 'account/address');
  }

  updateUserAddress(address: IAddress): Observable<IAddress> {
    return this.http.put<IAddress>(this.baseURl + 'account/address', address);
  }
}
