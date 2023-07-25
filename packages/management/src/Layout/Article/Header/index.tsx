import React from "react";
import { useTranslation } from "react-i18next";

const ArticleHeaderContent = () => {
  const { t } = useTranslation("Common");

  return <h1>Space management</h1>;
};

export default React.memo(ArticleHeaderContent);
