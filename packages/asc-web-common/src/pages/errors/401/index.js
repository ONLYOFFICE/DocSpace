import React, { useEffect } from "react";
import ErrorContainer from "../../../components/ErrorContainer";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { changeLanguage } from "../../../utils";

const Error404Container = () => {
  const { t } = useTranslation("translation", { i18n });

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return <ErrorContainer headerText={t("Error401Text")} />;
};

const Error401 = Error404Container;

export default Error401;
