import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IDeliveryMethod } from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

@Injectable({
  providedIn: 'root',
})
export class DeliveryMethodDataService {
  private baseUrl = environment.apiUrl + ApiUrls.deliveryMethod;
  private deliveryMethods: IDeliveryMethod[] = [];

  constructor(private http: HttpClient) {}

  getDeliveryMethods(): Observable<IDeliveryMethod[]> {
    if (this.deliveryMethods.length > 0) {
      return this.getDeliveryMethodsFromCache();
    }

    return this.http.get<IDeliveryMethod[]>(this.baseUrl).pipe(
      map((response: IDeliveryMethod[]) => {
        response.sort((a, b) => b.price - a.price);
        this.addDeliveryMethodsToCache(response);
        return response;
      })
    );
  }

  private getDeliveryMethodsFromCache(): Observable<IDeliveryMethod[]> {
    return of(this.deliveryMethods);
  }

  private addDeliveryMethodsToCache(response: IDeliveryMethod[]) {
    this.deliveryMethods = response;
  }
}
