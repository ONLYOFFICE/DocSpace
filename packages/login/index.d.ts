import { Request } from "express";
export {};

declare global {
  interface Window {
    authCallback?: (profile: object) => {};
    __ASC_INITIAL_LOGIN_STATE__?: object;
    initialI18nStoreASC?: object;
    initialLanguage?: string;
  }

  interface IInitLoginState {
    error?: string | object;
  }

  interface DevRequest {
    assets: assetsType;
  }
  var IS_DEVELOPMENT: boolean;
  var PORT: number;

  type assetsType = { [key: string]: string } | undefined;
}
