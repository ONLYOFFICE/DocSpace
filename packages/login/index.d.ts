import { Request } from "express";

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
    authCallback?: (profile: string) => {};
    __ASC_INITIAL_LOGIN_STATE__: IInitialState;
    initialI18nStoreASC: IInitialI18nStoreASC;
    initialLanguage: string;
    i18n: WindowI18nType;
    [key: string]: object;
  }

  type MatchType = {
    confirmedEmail?: string;
    message?: string;
    messageKey?: string;
    authError?: string;
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
    standalone: boolean;
    trustedDomains: string[];
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
  type ProvidersType = IProvider[] | undefined;

  interface ICapabilities {
    ldapEnabled: boolean;
    providers: string[];
    ssoLabel: string;
    ssoUrl: string;
  }

  type TThemeObj = {
    accent: string;
    buttons: string;
  }

  interface ITheme {
    id: number;
    main: TThemeObj;
    text: TThemeObj;
    name: string;
  }
  interface IThemes {
    limit: number;
    selected: number;
    themes: ITheme[];
  }

  interface IError {
    status: number;
    standalone: boolean;
    message: string | undefined;
  }

  interface IInitialState {
    portalSettings?: IPortalSettings;
    buildInfo?: IBuildInfo;
    providers?: ProvidersType;
    capabilities?: ICapabilities;
    match?: MatchType;
    currentColorScheme?: ITheme;
    isAuth?: boolean;
    logoUrls: ILogoUrl[];
    error?: IError;
  }

  interface DevRequest {
    assets: assetsType;
  }
  var IS_DEVELOPMENT: boolean;
  var PORT: number;
  var IS_PERSONAL: boolean;
  var IS_ROOMS_MODE: boolean;

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

  type HTMLElementEvent<T extends HTMLElement> = Event & {
    target: T;
  };

  type TFuncType = (key: string) => string;

  interface IParsedConfig extends Object {
    PORT: number;
  }
  interface ILoginRequest extends Request {
    i18n?: I18next;
    t?: TFuncType;
  }
  type timeoutType = ReturnType<typeof setTimeout>;
  interface IAcceptLanguage {
    code?: string;
    quality?: number;
  }

  interface IUserTheme {
    [key: string]: string;
    isBase: boolean;
  }

  type TLogoPath = {
    light: string;
    dark?: string;
  }

  type TLogoSize = {
    width: number;
    height: number;
    isEmpty: boolean;
  }

  interface ILogoUrl {
    name: string;
    path: TLogoPath;
    size: TLogoSize;
  }
}