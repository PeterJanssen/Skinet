import { IOrder } from './order';
import { IProduct } from './product';

export interface IPagination {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: any[];
}

export class ProductPagination implements IPagination {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: IProduct[] = [];
}

export class OrderPagination implements IPagination {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: IOrder[] = [];
}
