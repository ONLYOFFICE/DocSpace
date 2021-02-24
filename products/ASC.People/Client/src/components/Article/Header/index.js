import React from "react";
import { Headline, Loaders } from "@appserver/common/src";
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
