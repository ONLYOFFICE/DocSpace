import React from "react";
import Headline from "@appserver/common/components/Headline";
import { useTranslation } from "react-i18next";

const ArticleHeaderContent = () => {
  const { t } = useTranslation("Common");
  return <Headline type="menu">{t("Settings")}</Headline>;
};

export default ArticleHeaderContent;
