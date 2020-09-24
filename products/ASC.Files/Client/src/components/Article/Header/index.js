import React from "react";
import { connect } from "react-redux";
import { store, Headline, Loaders } from "asc-web-common";

const { getCurrentModule } = store.auth.selectors;

const ArticleHeaderContent = ({ currentModuleName }) => {
  return currentModuleName ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <Loaders.Headline />
  );
};

const mapStateToProps = (state) => {
  const currentModule = getCurrentModule(
    state.auth.modules,
    state.auth.settings.currentProductId
  );
  return {
    currentModuleName: (currentModule && currentModule.title) || "",
  };
};

export default connect(mapStateToProps)(ArticleHeaderContent);
