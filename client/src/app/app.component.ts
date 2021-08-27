import { Component, OnInit } from '@angular/core';
import { AuthService } from './account/auth.service';
import { BasketService } from './basket/basket.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(
    private basketService: BasketService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadBasket();
  }

  loadBasket(): void {
    const basketId = localStorage.getItem('basket_id');
    if (basketId) {
      this.basketService.getBasket(basketId).subscribe(
        () => {},
        (error) => console.log(error)
      );
    }
  }
}
