import React from "react";
import { Headline } from "asc-web-common";
import { useTranslation } from "react-i18next";

const ArticleHeaderContent = () => {
  const { t } = useTranslation();
  return <Headline type="menu">{t("Settings")}</Headline>;
};

export default ArticleHeaderContent;
