import React, { useEffect } from "react";
import { observer, Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";

import { isMobileOnly } from "react-device-detect";

import ThemeProvider from "@docspace/components/theme-provider";
import { Portal } from "@docspace/components";
import Toast from "@docspace/components/toast";

import "@docspace/common/custom.scss";

import { RootStoreContext, RootStore, useStore } from "./store";
import Client from "./categories";

import store from "client/store";

import i18n from "./i18n";

const App = observer(() => {
  const { authStore } = useStore();
  const { init, settingsStore, userStore } = authStore;
  const { theme, setTheme } = settingsStore;

  const userTheme = userStore?.user?.theme ? userStore?.user?.theme : "Dark";

  useEffect(() => {
    const initData = async () => {
      await init();
    };

    initData();
  }, []);

  useEffect(() => {
    if (userTheme) setTheme(userTheme);
  }, [userTheme]);

  const rootElement = document.getElementById("root");

  const toast = isMobileOnly ? (
    <Portal element={<Toast />} appendTo={rootElement} visible={true} />
  ) : (
    <Toast />
  );

  return (
    <ThemeProvider theme={theme}>
      {toast}
      <Client />
    </ThemeProvider>
  );
});

export default (props: any) => (
  <MobxProvider {...store}>
    <RootStoreContext.Provider value={new RootStore()}>
      <I18nextProvider i18n={i18n}>
        <App {...props} />
      </I18nextProvider>
    </RootStoreContext.Provider>
  </MobxProvider>
);
