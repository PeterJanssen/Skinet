import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { AddressDataService, BasketDataService } from '../core';
import { IBasketTotals } from '../shared';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit {
  checkoutForm: FormGroup;
  basketTotals$: Observable<IBasketTotals>;

  constructor(
    private fb: FormBuilder,
    private accountDataService: AddressDataService,
    private basketDataService: BasketDataService
  ) {}

  ngOnInit(): void {
    this.createCheckoutform();
    this.getAddressFormValues();
    this.getDeliveryMethodValue();
    this.basketTotals$ = this.basketDataService.basketTotal$;
  }

  createCheckoutform(): void {
    this.checkoutForm = this.fb.group({
      addressForm: this.fb.group({
        firstName: [null, Validators.required],
        lastName: [null, Validators.required],
        street: [null, Validators.required],
        city: [null, Validators.required],
        state: [null, Validators.required],
        zipCode: [null, Validators.required],
      }),
      deliveryForm: this.fb.group({
        deliveryMethod: [null, Validators.required],
      }),
      paymentForm: this.fb.group({
        nameOnCard: [null, Validators.required],
      }),
    });
  }

  getAddressFormValues(): void {
    this.accountDataService.getUserAddress().subscribe(
      (address) => {
        if (address) {
          this.checkoutForm.get('addressForm').patchValue(address);
        }
      },
      (error) => console.log(error)
    );
  }

  getDeliveryMethodValue(): void {
    const basket = this.basketDataService.getCurrentBasketValue();
    if (basket && basket.deliveryMethodId !== null) {
      this.checkoutForm
        .get('deliveryForm')
        .get('deliveryMethod')
        .patchValue(basket.deliveryMethodId.toString());
    }
  }
}
