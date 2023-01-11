import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import { withTranslation } from "react-i18next";
import withLoader from "../../../HOCs/withLoader";
import ArticleHeader from "@docspace/common/components/Article/sub-components/article-header";

const ArticleHeaderContent = () => {
  return <ArticleHeader />;
};

export default withTranslation([])(
  withLoader(ArticleHeaderContent)(<Loaders.ArticleHeader />)
);
