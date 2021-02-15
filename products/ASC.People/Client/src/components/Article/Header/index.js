import React from "react";
import { Headline, Loaders } from "asc-web-common";
import { inject, observer } from "mobx-react";

const ArticleHeaderContent = ({ currentModuleName }) => {
  return currentModuleName ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <Loaders.ArticleHeader />
  );
};

export default inject(({ store }) => ({
  currentModuleName: store.product.title,
}))(observer(ArticleHeaderContent));
