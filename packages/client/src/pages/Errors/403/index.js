import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";

const Error403 = () => {
  const { t } = useTranslation("Errors");

  return <ErrorContainer headerText={t("Error403Text")} />;
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <Error403 />
  </I18nextProvider>
);
