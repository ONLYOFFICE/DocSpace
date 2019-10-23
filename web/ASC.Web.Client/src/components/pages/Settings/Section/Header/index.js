import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Text, utils } from 'asc-web-components';
import styled from 'styled-components';

const Header = styled(Text.ContentHeader)`
  margin-left: 16px;
  margin-right: 16px;
  max-width: calc(100vw - 430px);
  @media ${utils.device.tablet} {
    max-width: calc(100vw - 96px);
  }
`;

const SectionHeaderContent = props => {

  return (
    <Header truncate={true}>
      {props.header}
    </Header>
  );
};

function mapStateToProps(state) {
  return {
    header: state.auth.settings.settingsTree.selectedSubtitle
  };
}

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));
