import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IAddress } from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

@Injectable({
  providedIn: 'root',
})
export class AddressDataService {
  private baseURl = environment.apiUrl + ApiUrls.address;
  private address: IAddress;

  constructor(private http: HttpClient) {}

  getUserAddress(): Observable<IAddress> {
    if (this.address) {
      return this.getAddressFromCache();
    }

    return this.http.get<IAddress>(this.baseURl).pipe(
      map((response: IAddress) => {
        this.addAddressToCache(response);
        return response;
      })
    );
  }

  updateUserAddress(address: IAddress): Observable<IAddress> {
    return this.http.put<IAddress>(this.baseURl, address).pipe(
      map((response: IAddress) => {
        this.addAddressToCache(response);
        return response;
      })
    );
  }

  private addAddressToCache(response: IAddress): void {
    this.address = response;
  }

  private getAddressFromCache(): Observable<IAddress> {
    return of(this.address);
  }
}
