import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({ currentModuleName, isArticleLoading }) => {
  return isArticleLoading ? (
    <Loaders.ArticleHeader />
  ) : (
    <>{currentModuleName}</>
  );
};

export default inject(({ auth, filesStore }) => {
  const { isLoaded, isLoading, firstLoad } = filesStore;
  const isArticleLoading = (!isLoaded || isLoading) && firstLoad;
  return {
    isArticleLoading,
    currentModuleName: (auth.product && auth.product.title) || "",
  };
})(observer(ArticleHeaderContent));
