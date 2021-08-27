import { Component, OnInit } from '@angular/core';
import { fromEvent, Observable } from 'rxjs';
import { debounceTime, map, startWith } from 'rxjs/operators';
import { AuthService } from 'src/app/account/auth.service';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket } from 'src/app/shared/models/basket';
import { IApplicationUser } from 'src/app/shared/models/user';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss'],
})
export class NavBarComponent implements OnInit {
  basket$: Observable<IBasket>;
  currentUser$: Observable<IApplicationUser>;
  isAdmin$: Observable<boolean>;
  isScreenSmall$: Observable<boolean>;
  isNavBarOpen: boolean;

  constructor(
    private basketService: BasketService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.basket$ = this.basketService.basket$;
    this.currentUser$ = this.authService.user$;
    this.isAdmin$ = this.authService.isAdmin$;

    const checkScreenSize = () => document.body.offsetWidth < 620;

    // Create observable from window resize event throttled so only fires every 500ms
    const screenSizeChanged$ = fromEvent(window, 'resize')
      .pipe(debounceTime(500))
      .pipe(map(checkScreenSize));

    // Start off with the initial value use the isScreenSmall$ | async in the
    // view to get both the original value and the new value after resize.
    this.isScreenSmall$ = screenSizeChanged$.pipe(startWith(checkScreenSize()));
  }

  toggleCollapse(): void {
    this.isNavBarOpen = !this.isNavBarOpen;
  }

  logout(): void {
    this.authService.logout();
  }
}
