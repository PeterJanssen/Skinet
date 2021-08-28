import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { IAddress, IApplicationUser } from 'src/app/shared';
import { AddressService } from '../address.service';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-my-account',
  templateUrl: './my-account.component.html',
  styleUrls: ['./my-account.component.scss'],
})
export class MyAccountComponent implements OnInit {
  addressForm: FormGroup;
  currentUser$: Observable<IApplicationUser>;

  constructor(
    private authService: AuthService,
    private addressService: AddressService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.createAddressForm();
    this.getAddressFormValues();
    this.currentUser$ = this.authService.user$;
  }

  createAddressForm(): void {
    this.addressForm = new FormGroup({
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      state: new FormControl('', Validators.required),
      street: new FormControl('', Validators.required),
      zipCode: new FormControl('', Validators.required),
    });
  }

  getAddressFormValues(): void {
    this.addressService.getUserAddress().subscribe(
      (address) => {
        if (address) {
          this.addressForm.patchValue(address);
        }
      },
      (error) => console.log(error)
    );
  }

  onSubmit(): void {
    if (this.addressForm.valid) {
      this.addressService.updateUserAddress(this.addressForm.value).subscribe(
        (address: IAddress) => {
          this.toastr.success('Address saved');
          this.addressForm.reset(address);
        },
        (error) => console.log(error)
      );
    }
  }
}
