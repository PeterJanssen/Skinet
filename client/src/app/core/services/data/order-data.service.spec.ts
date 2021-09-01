import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  HttpClientTestingModule,
  TestRequest,
} from '@angular/common/http/testing';

import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

import { OrderDataService } from './order-data.service';
import {
  IOrder,
  IOrderToCreate,
  OrderPagination,
  OrderParams,
} from 'src/app/shared';

describe('OrderDataService', () => {
  let orderDataService: OrderDataService;
  let httpTestingController: HttpTestingController;
  const baseUrl = environment.apiUrl + ApiUrls.order;

  const testPaginatedOrders: OrderPagination = {
    count: 2,
    pageIndex: 1,
    pageSize: 2,
    data: [
      {
        id: 1,
        buyerEmail: 'admin@test.com',
        deliverMethod: 'test',
        orderItems: [],
        orderDate: undefined,
        shippingPrice: 5,
        status: 'Pending',
        subTotal: 0,
        total: 5,
        shipToAddress: undefined,
      },
      {
        id: 2,
        buyerEmail: 'admin@test.com',
        deliverMethod: 'test',
        orderItems: [],
        orderDate: undefined,
        shippingPrice: 5,
        status: 'Pending',
        subTotal: 0,
        total: 5,
        shipToAddress: undefined,
      },
    ],
  };

  const testOrderParams: OrderParams = {
    pageNumber: 1,
    pageSize: 6,
    search: '',
    sort: 'OrderDateDesc',
    status: 0,
  };

  const testOrderToCreate: IOrderToCreate = {
    basketId: 'basket1',
    deliveryMethodId: 1,
    shipToAddress: {
      city: 'test',
      firstName: 'test',
      lastName: 'test',
      state: 'test',
      street: 'test',
      zipCode: 'test',
    },
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [OrderDataService],
    });

    orderDataService = TestBed.inject(OrderDataService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(orderDataService).toBeTruthy();
  });

  it('should set order params', () => {
    orderDataService.setOrderParams(testOrderParams);
    const orderParams: OrderParams = orderDataService.getOrderParams();

    expect(orderParams).toEqual(testOrderParams);
  });

  it('should GET all paginated orders', () => {
    orderDataService
      .getOrdersForUser()
      .subscribe((response: OrderPagination) => {
        expect(response.count).toBe(2);
        expect(response.pageSize).toBe(2);
        expect(response.pageIndex).toBe(1);
        expect(response.data).toBeTruthy();
        expect(response.data.length).toBe(2);
      });

    const orderRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '?sort=OrderDateDesc&pageIndex=1&pageSize=6'
    );
    expect(orderRequest.request.method).toEqual('GET');

    orderRequest.flush(testPaginatedOrders);

    httpTestingController.verify();
  });

  it('should GET an order', () => {
    orderDataService.getOrder(1).subscribe((order: IOrder) => {
      expect(order).toBeTruthy();
      expect(order).toEqual(testPaginatedOrders.data[0]);
    });

    const orderRequest: TestRequest = httpTestingController.expectOne(
      baseUrl + '1'
    );
    expect(orderRequest.request.method).toEqual('GET');

    orderRequest.flush(testPaginatedOrders.data[0]);

    httpTestingController.verify();
  });

  it('should POST an order', () => {
    orderDataService
      .createOrder(testOrderToCreate)
      .subscribe((order: IOrderToCreate) => {
        expect(testOrderToCreate).toEqual(order);
      });

    const orderRequest: TestRequest = httpTestingController.expectOne(baseUrl);
    expect(orderRequest.request.method).toEqual('POST');

    orderRequest.flush(testOrderToCreate);

    httpTestingController.verify();
  });
});
