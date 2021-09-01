export interface IParam {
  sort: string;
  pageNumber: number;
  pageSize: number;
  search: string;
}

export class ProductParams implements IParam {
  brandId = 0;
  typeId = 0;
  sort = 'name';
  pageNumber = 1;
  pageSize = 6;
  search: string;
}

export class OrderParams implements IParam {
  sort = 'OrderDateDesc';
  pageNumber = 1;
  pageSize = 6;
  search: string;
  status = 0;
}
