import { Component, OnInit } from '@angular/core';
import { ProductDataService } from '../core';
import { IProduct } from '../shared/models/product';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  products: IProduct[] = [];
  featuredProduct = [18, 16, 11, 8];
  myInterval = 3000;

  constructor(private productDataService: ProductDataService) {}

  ngOnInit(): void {
    this.getProducts();
  }

  getProducts(): void {
    this.featuredProduct.forEach((id) => {
      this.productDataService.getProduct(id).subscribe(
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
