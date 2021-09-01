import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IBrand } from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

@Injectable({
  providedIn: 'root',
})
export class BrandDataService {
  private baseUrl = environment.apiUrl + ApiUrls.brand;
  private brands: IBrand[] = [];

  constructor(private http: HttpClient) {}

  getBrands(): Observable<IBrand[]> {
    if (this.brands.length > 0) {
      return this.getBrandsFromCache();
    }

    return this.http.get<IBrand[]>(this.baseUrl).pipe(
      map((response: IBrand[]) => {
        this.addBrandsToCache(response);
        return response;
      })
    );
  }

  private getBrandsFromCache(): Observable<IBrand[]> {
    return of(this.brands);
  }

  private addBrandsToCache(response: IBrand[]): void {
    this.brands = response;
  }
}
