import React, { useEffect } from "react";
import { observer, Provider as MobxProvider } from "mobx-react";

import ThemeProvider from "@docspace/components/theme-provider";
import "@docspace/common/custom.scss";

import { RootStoreContext, RootStore, useStore } from "./store";
import Client from "./categories";

import store from "client/store";

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

  return (
    <ThemeProvider theme={theme}>
      <Client />
    </ThemeProvider>
  );
});

export default (props: any) => (
  <MobxProvider {...store}>
    <RootStoreContext.Provider value={new RootStore()}>
      <App {...props} />
    </RootStoreContext.Provider>
  </MobxProvider>
);
