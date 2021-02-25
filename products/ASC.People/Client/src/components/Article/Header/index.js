import React from "react";
import Headline from "@appserver/common/Headline";
import Loaders from "@appserver/common/Loaders";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({ currentModuleName }) => {
  return currentModuleName ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <Loaders.ArticleHeader />
  );
};

export default inject(({ auth }) => ({
  currentModuleName: auth.product.title,
}))(observer(ArticleHeaderContent));
