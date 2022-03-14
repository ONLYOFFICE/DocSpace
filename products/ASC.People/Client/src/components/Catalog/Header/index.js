import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const CatalogHeaderContent = ({
  isVisitor,
  isLoading,
  firstLoad,
  currentModuleName,
}) => {
  return (
    !isVisitor &&
    (firstLoad ? (
      isLoading ? (
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
    firstLoad: loadingStore.firstLoad,
    currentModuleName: auth.product.title,
  };
})(observer(CatalogHeaderContent));
