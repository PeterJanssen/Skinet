import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BasketDataService, DeliveryMethodDataService } from 'src/app/core';
import { IDeliveryMethod } from 'src/app/shared';

@Component({
  selector: 'app-checkout-delivery',
  templateUrl: './checkout-delivery.component.html',
  styleUrls: ['./checkout-delivery.component.scss'],
})
export class CheckoutDeliveryComponent implements OnInit {
  @Input() checkoutForm: FormGroup;
  deliveryMethods: IDeliveryMethod[];

  constructor(
    private deliveryMethodDataService: DeliveryMethodDataService,
    private basketDataService: BasketDataService
  ) {}

  ngOnInit(): void {
    this.deliveryMethodDataService.getDeliveryMethods().subscribe(
      (dm: IDeliveryMethod[]) => {
        this.deliveryMethods = dm;
      },
      (error) => console.log(error)
    );
  }

  setShippingPrice(deliveryMethod: IDeliveryMethod): void {
    this.basketDataService.setShippingPrice(deliveryMethod);
  }
}
