import {
  HttpClient,
  HttpEvent,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  IProduct,
  IReview,
  ProductFormValues,
  ProductPagination,
  ProductParams as ProductParams,
} from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

@Injectable({
  providedIn: 'root',
})
export class ProductDataService {
  private baseUrl = environment.apiUrl + ApiUrls.product;
  private pagination = new ProductPagination();
  private productParams = new ProductParams();
  private productCache = new Map();

  constructor(private http: HttpClient) {}

  getProducts(useCache: boolean = false): Observable<ProductPagination> {
    if (useCache === false) {
      this.createNewPaginatedProductsCache();
    }

    if (
      this.productCache.size > 0 &&
      this.productCache.has(Object.values(this.productParams).join('-'))
    ) {
      return this.getPaginatedProductsFromCache();
    }

    const params = this.createHttpParams();

    return this.http
      .get<ProductPagination>(this.baseUrl, {
        observe: 'response',
        params,
      })
      .pipe(
        map((response: HttpResponse<ProductPagination>) => {
          this.addProductsToCache(response);
          this.pagination = response.body;
          return response.body;
        })
      );
  }

  setProductParams(params: ProductParams): void {
    this.productParams = params;
  }

  getProductParams(): ProductParams {
    return this.productParams;
  }

  getProduct(id: number): Observable<IProduct> {
    let product: IProduct;

    for (const [, value] of this.productCache) {
      product = value.find(
        (cachedProduct: IProduct) => cachedProduct.id === id
      );
      if (product) {
        return of(product);
      }
    }

    return this.http.get<IProduct>(this.baseUrl + id);
  }

  createProduct(product: ProductFormValues): Observable<void> {
    return this.http.post<void>(this.baseUrl, product);
  }

  updateProduct(product: ProductFormValues, id: number): Observable<IProduct> {
    return this.http.put<IProduct>(this.baseUrl + id, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + id);
  }

  addReviewToProduct(review: IReview): Observable<IProduct> {
    return this.http.put<IProduct>(
      this.baseUrl + review.productId + '/rating',
      review
    );
  }

  uploadImage(file: File, id: number): Observable<HttpEvent<any>> {
    const formData = new FormData();
    formData.append('photo', file, 'image.png');
    return this.http.put<HttpEvent<any>>(
      this.baseUrl + id + '/photo',
      formData,
      {
        reportProgress: true,
        observe: 'events',
      }
    );
  }

  deleteProductPhoto(photoId: number, productId: number): Observable<void> {
    return this.http.delete<void>(
      this.baseUrl + productId + '/photo/' + photoId
    );
  }

  setMainPhoto(photoId: number, productId: number): Observable<IProduct> {
    return this.http.put<IProduct>(
      this.baseUrl + productId + '/photo/' + photoId,
      {}
    );
  }

  private createNewPaginatedProductsCache() {
    this.productCache = new Map();
  }

  private addProductsToCache(response: HttpResponse<ProductPagination>) {
    this.productCache.set(
      Object.values(this.productParams).join('-'),
      response.body.data
    );
  }

  private getPaginatedProductsFromCache() {
    this.pagination.data = this.productCache.get(
      Object.values(this.productParams).join('-')
    );

    return of(this.pagination);
  }

  private createHttpParams(): HttpParams {
    let params = new HttpParams();

    if (this.productParams.brandId !== 0) {
      params = params.append('brandId', this.productParams.brandId.toString());
    }

    if (this.productParams.typeId !== 0) {
      params = params.append('typeId', this.productParams.typeId.toString());
    }

    if (this.productParams.search) {
      params = params.append('search', this.productParams.search);
    }

    params = params.append('sort', this.productParams.sort);
    params = params.append(
      'pageIndex',
      this.productParams.pageNumber.toString()
    );
    params = params.append('pageSize', this.productParams.pageSize.toString());

    return params;
  }
}
