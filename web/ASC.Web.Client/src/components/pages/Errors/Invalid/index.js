import React from "react";
import ErrorContainer from "@appserver/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";

const InvalidError = () => {
  const { t } = useTranslation();

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
