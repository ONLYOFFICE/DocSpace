import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import withLoading from "SRC_DIR/HOCs/withLoading";
import ArticleHeader from "@docspace/common/components/Article/sub-components/article-header";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({ isLoadedArticleBody }) => {
  return isLoadedArticleBody ? <ArticleHeader /> : <Loaders.ArticleHeader />;
};

export default inject(({ common }) => {
  const { isLoadedArticleBody } = common;

  return {
    isLoadedArticleBody,
  };
})(withLoading(withRouter(observer(ArticleHeaderContent))));
