import { Component, OnInit } from '@angular/core';
import { BasketDataService } from './core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private basketDataService: BasketDataService) {}

  ngOnInit(): void {
    this.loadBasket();
  }

  loadBasket(): void {
    const basketId = localStorage.getItem('basket_id');
    if (basketId) {
      this.basketDataService.getBasket(basketId).subscribe(
        () => {},
        (error) => console.log(error)
      );
    }
  }
}
