import React, { useEffect } from "react";
import { observer, Provider as MobxProvider } from "mobx-react";
import { I18nextProvider, useTranslation } from "react-i18next";
import { Outlet } from "react-router-dom";

import { isMobileOnly } from "react-device-detect";

import ThemeProvider from "@docspace/components/theme-provider";
import { Portal } from "@docspace/components";
import Toast from "@docspace/components/toast";

import "@docspace/common/custom.scss";

import { RootStoreContext, RootStore, useStore } from "./store";
import SimpleHeader from "./SimpleHeader";

import store from "client/store";
import Layout from "client/Layout";
import Main from "client/Main";
import NavMenu from "client/NavMenu";
import MainLayout from "SRC_DIR/Layout";

import i18n from "./i18n";

const App = observer(() => {
  const { i18n } = useTranslation();

  const { authStore } = useStore();
  const { init, settingsStore, userStore } = authStore;
  const { theme, setTheme, currentColorScheme } = settingsStore;

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
    <ThemeProvider
      theme={{ ...theme, interfaceDirection: i18n.dir() }}
      currentColorScheme={currentColorScheme}
    >
      <Layout>
        {toast}
        <NavMenu hideProfileMenu customHeader={<SimpleHeader />} />
        <Main isDesktop={false}>
          <div className="main-container">
            <MainLayout>
              <Outlet />
            </MainLayout>
          </div>
        </Main>
      </Layout>
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