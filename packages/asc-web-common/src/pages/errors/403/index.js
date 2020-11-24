import React, { useEffect } from "react";
import ErrorContainer from "../../../components/ErrorContainer";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { changeLanguage } from "../../../utils";

const Error403Container = () => {
  const { t } = useTranslation("translation", { i18n });

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return <ErrorContainer headerText={t("Error403Text")} />;
};

const Error403 = Error403Container;

export default Error403;
