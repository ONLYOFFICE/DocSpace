import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const CatalogHeaderContent = ({
  currentModuleName,
  isLoading,
  firstLoad,
  isLoaded,
}) => {
  return firstLoad || !isLoaded ? (
    isLoading || !isLoaded ? (
      <Loaders.ArticleHeader />
    ) : (
      <>{currentModuleName}</>
    )
  ) : (
    <>{currentModuleName}</>
  );
};

export default inject(({ auth, filesStore }) => {
  const { isLoading, firstLoad, isLoaded } = filesStore;

  return {
    isLoading,
    firstLoad,
    isLoaded,
    currentModuleName: (auth.product && auth.product.title) || "",
  };
})(observer(CatalogHeaderContent));
