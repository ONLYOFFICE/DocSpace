import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({
  isVisitor,
  isArticleLoading,
  currentModuleName,
}) => {
  return !isVisitor && isArticleLoading ? (
    <Loaders.ArticleHeader />
  ) : (
    <>{currentModuleName}</>
  );
};

export default inject(({ auth, peopleStore }) => {
  const { loadingStore } = peopleStore;

  const { isLoading, isLoaded, firstLoad } = loadingStore;

  const isArticleLoading = (isLoading || !isLoaded) && firstLoad;
  return {
    isVisitor: auth.userStore.user.isVisitor,
    isArticleLoading,
    currentModuleName: auth.product.title,
  };
})(observer(ArticleHeaderContent));
