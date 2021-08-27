import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormBuilder,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  errors: string[] = [];
  returnUrl: string;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.createRegisterForm();
    this.returnUrl =
      this.activatedRoute.snapshot.queryParams.returnUrl || '/shop';
  }

  createRegisterForm(): void {
    this.registerForm = this.fb.group({
      displayName: [null, [Validators.required]],
      email: [
        null,
        [
          Validators.required,
          Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$'),
        ],
        [this.validateEmailNotTaken()],
      ],
      password: [null, Validators.required],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) =>
      control?.value === control?.parent?.controls[matchTo].value
        ? null
        : { isMatching: true };
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe(
        () => this.router.navigateByUrl(this.returnUrl),
        (error) => {
          console.log(error);
          this.errors = error.errors;
        }
      );
    }
  }

  validateEmailNotTaken(): AsyncValidatorFn {
    return (control) =>
      timer(500).pipe(
        switchMap(() => {
          if (!control.value) {
            return of(null);
          }
          return this.authService
            .checkEmailExists(control.value)
            .pipe(map((res) => (res ? { emailExists: true } : null)));
        })
      );
  }

  goToLoginForm(): void {
    this.router.navigateByUrl(`/account/login?returnUrl=${this.returnUrl}`);
  }
}
