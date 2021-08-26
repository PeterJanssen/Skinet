import { Component, OnInit } from '@angular/core';
import { ShopService } from '../shop/shop.service';
import { AdminService } from './admin.service';
import { IProduct } from '../shared/models/product';
import { ShopParams } from '../shared/models/shopParams';
import { ConfirmService } from '../shared/services/confirm.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})
export class AdminComponent implements OnInit {
  products: IProduct[];
  totalCount: number;
  shopParams: ShopParams;

  constructor(
    private shopService: ShopService,
    private adminService: AdminService,
    private confirmService: ConfirmService
  ) {}

  ngOnInit(): void {
    this.shopParams = new ShopParams();
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }

  getProducts(useCache = false) {
    this.shopService.getProducts(useCache).subscribe(
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
    const params = this.shopService.getShopParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.shopService.setShopParams(params);
      this.getProducts(true);
    }
  }

  deleteProduct(id: number) {
    this.confirmService
      .confirm('Confirm delete product', 'This cannot be undone')
      .subscribe((result) => {
        if (result) {
          this.adminService.deleteProduct(id).subscribe(() => {
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
