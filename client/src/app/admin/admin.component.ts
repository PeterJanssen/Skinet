import { Component, OnInit } from '@angular/core';
import { ConfirmService, IProduct, ProductParams } from '../shared';
import { ProductDataService } from '../core';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})
export class AdminComponent implements OnInit {
  products: IProduct[];
  totalCount: number;
  productParams: ProductParams;

  constructor(
    private productDataService: ProductDataService,
    private confirmService: ConfirmService
  ) {}

  ngOnInit(): void {
    this.productParams = new ProductParams();
    this.productDataService.setProductParams(this.productParams);
    this.getProducts();
  }

  getProducts(useCache = false) {
    this.productDataService.getProducts(useCache).subscribe(
      (response) => {
        this.products = response.data;
        this.totalCount = response.count;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  onPageChanged(event: any) {
    const params = this.productDataService.getProductParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.productDataService.setProductParams(params);
      this.getProducts(true);
    }
  }

  deleteProduct(id: number, name: string) {
    this.confirmService
      .confirm(
        `Confirm delete product "${name}"`,
        'This action cannot be undone',
        'Confirm'
      )
      .subscribe((result) => {
        if (result) {
          this.productDataService.deleteProduct(id).subscribe(() => {
            this.products.splice(
              this.products.findIndex((p) => p.id === id),
              1
            );
            this.totalCount--;
          });
        }
      });
  }
}
