import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { IType } from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

@Injectable({
  providedIn: 'root',
})
export class TypeDataService {
  private baseUrl = environment.apiUrl + ApiUrls.type;
  private types: IType[] = [];

  constructor(private http: HttpClient) {}

  getTypes(): Observable<IType[]> {
    if (this.types.length > 0) {
      return this.getTypesFromCache();
    }

    return this.http.get<IType[]>(this.baseUrl).pipe(
      map((response: IType[]) => {
        this.addTypesToCache(response);
        return response;
      })
    );
  }

  private getTypesFromCache(): Observable<IType[]> {
    return of(this.types);
  }

  private addTypesToCache(response: IType[]) {
    this.types = response;
  }
}
