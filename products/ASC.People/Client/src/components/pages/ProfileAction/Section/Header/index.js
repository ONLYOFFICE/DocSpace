import React, { useCallback } from 'react';
import styled from 'styled-components';
import { connect } from 'react-redux';
import { withRouter } from "react-router";
import { IconButton, Text } from 'asc-web-components';
import { useTranslation } from 'react-i18next';

const Wrapper = styled.div`
  display: flex;
  align-Items: center;
`;

const Header = styled(Text.ContentHeader)`
  margin-left: 16px;
`;

const SectionHeaderContent = (props) => {
  const { profile, history, settings, match } = props;
  const { type } = match.params;
  const { t } = useTranslation();

  const headerText = type
    ? type === "guest"
      ? t('NewGuest')
      : t('NewEmployee')
    : profile
      ? profile.displayName
      : "";

  const onClick = useCallback(() => {
    history.push(settings.homepage)
  }, [history, settings]);

  return (
    <Wrapper>
      <IconButton iconName={'ArrowPathIcon'} size="16" onClick={onClick}/>
      <Header>{headerText}</Header>
    </Wrapper>
  );
};

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings
  };
};

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));