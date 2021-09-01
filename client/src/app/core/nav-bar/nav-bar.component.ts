import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { fromEvent, Observable } from 'rxjs';
import { debounceTime, map, startWith } from 'rxjs/operators';
import { IBasket } from 'src/app/shared/models/basket';
import { IApplicationUser } from 'src/app/shared/models/user';
import { AuthDataService, BasketDataService } from '..';

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
    private basketDataService: BasketDataService,
    private authDataService: AuthDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.basket$ = this.basketDataService.basket$;
    this.currentUser$ = this.authDataService.user$;
    this.isAdmin$ = this.authDataService.isAdmin$;

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
    this.authDataService.logout().subscribe(() => {
      this.router.navigate(['account/login']);
    });
  }
}
