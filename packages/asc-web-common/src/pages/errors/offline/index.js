import React, { useEffect } from "react";
import ErrorContainer from "../../../components/ErrorContainer";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { changeLanguage } from "../../../utils";

const ErrorOfflineContainer = () => {
  const { t } = useTranslation("translation", { i18n });

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return <ErrorContainer headerText={t("ErrorOfflineText")} />;
};

const Offline = ErrorOfflineContainer;

export default Offline;
