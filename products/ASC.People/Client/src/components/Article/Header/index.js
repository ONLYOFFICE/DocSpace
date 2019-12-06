import React from 'react';
import { connect } from 'react-redux';
import { store, Heading } from 'asc-web-common';
const { getCurrentModule } = store.auth.selectors;

const ArticleHeaderContent = ({currentModuleName}) => {
  return <Heading type="menu">{currentModuleName}</Heading>;
}

const mapStateToProps = (state) => {
  const currentModule = getCurrentModule(state.auth.modules, state.auth.settings.currentProductId);
  return {
      currentModuleName: (currentModule && currentModule.title) || ""
  }
}

export default connect(mapStateToProps)(ArticleHeaderContent);