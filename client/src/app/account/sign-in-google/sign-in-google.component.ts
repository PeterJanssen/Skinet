import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SocialUser } from 'angularx-social-login';
import { AuthDataService } from 'src/app/core';
import { ExternalAuthDto } from 'src/app/shared';

@Component({
  selector: 'app-sign-in-google',
  templateUrl: './sign-in-google.component.html',
  styleUrls: ['./sign-in-google.component.scss'],
})
export class SignInGoogleComponent implements OnInit {
  @Input() returnUrl: string;

  constructor(
    private authDataService: AuthDataService,
    private router: Router
  ) {}

  ngOnInit(): void {}

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
