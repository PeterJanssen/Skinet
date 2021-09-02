import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SocialUser } from 'angularx-social-login';
import { AuthDataService } from 'src/app/core';
import { ExternalAuthDto } from 'src/app/shared';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  returnUrl: string;

  constructor(
    private authDataService: AuthDataService,
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
    if (this.loginForm.valid) {
      this.authDataService.login(this.loginForm.value).subscribe(
        () => this.router.navigateByUrl(this.returnUrl),
        (error) => console.log(error)
      );
    }
  }

  externalLogin = () => {
    this.authDataService.signInWithGoogle().then(
      (res) => {
        const user: SocialUser = { ...res };
        const externalAuth: ExternalAuthDto = {
          provider: user.provider,
          idToken: user.idToken,
        };
        this.validateExternalAuth(externalAuth);
      },
      (error) => console.log(error)
    );
  };

  goToRegisterForm(): void {
    this.router.navigateByUrl(`/account/register?returnUrl=${this.returnUrl}`);
  }

  private validateExternalAuth(externalAuth: ExternalAuthDto) {
    this.authDataService.externalLogin(externalAuth).subscribe(
      () => this.router.navigateByUrl(this.returnUrl),
      (error) => {
        console.log(error);
        this.authDataService.signOutExternal();
      }
    );
  }
}
