import { CdkStepper } from '@angular/cdk/stepper';
import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, Subscription } from 'rxjs';
import { BasketDataService } from 'src/app/core';
import { IBasket } from 'src/app/shared';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss'],
})
export class CheckoutReviewComponent implements OnInit {
  @Input() appStepper: CdkStepper;
  basket$: Observable<IBasket>;

  constructor(
    private basketDataService: BasketDataService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.basket$ = this.basketDataService.basket$;
  }

  createPaymentIntent(): Subscription {
    return this.basketDataService.createPaymentIntent().subscribe(
      () => this.appStepper.next(),
      (error) => {
        console.log(error);
        this.toastr.error(error.message);
      }
    );
  }
}
