import React from "react";
import Headline from "@appserver/common/src/components/Headline";
import Loaders from "@appserver/common/src/components/Loaders";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({ currentModuleName }) => {
  return currentModuleName ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <Loaders.ArticleHeader />
  );
};

export default inject(({ auth }) => {
  return {
    currentModuleName: (auth.product && auth.product.title) || "",
  };
})(observer(ArticleHeaderContent));
