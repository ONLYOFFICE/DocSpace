import React from "react";
import Headline from "@appserver/common/components/Headline";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({ isLoaded, currentModuleName }) => {
  return isLoaded ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <Loaders.ArticleHeader />
  );
};

export default inject(({ auth }) => {
  return {
    isLoaded: auth.isLoaded,
    currentModuleName: auth.product.title,
  };
})(observer(ArticleHeaderContent));
