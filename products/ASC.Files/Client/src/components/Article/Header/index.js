import React from "react";
import { connect } from "react-redux";
import { store, Headline } from "asc-web-common";
import ContentLoader from "react-content-loader";
const { getCurrentModule } = store.auth.selectors;

const HeadlineLoader = () => (
  <ContentLoader
    speed={2}
    width={264}
    height={56}
    viewBox="0 0 264 56"
    backgroundColor="#f3f3f3"
    foregroundColor="#ecebeb"
  >
    <rect x="0" y="88" rx="3" ry="3" width="178" height="6" />
    <rect x="0" y="21" rx="0" ry="0" width="216" height="23" />
  </ContentLoader>
);

const ArticleHeaderContent = ({ currentModuleName }) => {
  return currentModuleName ? (
    <Headline type="menu">{currentModuleName}</Headline>
  ) : (
    <HeadlineLoader />
  );
};

const mapStateToProps = state => {
  const currentModule = getCurrentModule(
    state.auth.modules,
    state.auth.settings.currentProductId
  );
  return {
    currentModuleName: (currentModule && currentModule.title) || ""
  };
};

export default connect(mapStateToProps)(ArticleHeaderContent);
