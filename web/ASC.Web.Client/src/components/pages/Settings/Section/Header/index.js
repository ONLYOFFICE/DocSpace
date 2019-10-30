import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Text, utils } from 'asc-web-components';
import styled from 'styled-components';
import { settingsTree } from '../../../../../helpers/constants';
import { useTranslation } from 'react-i18next';

const Header = styled(Text.ContentHeader)`
  margin-left: 16px;
  margin-right: 16px;
  max-width: calc(100vw - 430px);
  @media ${utils.device.tablet} {
    max-width: calc(100vw - 96px);
  }
`;

const getSelectedLinkByKey = key => {
  const length = key.length;
  if (length === 1) {
    return settingsTree[key].link;
  }
  else if (length === 3) {
    return settingsTree[key[0]].children[key[2]].link;
  }
};

const SectionHeaderContent = props => {
  const { t } = useTranslation();

  const header = getSelectedLinkByKey(props.selectedKey)
  return (
    <Header truncate={true}>
      {t(`Settings_${header}`)}
    </Header>
  );
};

function mapStateToProps(state) {
  return {
    selectedKey: state.auth.settings.settingsTree.selectedKey[0]
  };
}

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));
