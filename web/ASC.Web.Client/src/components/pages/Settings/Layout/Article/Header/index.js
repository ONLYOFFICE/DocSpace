import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { useTranslation } from "react-i18next";

const ArticleHeaderContent = ({ isLoadedPage }) => {
  const { t } = useTranslation("Common");

  const commonSettings =
    location.pathname.includes("common/customization") ||
    location.pathname === "/settings";

  const showLoader = commonSettings ? !isLoadedPage : false;

  return showLoader ? (
    <Loaders.ArticleHeader height="32px" />
  ) : (
    <>{t("Settings")}</>
  );
};

export default React.memo(ArticleHeaderContent);
