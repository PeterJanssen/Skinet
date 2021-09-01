import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BrandDataService, ProductDataService, TypeDataService } from '../core';
import {
  IBrand,
  IProduct,
  IType,
  ProductPagination,
  ProductParams,
} from '../shared';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss'],
})
export class ShopComponent implements OnInit {
  @ViewChild('search', { static: false }) searchTerm: ElementRef;
  products: IProduct[];
  brands: IBrand[];
  types: IType[];
  productParams: ProductParams;
  totalCount: number;
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' },
  ];

  constructor(
    private productDataService: ProductDataService,
    private brandDataService: BrandDataService,
    private typeDataService: TypeDataService
  ) {
    this.productParams = productDataService.getProductParams();
  }

  ngOnInit(): void {
    this.getProducts(true);
    this.getBrands();
    this.getTypes();
  }

  getProducts(useCache = false): void {
    this.productDataService.getProducts(useCache).subscribe(
      (response: ProductPagination) => {
        this.products = response.data;
        this.totalCount = response.count;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  getBrands(): void {
    this.brandDataService.getBrands().subscribe(
      (response: IBrand[]) => {
        this.brands = [{ id: 0, name: 'All' }, ...response];
      },
      (error) => {
        console.log(error);
      }
    );
  }

  getTypes(): void {
    this.typeDataService.getTypes().subscribe(
      (response: IType[]) => {
        this.types = [{ id: 0, name: 'All' }, ...response];
      },
      (error) => {
        console.log(error);
      }
    );
  }

  onBrandSelected(brandId: number): void {
    const params = this.productDataService.getProductParams();
    params.brandId = brandId;
    params.pageNumber = 1;
    this.productDataService.setProductParams(params);
    this.getProducts();
  }

  onTypeSelected(typeId: number): void {
    const params = this.productDataService.getProductParams();
    params.typeId = typeId;
    params.pageNumber = 1;
    this.productDataService.setProductParams(params);
    this.getProducts();
  }

  onSortSelected(): void {
    this.productDataService.setProductParams(this.productParams);
    this.getProducts(true);
  }

  onPageChanged(event: any): void {
    const params = this.productDataService.getProductParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.productDataService.setProductParams(params);
      this.getProducts(true);
    }
  }

  onSearch(): void {
    const params = this.productDataService.getProductParams();
    params.search = this.searchTerm.nativeElement.value;
    params.pageNumber = 1;
    this.productDataService.setProductParams(params);
    this.getProducts();
  }

  onReset(): void {
    this.searchTerm.nativeElement.value = '';
    this.productParams = new ProductParams();
    this.productDataService.setProductParams(this.productParams);
    this.getProducts();
  }
}
