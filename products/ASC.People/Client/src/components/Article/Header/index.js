import React from "react";
import { connect } from "react-redux";
import { store, Headline, Loaders } from "asc-web-common";

const { getCurrentProductName } = store.auth.selectors;

const ArticleHeaderContent = ({ currentModuleName }) => {
  console.log("CurrentModuleName", currentModuleName);
  return currentModuleName ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <Loaders.ArticleHeader />
  );
};

const mapStateToProps = (state) => {
  const currentModuleName = getCurrentProductName(state);
  return {
    currentModuleName,
  };
};

export default connect(mapStateToProps)(ArticleHeaderContent);
