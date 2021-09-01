import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { DeliveryMethodDataService } from './delivery-method-data.service';
import { IDeliveryMethod } from 'src/app/shared';

describe('DeliveryMethodDataService', () => {
  let deliveryMethodDataService: DeliveryMethodDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.deliveryMethod;

  const testDeliveryMethods: IDeliveryMethod[] = [
    {
      id: 1,
      shortName: 'UPS1',
      description: 'Fastest delivery time',
      deliveryTime: '1-2 Days',
      price: 10,
    },
    {
      id: 2,
      shortName: 'UPS2',
      description: 'Get it within 5 days',
      deliveryTime: '2-5 Days',
      price: 5,
    },
    {
      id: 3,
      shortName: 'UPS3',
      description: 'Slower but cheap',
      deliveryTime: '5-10 Days',
      price: 2,
    },
    {
      id: 4,
      shortName: 'FREE',
      description: 'Free! You get what you pay for',
      deliveryTime: '1-2 Weeks',
      price: 0,
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DeliveryMethodDataService],
    });

    deliveryMethodDataService = TestBed.inject(DeliveryMethodDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(deliveryMethodDataService).toBeTruthy();
  });

  it('should GET all deliveryMethods', () => {
    deliveryMethodDataService
      .getDeliveryMethods()
      .subscribe((deliveryMethods: IDeliveryMethod[]) => {
        expect(deliveryMethods).toBeTruthy();
        expect(deliveryMethods.length).toBe(4);
      });

    const addressRequest: TestRequest =
      httpTestingController.expectOne(baseUrl);
    expect(addressRequest.request.method).toEqual('GET');

    addressRequest.flush(testDeliveryMethods);

    httpTestingController.verify();
  });
});
