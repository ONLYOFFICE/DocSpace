import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({ currentModuleName, isArticleLoaded }) => {
  return !isArticleLoaded ? (
    <Loaders.ArticleHeader />
  ) : (
    <>{currentModuleName}</>
  );
};

export default inject(({ auth, filesStore }) => {
  const { isArticleLoaded } = filesStore;

  return {
    isArticleLoaded,
    currentModuleName: (auth.product && auth.product.title) || "",
  };
})(observer(ArticleHeaderContent));
