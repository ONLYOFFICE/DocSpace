import React from "react";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";
const ErrorOfflineContainer = () => {
  const { t } = useTranslation();

  return <ErrorContainer headerText={t("ErrorOfflineText")} />;
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <Error520 />
  </I18nextProvider>
);
