import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  returnUrl: string;

  constructor(
    private authService: AuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.createLoginForm();
    this.returnUrl =
      this.activatedRoute.snapshot.queryParams.returnUrl || '/shop';
  }

  createLoginForm(): void {
    this.loginForm = new FormGroup({
      email: new FormControl('', [
        Validators.required,
        Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$'),
      ]),
      password: new FormControl('', Validators.required),
    });
  }

  onSubmit(): void {
    this.authService.login(this.loginForm.value).subscribe(
      () => this.router.navigateByUrl(this.returnUrl),
      (error) => console.log(error)
    );
  }

  goToRegisterForm(): void {
    this.router.navigateByUrl(`/account/register?returnUrl=${this.returnUrl}`);
  }
}
