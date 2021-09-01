import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  Basket,
  IBasket,
  IBasketItem,
  IBasketTotals,
  IDeliveryMethod,
  IProduct,
} from 'src/app/shared';
import { environment } from 'src/environments/environment';
import { ApiUrls } from './api-urls';

@Injectable({
  providedIn: 'root',
})
export class BasketDataService {
  shipping = 0;
  basket$: Observable<IBasket>;
  basketTotal$: Observable<IBasketTotals>;
  private baseUrl = environment.apiUrl + ApiUrls.basket;
  private paymentUrl = environment.apiUrl + ApiUrls.payment;
  private basketSource = new BehaviorSubject<IBasket>(null);
  private basketTotalSource = new BehaviorSubject<IBasketTotals>(null);

  constructor(private http: HttpClient) {
    this.basket$ = this.basketSource.asObservable();
    this.basketTotal$ = this.basketTotalSource.asObservable();
  }

  getBasket(id: string): Observable<void> {
    return this.http.get<IBasket>(this.baseUrl + id).pipe(
      map((basket: IBasket) => {
        this.basketSource.next(basket);
        this.shipping = basket.shippingPrice;
        this.calculateTotals();
      })
    );
  }

  getCurrentBasketValue(): IBasket {
    return this.basketSource.value;
  }

  deleteBasket(id: string): Observable<void> {
    return this.http.delete<void>(this.baseUrl + id);
  }

  deleteLocalBasket(): void {
    this.basketSource.next(null);
    this.basketTotalSource.next(null);
    localStorage.removeItem('basket_id');
  }

  resetShippingPrice(): void {
    this.shipping = 0;
  }

  addItemToBasket(item: IProduct, quantity = 1): void {
    const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(
      item,
      quantity
    );
    const basket = this.getCurrentBasketValue() ?? this.createBasket();
    basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
    this.setBasket(basket);
  }

  removeItemFromBasket(item: IBasketItem): void {
    const basket = this.getCurrentBasketValue();
    if (
      basket.items.some((basketItem: IBasketItem) => basketItem.id === item.id)
    ) {
      basket.items = basket.items.filter(
        (basketItem: IBasketItem) => basketItem.id !== item.id
      );
      if (basket.items.length > 0) {
        this.setBasket(basket);
      } else {
        this.deleteLocalBasket();
        this.deleteBasket(basket.id);
      }
    }
  }

  incrementItemQuantity(item: IBasketItem): void {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(
      (basketItem: IBasketItem) => basketItem.id === item.id
    );
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }

  decrementItemQuantity(item: IBasketItem): void {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex((x) => x.id === item.id);
    if (basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity--;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }

  setShippingPrice(deliveryMethod: IDeliveryMethod): void {
    this.shipping = deliveryMethod.price;
    const basket = this.getCurrentBasketValue();
    basket.deliveryMethodId = deliveryMethod.id;
    basket.shippingPrice = deliveryMethod.price;
    this.calculateTotals();
    this.setBasket(basket);
  }

  createPaymentIntent(): Observable<void> {
    return this.http
      .put<IBasket>(this.paymentUrl + this.getCurrentBasketValue().id, {})
      .pipe(
        map((basket: IBasket) => {
          this.basketSource.next(basket);
        })
      );
  }

  private setBasket(basket: IBasket): Subscription {
    return this.updateBasket(basket, basket.id).subscribe(
      (response: IBasket) => {
        this.basketSource.next(response);
        this.calculateTotals();
      },
      (error) => console.log(error)
    );
  }

  private updateBasket(basket: IBasket, id: string): Observable<IBasket> {
    return this.http.put<IBasket>(this.baseUrl + id, basket);
  }

  private calculateTotals(): void {
    const basket = this.getCurrentBasketValue();
    const shipping = this.shipping;
    const subTotal = basket.items.reduce(
      (result, item) => item.price * item.quantity + result,
      0
    );
    const total = shipping + subTotal;
    this.basketTotalSource.next({ shipping, total, subTotal });
  }

  private addOrUpdateItem(
    items: IBasketItem[],
    itemToAdd: IBasketItem,
    quantity: number
  ): IBasketItem[] {
    const index = items.findIndex(
      (basketItem: IBasketItem) => basketItem.id === itemToAdd.id
    );

    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }

    return items;
  }

  private mapProductItemToBasketItem(
    item: IProduct,
    quantity: number
  ): IBasketItem {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      quantity,
      brand: item.productBrand,
      type: item.productType,
    };
  }

  private createBasket(): IBasket {
    const basket = new Basket();
    localStorage.setItem('basket_id', basket.id);

    return basket;
  }
}
