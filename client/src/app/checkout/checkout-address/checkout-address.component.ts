import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AddressService } from 'src/app/account/address.service';
import { IAddress } from 'src/app/shared';

@Component({
  selector: 'app-checkout-address',
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss'],
})
export class CheckoutAddressComponent implements OnInit {
  @Input() checkoutForm: FormGroup;

  constructor(
    private accountService: AddressService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  saveUserAddress(): void {
    this.accountService
      .updateUserAddress(this.checkoutForm.get('addressForm').value)
      .subscribe(
        (address: IAddress) => {
          this.toastr.success('Address saved');
          this.checkoutForm.get('addressForm').reset(address);
        },
        (error) => console.log(error)
      );
  }
}
