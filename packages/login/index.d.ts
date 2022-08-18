import { Request } from "express";
export {};

type WindowI18nType = {
  inLoad: object[];
  loaded: {
    [key: string]: {
      data: {
        [key: string]: string | undefined;
      };
      namespaces?: string;
    };
  };
};

declare global {
  interface Window {
    authCallback?: (profile: object) => {};
    __ASC_INITIAL_LOGIN_STATE__: IInitialState;
    initialI18nStoreASC: IInitialI18nStoreASC;
    initialLanguage: string;
    i18n: WindowI18nType;
    [key: string]: object;
  }

  type MatchType = {
    confirmedEmail?: string;
    error?: string;
  };

  type PasswordHashType = {
    iterations: number;
    salt: string;
    size: number;
  };

  interface IEmailValid {
    value: string;
    isValid: boolean;
    errors: string[]; // TODO: check type
  }

  interface IPortalSettings {
    culture: string;
    debugInfo: boolean;
    docSpace: boolean;
    enableAdmMess: boolean;
    enabledJoin: boolean;
    greetingSettings: string;
    ownerId: string;
    passwordHash: PasswordHashType;
    personal: boolean;
    tenantAlias: string;
    tenantStatus: number;
    thirdpartyEnable: boolean;
    trustedDomainsType: number;
    utcHoursOffset: number;
    utcOffset: string;
    version: string;
  }

  interface IBuildInfo {
    communityServer: string;
    documentServer: string;
    mailServer: string;
  }

  interface IProvider {
    linked: boolean;
    provider: string;
    url: string;
  }
  type ProvidersType = IProvider[];

  interface ICapabilities {
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
    match: MatchType;
  }

  interface DevRequest {
    assets: assetsType;
  }
  var IS_DEVELOPMENT: boolean;
  var PORT: number;

  type assetsType = { [key: string]: string } | undefined;

  interface IInitialI18nStoreASC extends Object {
    en: {
      [Common: string]: { [key: any]: string };
      [Login: string]: { [key: any]: string };
    };
    [key: string]: {
      [Common: string]: { [key: any]: string };
      [Login: string]: { [key: any]: string };
    };
  }
}
