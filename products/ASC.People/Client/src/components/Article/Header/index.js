import React from 'react';
import { connect } from 'react-redux';
import { Header } from 'asc-web-components';
import { store } from 'asc-web-common';
const { getCurrentModule } = store.auth.selectors;

const ArticleHeaderContent = ({currentModuleName}) => {
  return <Header type="menu">{currentModuleName}</Header>;
}

const mapStateToProps = (state) => {
  const currentModule = getCurrentModule(state.auth.modules, state.auth.settings.currentProductId);
  return {
      currentModuleName: (currentModule && currentModule.title) || ""
  }
}

export default connect(mapStateToProps)(ArticleHeaderContent);