import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';
import { IOrderToCreate } from '../shared/models/order';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createOrder(order: IOrderToCreate): Observable<IOrderToCreate> {
    return this.http.post<IOrderToCreate>(this.baseUrl + 'orders', order);
  }

  getDeliveryMethods(): Observable<IDeliveryMethod[]> {
    return this.http
      .get(this.baseUrl + 'orders/deliveryMethods')
      .pipe(
        map((dm: IDeliveryMethod[]) => dm.sort((a, b) => b.price - a.price))
      );
  }
}
