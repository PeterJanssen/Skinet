import { AuthService } from 'src/app/account/auth.service';

// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function appInitializer(authService: AuthService) {
  return () =>
    new Promise((resolve) => {
      authService.refreshToken().subscribe().add(resolve);
    });
}
