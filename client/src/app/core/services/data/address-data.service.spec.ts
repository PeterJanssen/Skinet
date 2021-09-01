import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { AddressDataService } from './address-data.service';
import { IAddress } from 'src/app/shared';

describe('AddressDataService', () => {
  let addressDataService: AddressDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.address;

  const testAddress: IAddress = {
    firstName: 'Bob',
    lastName: 'Bobbity',
    street: '10 The Street',
    city: 'New York',
    state: 'NY',
    zipCode: '90210',
  };

  const updatedAddress: IAddress = {
    firstName: 'Bobbientje',
    lastName: 'Bob',
    street: '15 The Street',
    city: 'Los Angeles',
    state: 'LA',
    zipCode: '90210',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AddressDataService],
    });

    addressDataService = TestBed.inject(AddressDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(addressDataService).toBeTruthy();
  });

  it('should GET an address', () => {
    addressDataService.getUserAddress().subscribe((address: IAddress) => {
      expect(address).toBeTruthy();
      expect(address).toBe(testAddress);
    });

    const addressRequest: TestRequest =
      httpTestingController.expectOne(baseUrl);
    expect(addressRequest.request.method).toEqual('GET');

    addressRequest.flush(testAddress);

    httpTestingController.verify();
  });

  it('should PUT an address', () => {
    addressDataService
      .updateUserAddress(updatedAddress)
      .subscribe((address: IAddress) => {
        expect(address).toBeTruthy();
        expect(address).toBe(updatedAddress);
      });

    const addressRequest: TestRequest =
      httpTestingController.expectOne(baseUrl);
    expect(addressRequest.request.method).toEqual('PUT');

    addressRequest.flush(updatedAddress);

    httpTestingController.verify();
  });
});
