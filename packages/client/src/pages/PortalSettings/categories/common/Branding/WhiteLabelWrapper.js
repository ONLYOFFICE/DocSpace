import React, { useEffect } from "react";
import { Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import store from "client/store";
import CommonWhiteLabel from "./CommonWhiteLabel";
import i18n from "../../i18n";
const { auth: authStore } = store;

const WhiteLabelWrapper = (props) => {
  useEffect(() => {
    authStore.init(true);
  }, []);

  return (
    <MobxProvider {...store}>
      <I18nextProvider i18n={i18n}>
        <CommonWhiteLabel {...props} />
      </I18nextProvider>
    </MobxProvider>
  );
};

export default WhiteLabelWrapper;
