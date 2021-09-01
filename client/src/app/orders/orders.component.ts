import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { OrderDataService } from '../core';
import { IOrder, OrderParams } from '../shared';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
})
export class OrdersComponent implements OnInit {
  @ViewChild('search', { static: false }) searchTerm: ElementRef;
  isFiltered = false;
  orders: IOrder[] = [];
  orderParams: OrderParams;
  totalCount: number;
  sortOptions = [
    { name: 'Order: latest first', value: 'OrderDateDesc' },
    { name: 'Order: earliest first', value: 'OrderDateAsc' },
    { name: 'Total: low to high', value: 'OrderPriceAsc' },
    { name: 'Total: high to low', value: 'OrderPriceDesc' },
  ];

  statusFilterOptions = [
    { name: 'Pending', id: 0 },
    { name: 'Payment Received', id: 1 },
    { name: 'Payment Failed', id: 2 },
  ];

  constructor(private orderDataService: OrderDataService) {
    this.orderParams = orderDataService.getOrderParams();
  }

  ngOnInit(): void {
    this.getOrders(true);
    this.checkIsFiltered();
  }

  checkIsFiltered() {
    if (this.orderParams.search) {
      this.isFiltered = true;
    } else {
      this.isFiltered = false;
    }
  }

  getOrders(useCache = false): void {
    this.orderDataService.getOrdersForUser(useCache).subscribe(
      (response) => {
        this.orders = response.data;
        this.totalCount = response.count;
      },
      (error) => console.log(error)
    );
  }

  onSortSelected(): void {
    this.orderDataService.setOrderParams(this.orderParams);
    this.getOrders(true);
  }

  onStatusSelected(id: number): void {
    const params = this.orderDataService.getOrderParams();
    params.status = id;
    params.pageNumber = 1;
    this.orderDataService.setOrderParams(params);
    this.getOrders();
  }

  onPageChanged(event: any): void {
    const params = this.orderDataService.getOrderParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.orderDataService.setOrderParams(params);
      this.getOrders(true);
    }
  }

  onSearch(): void {
    const params = this.orderDataService.getOrderParams();
    params.search = this.searchTerm.nativeElement.value;
    params.pageNumber = 1;
    this.orderDataService.setOrderParams(params);
    this.getOrders();
    this.isFiltered = true;
  }

  onReset(): void {
    this.searchTerm.nativeElement.value = '';
    this.orderParams = new OrderParams();
    this.orderDataService.setOrderParams(this.orderParams);
    this.getOrders();
    this.isFiltered = false;
  }
}
