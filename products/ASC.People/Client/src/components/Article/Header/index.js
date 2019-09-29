import React from 'react';
import { connect } from 'react-redux';
import { Text } from 'asc-web-components';
import { getCurrentModule } from '../../../store/auth/selectors';

const ArticleHeaderContent = ({currentModuleName}) => {
  return <Text.MenuHeader>{currentModuleName}</Text.MenuHeader>;
}

const mapStateToProps = (state) => {
  return {
      currentModuleName: getCurrentModule(state.auth.modules, state.auth.settings.currentProductId).title
  }
}

export default connect(mapStateToProps)(ArticleHeaderContent);