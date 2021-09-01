import { Component, Input, OnInit } from '@angular/core';
import { BasketDataService } from 'src/app/core';
import { IProduct } from 'src/app/shared';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss'],
})
export class ProductItemComponent implements OnInit {
  @Input() product: IProduct;

  constructor(private basketDataService: BasketDataService) {}

  ngOnInit(): void {}

  addItemToBasket(): void {
    this.basketDataService.addItemToBasket(this.product);
  }
}
