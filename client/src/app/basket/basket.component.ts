import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BasketDataService } from '../core';
import { IBasket, IBasketItem, IBasketTotals } from '../shared';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss'],
})
export class BasketComponent implements OnInit {
  basket$: Observable<IBasket>;
  basketTotals$: Observable<IBasketTotals>;

  constructor(private basketDataService: BasketDataService) {}

  ngOnInit(): void {
    this.basket$ = this.basketDataService.basket$;
    this.basketTotals$ = this.basketDataService.basketTotal$;
  }

  removeBasketItem(item: IBasketItem): void {
    this.basketDataService.removeItemFromBasket(item);
  }

  incrementItemQuantity(item: IBasketItem): void {
    this.basketDataService.incrementItemQuantity(item);
  }

  decrementItemQuantity(item: IBasketItem): void {
    this.basketDataService.decrementItemQuantity(item);
  }
}
