import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Text, utils } from 'asc-web-components';
import styled from 'styled-components';
import { settingsTree } from '../../../../../helpers/constants';

const Header = styled(Text.ContentHeader)`
  margin-left: 16px;
  margin-right: 16px;
  max-width: calc(100vw - 430px);
  @media ${utils.device.tablet} {
    max-width: calc(100vw - 96px);
  }
`;

const getSelectedTitleByKey = key => {
  const length = key.length;
  if (length === 1) {
    return settingsTree[key].title;
  }
  else if (length === 3) {
    return settingsTree[key[0]].children[key[2]].title;
  }
};

const SectionHeaderContent = props => {

  const header = getSelectedTitleByKey(props.selectedKey)
  return (
    <Header truncate={true}>
      {header}
    </Header>
  );
};

function mapStateToProps(state) {
  return {
    selectedKey: state.auth.settings.settingsTree.selectedKey[0]
  };
}

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));
