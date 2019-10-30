import React from 'react';
import { connect } from 'react-redux';
import { Text } from 'asc-web-components';
import { getCurrentModule } from '../../../store/auth/selectors';

const ArticleHeaderContent = ({currentModuleName}) => {
  return <Text.MenuHeader>{currentModuleName}</Text.MenuHeader>;
}

const mapStateToProps = (state) => {
  const currentModule = getCurrentModule(state.auth.modules, state.auth.settings.currentProductId);
  return {
      currentModuleName: (currentModule && currentModule.title) || ""
  }
}

export default connect(mapStateToProps)(ArticleHeaderContent);