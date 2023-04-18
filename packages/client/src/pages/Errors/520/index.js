import React from "react";
import PropTypes from "prop-types";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { useParams } from "react-router-dom";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";
const Error520 = ({ match }) => {
  const { t } = useTranslation(["Common"]);
  const { error } = useParams();

  return (
    <ErrorContainer headerText={t("SomethingWentWrong")} bodyText={error} />
  );
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <Error520 />
  </I18nextProvider>
);
