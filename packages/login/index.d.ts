import { Request } from "express";
export {};

declare global {
  interface Window {
    authCallback?: (profile: object) => {};
    __ASC_INITIAL_LOGIN_STATE__: IInitialState;
    initialI18nStoreASC?: object;
    initialLanguage: string;
  }

  interface IPortalSettings extends Object {
    culture: string;
    debugInfo: boolean;
    docSpace: boolean;
    enableAdmMess: boolean;
    enabledJoin: boolean;
    greetingSettings: string;
    ownerId: string;
    passwordHash: {
      iterations: number;
      salt: string;
      size: number;
    };
    personal: boolean;
    tenantAlias: string;
    tenantStatus: number;
    thirdpartyEnable: boolean;
    trustedDomainsType: number;
    utcHoursOffset: number;
    utcOffset: string;
    version: string;
  }

  interface IBuildInfo extends Object {
    communityServer: string;
    documentServer: string;
    mailServer: string;
  }

  interface IProvider extends Object {
    linked: boolean;
    provider: string;
    url: string;
  }
  type ProvidersType = IProvider[];

  interface ICapabilities extends Object {
    ldapEnabled: boolean;
    providers: string[];
    ssoLabel: string;
    ssoUrl: string;
  }
  interface IInitialState {
    portalSettings: IPortalSettings;
    buildInfo: IBuildInfo;
    providers: ProvidersType;
    capabilities: ICapabilities;
  }

  interface DevRequest {
    assets: assetsType;
  }
  var IS_DEVELOPMENT: boolean;
  var PORT: number;

  type assetsType = { [key: string]: string } | undefined;
}
