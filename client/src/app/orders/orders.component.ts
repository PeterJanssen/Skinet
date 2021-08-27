import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IOrder, OrderParams } from '../shared';
import { OrdersService } from './orders.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
})
export class OrdersComponent implements OnInit {
  @ViewChild('search', { static: false }) searchTerm: ElementRef;
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
    { name: 'Pending', value: 0 },
    { name: 'Payment Received', value: 1 },
    { name: 'Payment Failed', value: 2 },
  ];
  isStatusCollapsed = false;

  constructor(private orderService: OrdersService) {
    this.orderParams = orderService.getOrderParams();
  }

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders(): void {
    this.orderService.getOrdersForUser().subscribe(
      (response) => {
        this.orders = response.data;
        this.totalCount = response.count;
      },
      (error) => console.log(error)
    );
  }

  onSortSelected(sort: string): void {
    const params = this.orderService.getOrderParams();
    params.sort = sort;
    this.orderService.setOrderParams(params);
    this.getOrders();
  }

  onStatusSelected(status: number): void {
    const params = this.orderService.getOrderParams();
    params.status = status;
    params.pageNumber = 1;
    this.orderService.setOrderParams(params);
    this.getOrders();
  }

  onPageChanged(event: any): void {
    const params = this.orderService.getOrderParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.orderService.setOrderParams(params);
      this.getOrders();
    }
  }

  onSearch(): void {
    const params = this.orderService.getOrderParams();
    params.search = this.searchTerm.nativeElement.value;
    params.pageNumber = 1;
    this.orderService.setOrderParams(params);
    this.getOrders();
  }

  onReset(): void {
    this.searchTerm.nativeElement.value = '';
    this.orderParams = new OrderParams();
    this.orderService.setOrderParams(this.orderParams);
    this.getOrders();
  }
}
