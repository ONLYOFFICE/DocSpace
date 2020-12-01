import React, { useEffect } from "react";
import PropTypes from "prop-types";
import ErrorContainer from "../../../components/ErrorContainer";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { changeLanguage } from "../../../utils";

const Error520Container = ({ match }) => {
  const { t } = useTranslation("translation", { i18n });
  const { error } = (match && match.params) || {};

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return <ErrorContainer headerText={t("Error520Text")} bodyText={error} />;
};

Error520Container.propTypes = {
  match: PropTypes.object,
};

const Error520 = Error520Container;

export default Error520;
