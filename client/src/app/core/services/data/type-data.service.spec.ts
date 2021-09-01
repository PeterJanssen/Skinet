import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { TypeDataService } from './type-data.service';
import { IType } from 'src/app/shared';

describe('TypeDataService', () => {
  let typeDataService: TypeDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.type;

  const testTypes: IType[] = [
    {
      id: 1,
      name: 'Boards',
    },
    {
      id: 2,
      name: 'Hats',
    },
    {
      id: 3,
      name: 'Boots',
    },
    {
      id: 4,
      name: 'Gloves',
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TypeDataService],
    });

    typeDataService = TestBed.inject(TypeDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(typeDataService).toBeTruthy();
  });

  it('should GET all types', () => {
    typeDataService.getTypes().subscribe((types: IType[]) => {
      expect(types).toBeTruthy();
      expect(types.length).toBe(4);
    });

    const addressRequest: TestRequest =
      httpTestingController.expectOne(baseUrl);
    expect(addressRequest.request.method).toEqual('GET');

    addressRequest.flush(testTypes);

    httpTestingController.verify();
  });
});
