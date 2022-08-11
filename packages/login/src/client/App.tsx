import React from "react";
import { useSSR } from "react-i18next";
import { fonts } from "@docspace/common/fonts";
import GlobalStyle from "./components/GlobalStyle";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@docspace/components/theme-provider";
import store from "client/store";

const ThemeProviderWrapper = inject(({ auth }) => {
  const { settingsStore } = auth;
  return { theme: settingsStore.theme };
})(observer(ThemeProvider));

interface ILoginProps {
  portalSettings: IPortalSettings;
  buildInfo: IBuildInfo;
  providers: ProvidersType;
  capabilities: ICapabilities;
  initialLanguage: string;
  initialI18nStoreASC: any;
}
const App: React.FC<ILoginProps> = ({
  initialLanguage,
  initialI18nStoreASC,
  ...rest
}) => {
  useSSR(initialI18nStoreASC, initialLanguage);

  return (
    <MobxProvider {...store}>
      <ThemeProviderWrapper>
        <GlobalStyle fonts={fonts} /> Test: {JSON.stringify(rest)}
      </ThemeProviderWrapper>
    </MobxProvider>
  );
};

export default App;
