import React from "react";
import { connect } from "react-redux";
import { store, Headline, Loaders } from "asc-web-common";

const { getCurrentProduct } = store.auth.selectors;

const ArticleHeaderContent = ({ currentModuleName }) => {
  return currentModuleName ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <Loaders.Headline />
  );
};

const mapStateToProps = (state) => {
  const currentModule = getCurrentProduct(state);
  return {
    currentModuleName: (currentModule && currentModule.title) || "",
  };
};

export default connect(mapStateToProps)(ArticleHeaderContent);
