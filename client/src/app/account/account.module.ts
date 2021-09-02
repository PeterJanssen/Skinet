import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AccountRoutingModule } from './account-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MyAccountComponent } from './my-account/my-account.component';
import { SignInGoogleComponent } from './sign-in-google/sign-in-google.component';

@NgModule({
  declarations: [LoginComponent, RegisterComponent, MyAccountComponent, SignInGoogleComponent],
  imports: [CommonModule, AccountRoutingModule, SharedModule],
})
export class AccountModule {}
