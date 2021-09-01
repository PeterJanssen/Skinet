import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  IOrder,
  IOrderToCreate,
  OrderPagination,
  OrderParams,
} from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

@Injectable({
  providedIn: 'root',
})
export class OrderDataService {
  private orderCache = new Map();
  private pagination = new OrderPagination();
  private orderParams = new OrderParams();
  private baseUrl = environment.apiUrl + ApiUrls.order;

  constructor(private http: HttpClient) {}

  createOrder(order: IOrderToCreate): Observable<IOrderToCreate> {
    return this.http.post<IOrderToCreate>(this.baseUrl, order);
  }

  getOrdersForUser(useCache: boolean = false): Observable<OrderPagination> {
    if (useCache === false) {
      this.createNewPaginatedOrdersCache();
    }

    if (
      this.orderCache.size > 0 &&
      this.orderCache.has(Object.values(this.orderParams).join('-'))
    ) {
      return this.getPaginatedOrdersFromCache();
    }

    const params = this.createHttpParams();

    return this.http
      .get<OrderPagination>(this.baseUrl, {
        observe: 'response',
        params,
      })
      .pipe(
        map((response: HttpResponse<OrderPagination>) => {
          this.addOrdersToCache(response);
          this.pagination = response.body;
          return response.body;
        })
      );
  }

  getOrder(id: number): Observable<IOrder> {
    let order: IOrder;

    for (const [, value] of this.orderCache) {
      order = value.find((cachedProduct: IOrder) => cachedProduct.id === id);
      if (order) {
        return of(order);
      }
    }

    return this.http.get<IOrder>(this.baseUrl + id);
  }

  setOrderParams(params: OrderParams): void {
    this.orderParams = params;
  }

  getOrderParams(): OrderParams {
    return this.orderParams;
  }

  private createNewPaginatedOrdersCache() {
    this.orderCache = new Map();
  }

  private addOrdersToCache(response: HttpResponse<OrderPagination>) {
    this.orderCache.set(
      Object.values(this.orderParams).join('-'),
      response.body.data
    );
  }

  private getPaginatedOrdersFromCache() {
    this.pagination.data = this.orderCache.get(
      Object.values(this.orderParams).join('-')
    );

    return of(this.pagination);
  }

  private createHttpParams(): HttpParams {
    let params = new HttpParams();

    if (this.orderParams.status && this.orderParams.status !== -1) {
      params = params.append('status', this.orderParams.status.toString());
    }

    if (this.orderParams.search) {
      params = params.append('search', this.orderParams.search);
    }

    params = params.append('sort', this.orderParams.sort);
    params = params.append('pageIndex', this.orderParams.pageNumber.toString());
    params = params.append('pageSize', this.orderParams.pageSize.toString());

    return params;
  }
}
