import { Component, OnInit } from '@angular/core';
import { IProduct } from '../shared/models/product';
import { ShopService } from '../shop/shop.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  products: IProduct[] = [];
  featuredProduct = [12, 9, 4, 8];
  myInterval = 3000;

  constructor(private shopService: ShopService) {}

  ngOnInit(): void {
    this.getProducts();
  }

  getProducts(): void {
    this.featuredProduct.forEach((id) => {
      this.shopService.getProduct(id).subscribe(
        (response) => {
          this.products.push(response);
        },
        (error) => {
          console.log(error);
        }
      );
    });
  }
}
