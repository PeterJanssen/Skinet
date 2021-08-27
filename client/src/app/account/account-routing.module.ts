import { NgModule } from '@angular/core';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { RouterModule, Routes } from '@angular/router';
import { MyAccountComponent } from './my-account/my-account.component';
import { AuthGuard, LoggedInGuard } from '../core';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [LoggedInGuard],
    data: { breadcrumb: 'Login' },
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [LoggedInGuard],
    data: { breadcrumb: 'Register' },
  },
  {
    path: 'my-account',
    component: MyAccountComponent,
    canActivate: [AuthGuard],
    data: { breadcrumb: 'My Account' },
  },
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountRoutingModule {}
