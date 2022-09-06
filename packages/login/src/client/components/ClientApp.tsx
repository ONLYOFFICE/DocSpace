import React from "react";
import { useSSR } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import ErrorBoundary from "./ErrorBoundary";
import App from "../App";
import i18n from "../i18n";
import { I18nextProvider } from "react-i18next";
import { fonts } from "@docspace/common/fonts";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@docspace/components/theme-provider";
import store from "client/store";
import { BrowserRouter } from "react-router-dom";
import GlobalStyles from "./GlobalStyle";

interface IClientApp extends IInitialState {
  initialLanguage: string;
  initialI18nStoreASC: any;
  isDesktopEditor: boolean;
}

const ThemeProviderWrapper = inject(({ auth }) => {
  const { settingsStore } = auth;
  return { theme: settingsStore.theme };
})(observer(ThemeProvider));

const ClientApp: React.FC<IClientApp> = ({
  initialLanguage,
  initialI18nStoreASC,
  ...rest
}) => {
  useSSR(initialI18nStoreASC, initialLanguage);

  return (
    <BrowserRouter>
      <MobxProvider {...store}>
        <I18nextProvider i18n={i18n}>
          <ThemeProviderWrapper>
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
      <GlobalStyles fonts={fonts} />
      <ClientApp {...props} />
    </ErrorBoundary>
  );
};

export default ClientAppWrapper;
