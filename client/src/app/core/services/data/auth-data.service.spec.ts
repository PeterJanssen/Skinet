import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { AuthDataService } from './auth-data.service';

describe('AuthDataService', () => {
  let authDataService: AuthDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.auth;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthDataService],
    });

    authDataService = TestBed.inject(AuthDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(authDataService).toBeTruthy();
  });
});
