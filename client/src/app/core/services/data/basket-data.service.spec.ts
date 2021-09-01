import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { BasketDataService } from './basket-data.service';
import { IBasket, IDeliveryMethod } from 'src/app/shared';

describe('BasketDataService', () => {
  let basketDataService: BasketDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.basket;
  const paymentUrl = environment.apiUrl + ApiUrls.payment;

  const testBasket: IBasket = {
    id: 'basket1',
    items: [
      {
        id: 1,
        brand: 'test',
        pictureUrl: '',
        price: 5,
        productName: 'test',
        quantity: 2,
        type: 'test',
      },
      {
        id: 2,
        brand: 'test',
        pictureUrl: '',
        price: 5,
        productName: 'test',
        quantity: 1,
        type: 'test',
      },
    ],
    clientSecret: '',
    deliveryMethodId: 1,
    paymentIntentId: '',
    shippingPrice: 5,
  };

  const testDeliveryMethod: IDeliveryMethod = {
    id: 1,
    shortName: 'test',
    deliveryTime: '12 weeks',
    description: 'test',
    price: 5,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BasketDataService],
    });

    basketDataService = TestBed.inject(BasketDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(basketDataService).toBeTruthy();
  });

  it('should GET a basket', () => {
    basketDataService.getBasket('basket1').subscribe();

    const basketRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + 'basket1'
    );
    expect(basketRequest.request.method).toEqual('GET');

    basketRequest.flush(testBasket);

    httpTestingController.verify();
  });

  it('should DELETE a basket', () => {
    basketDataService.deleteBasket('basket1').subscribe();

    const basketRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + 'basket1'
    );
    expect(basketRequest.request.method).toEqual('DELETE');

    basketRequest.flush({});

    httpTestingController.verify();
  });

  it('should PUT a basket', () => {
    basketDataService.getBasket('basket1').subscribe();

    let basketRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + 'basket1'
    );

    basketRequest.flush(testBasket);

    basketDataService.createPaymentIntent().subscribe();

    basketRequest = httpTestingController.expectOne(paymentUrl + 'basket1');
    expect(basketRequest.request.method).toEqual('PUT');

    basketRequest.flush(testBasket);

    httpTestingController.verify();
  });

  it('should set a shipping price', () => {
    basketDataService.getBasket('basket1').subscribe();

    let basketRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + 'basket1'
    );

    basketRequest.flush(testBasket);

    basketDataService.setShippingPrice(testDeliveryMethod);

    basketRequest = httpTestingController.expectOne(baseUrl + 'basket1');
    expect(basketRequest.request.method).toEqual('PUT');

    basketRequest.flush(testBasket);

    httpTestingController.verify();
  });

  it('should increment quantity', () => {
    basketDataService.getBasket('basket1').subscribe();

    let basketRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + 'basket1'
    );

    basketRequest.flush(testBasket);

    basketDataService.incrementItemQuantity(testBasket.items[0]);

    basketRequest = httpTestingController.expectOne(baseUrl + 'basket1');
    expect(basketRequest.request.method).toEqual('PUT');

    basketRequest.flush(testBasket);

    httpTestingController.verify();
  });

  it('should decrement quantity', () => {
    basketDataService.getBasket('basket1').subscribe();

    let basketRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + 'basket1'
    );

    basketRequest.flush(testBasket);

    basketDataService.decrementItemQuantity(testBasket.items[0]);

    basketRequest = httpTestingController.expectOne(baseUrl + 'basket1');
    expect(basketRequest.request.method).toEqual('PUT');

    basketRequest.flush(testBasket);

    httpTestingController.verify();
  });

  it('should remove item if quantity is 0', () => {
    basketDataService.getBasket('basket1').subscribe();

    let basketRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + 'basket1'
    );

    basketRequest.flush(testBasket);

    basketDataService.decrementItemQuantity(testBasket.items[1]);

    basketRequest = httpTestingController.expectOne(baseUrl + 'basket1');
    expect(basketRequest.request.method).toEqual('PUT');
    expect(testBasket.items.length).toBe(1);

    basketRequest.flush(testBasket);

    httpTestingController.verify();
  });
});
