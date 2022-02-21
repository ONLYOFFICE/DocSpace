import React from "react";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../package.json";
import { combineUrl } from "@appserver/common/utils";

import i18n from "./i18n";

const InvalidError = () => {
  const { proxyURL } = AppServerConfig;
  const homepage = config.homepage;

  const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);

  const { t } = useTranslation();

  setTimeout(() => (location.href = PROXY_HOMEPAGE_URL), 10000);

  return (
    <ErrorContainer
      headerText={t("ErrorInvalidHeader")}
      bodyText={t("ErrorInvalidText")}
    />
  );
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <InvalidError />
  </I18nextProvider>
);
