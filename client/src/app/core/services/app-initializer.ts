import { AuthDataService } from '.';

// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function appInitializer(authDataService: AuthDataService) {
  return () =>
    new Promise((resolve) => {
      authDataService.refreshToken().subscribe().add(resolve);
    });
}
