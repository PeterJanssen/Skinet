import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { BrandDataService } from './brand-data.service';
import { IBrand } from 'src/app/shared';

describe('BrandDataService', () => {
  let brandDataService: BrandDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.brand;

  const testBrands: IBrand[] = [
    {
      id: 1,
      name: 'Angular',
    },
    {
      id: 2,
      name: 'NetCore',
    },
    {
      id: 3,
      name: 'VS Code',
    },
    {
      id: 4,
      name: 'React',
    },
    {
      id: 5,
      name: 'Typescript',
    },
    {
      id: 6,
      name: 'Redis',
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BrandDataService],
    });

    brandDataService = TestBed.inject(BrandDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(brandDataService).toBeTruthy();
  });

  it('should GET all brands', () => {
    brandDataService.getBrands().subscribe((brands: IBrand[]) => {
      expect(brands).toBeTruthy();
      expect(brands.length).toBe(6);
    });

    const brandRequest: TestRequest = httpTestingController.expectOne(baseUrl);
    expect(brandRequest.request.method).toEqual('GET');

    brandRequest.flush(testBrands);

    httpTestingController.verify();
  });
});
