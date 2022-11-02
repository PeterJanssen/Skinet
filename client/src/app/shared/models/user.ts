export interface IApplicationUser {
  email: string;
  username: string;
  displayName: string;
  role: string;
  originalUserName: string;
}

export interface ILoginResult {
  email: string;
  username: string;
  displayName: string;
  role: string;
  originalUserName: string;
  accessToken: string;
}

export interface ExternalAuthDto {
  provider: string;
  idToken: string;
}
