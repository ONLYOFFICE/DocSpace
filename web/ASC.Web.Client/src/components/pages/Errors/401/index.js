import React from "react";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";

const Error401 = () => {
  const { t } = useTranslation();

  return <ErrorContainer headerText={t("Error401Text")} />;
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <Error401 />
  </I18nextProvider>
);
