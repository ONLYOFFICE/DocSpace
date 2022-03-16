import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({
  isVisitor,
  isLoading,
  isLoaded,
  firstLoad,
  currentModuleName,
}) => {
  return (
    !isVisitor &&
    (firstLoad || !isLoaded ? (
      isLoading || !isLoaded ? (
        <Loaders.ArticleHeader />
      ) : (
        <>{currentModuleName}</>
      )
    ) : (
      <>{currentModuleName}</>
    ))
  );
};

export default inject(({ auth, peopleStore }) => {
  const { loadingStore } = peopleStore;
  return {
    isVisitor: auth.userStore.user.isVisitor,
    isLoading: loadingStore.isLoading,
    isLoaded: loadingStore.isLoaded,
    firstLoad: loadingStore.firstLoad,
    currentModuleName: auth.product.title,
  };
})(observer(ArticleHeaderContent));
