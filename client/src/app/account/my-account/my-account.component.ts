import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AddressDataService, AuthDataService } from 'src/app/core';
import { IAddress, IApplicationUser } from 'src/app/shared';

@Component({
  selector: 'app-my-account',
  templateUrl: './my-account.component.html',
  styleUrls: ['./my-account.component.scss'],
})
export class MyAccountComponent implements OnInit {
  addressForm: FormGroup;
  currentUser$: Observable<IApplicationUser>;

  constructor(
    private authDataService: AuthDataService,
    private addressDataService: AddressDataService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.createAddressForm();
    this.getAddressFormValues();
    this.currentUser$ = this.authDataService.user$;
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
    this.addressDataService.getUserAddress().subscribe(
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
      this.addressDataService
        .updateUserAddress(this.addressForm.value)
        .subscribe(
          (address: IAddress) => {
            this.toastr.success('Address saved');
            this.addressForm.reset(address);
          },
          (error) => console.log(error)
        );
    }
  }
}
