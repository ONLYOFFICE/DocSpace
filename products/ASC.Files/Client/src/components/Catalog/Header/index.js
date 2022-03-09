import React from "react";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const CatalogHeaderContent = ({ currentModuleName }) => {
  return currentModuleName ? (
    <>{currentModuleName}</>
  ) : (
    <Loaders.ArticleHeader />
  );
};

export default inject(({ auth }) => {
  return {
    currentModuleName: (auth.product && auth.product.title) || "",
  };
})(observer(CatalogHeaderContent));
