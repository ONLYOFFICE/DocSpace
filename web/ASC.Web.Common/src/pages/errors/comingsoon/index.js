import React, { useEffect } from "react";
import ErrorContainer from "../../../components/ErrorContainer";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";
import { changeLanguage } from "../../../utils";

const ComingSoonContainer = () => {
  const { t } = useTranslation("translation", { i18n });

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  const urlParams = window.location.pathname.split("/").filter((o) => o);
  const isProduct = urlParams[0] === "products";
  const headerText = isProduct
    ? urlParams[1][0].toUpperCase() +
      urlParams[1].substring(1) +
      ` : ` +
      t("ComingSoonHeader")
    : t("ComingSoonHeader");

  console.log(urlParams);

  return (
    <ErrorContainer
      headerText={headerText}
      bodyText={t("ComingSoonText")}
      buttonText={t("ComingSoonButtonText")}
      buttonUrl="https://www.onlyoffice.com/blog"
    />
  );
};

const ComingSoon = ComingSoonContainer;

export default ComingSoon;
