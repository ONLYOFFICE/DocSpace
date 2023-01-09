import React from "react";
import { useTranslation } from "react-i18next";

const ArticleHeaderContent = () => {
  const { t } = useTranslation("Common");

  return <>{t("Settings")}</>;
};

export default React.memo(ArticleHeaderContent);
