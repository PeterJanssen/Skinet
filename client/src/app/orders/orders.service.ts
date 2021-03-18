import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IOrder } from '../shared/models/order';
import { OrderParams } from '../shared/models/orderParams';
import { OrderPagination } from '../shared/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  baseUrl = environment.apiUrl;
  orders: IOrder[] = [];
  pagination = new OrderPagination();
  orderParams = new OrderParams();

  constructor(private http: HttpClient) {}

  getOrdersForUser(): Observable<any> {
    let params = new HttpParams();

    if (this.orderParams.status && this.orderParams.status !== -1) {
      params = params.append('status', this.orderParams.status.toString());
    }

    params = params.append('sort', this.orderParams.sort);
    params = params.append('pageIndex', this.orderParams.pageNumber.toString());
    params = params.append('pageIndex', this.orderParams.pageSize.toString());

    return this.http
      .get<OrderPagination>(this.baseUrl + 'orders', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          this.orders = response.body.data;
          this.pagination = response.body;
          return this.pagination;
        })
      );
  }

  getOrderDetailed(id: number): Observable<any> {
    return this.http.get(this.baseUrl + 'orders/' + id);
  }

  setOrderParams(params: OrderParams): void {
    this.orderParams = params;
  }

  getOrderParams(): OrderParams {
    return this.orderParams;
  }
}
