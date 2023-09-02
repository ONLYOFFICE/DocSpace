import React from "react";
import { useSSR, useTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import ErrorBoundary from "./ErrorBoundary";
import App from "../App";
import i18n from "../i18n";
import { I18nextProvider } from "react-i18next";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@docspace/components/theme-provider";
import store from "client/store";
import { BrowserRouter } from "react-router-dom";
import GlobalStyles from "./GlobalStyle";

interface IClientApp extends IInitialState {
  initialLanguage: string;
  initialI18nStoreASC: any;
  isDesktopEditor: boolean;
  theme: IUserTheme;
  setTheme: (theme: IUserTheme) => void;
}

const ThemeProviderWrapper = inject(({ auth }, props) => {
  const { currentColorScheme } = props;
  const { settingsStore } = auth;
  const { i18n } = useTranslation();

  return {
    theme: {
      ...settingsStore.theme,
      interfaceDirection: i18n.dir(),
    },
    currentColorScheme,
  };
})(observer(ThemeProvider));

const ClientApp: React.FC<IClientApp> = ({
  initialLanguage,
  initialI18nStoreASC,
  ...rest
}) => {
  useSSR(initialI18nStoreASC, initialLanguage);
  const { currentColorScheme } = rest;
  return (
    <BrowserRouter forceRefresh={true}>
      <MobxProvider {...store}>
        <I18nextProvider i18n={i18n}>
          <ThemeProviderWrapper currentColorScheme={currentColorScheme}>
            <App {...rest} />
          </ThemeProviderWrapper>
        </I18nextProvider>
      </MobxProvider>
    </BrowserRouter>
  );
};

const ClientAppWrapper: React.FC<IClientApp> = (props) => {
  const onError = (errorInfo: any) => {
    toastr.error(errorInfo);
  };
  return (
    <ErrorBoundary onError={onError}>
      <ClientApp {...props} />
    </ErrorBoundary>
  );
};

export default ClientAppWrapper;
