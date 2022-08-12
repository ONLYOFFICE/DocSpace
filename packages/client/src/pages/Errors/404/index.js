import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";
const Error404 = () => {
  const { t, ready } = useTranslation("Errors");

  return ready ? <ErrorContainer headerText={t("Error404Text")} /> : <></>;
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <Error404 />
  </I18nextProvider>
);
