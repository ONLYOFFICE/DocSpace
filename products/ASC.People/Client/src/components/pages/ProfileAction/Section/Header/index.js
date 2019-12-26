import React, { useCallback } from 'react';
import styled from 'styled-components';
import { connect } from 'react-redux';
import { withRouter } from "react-router";
import { IconButton, utils } from 'asc-web-components';
import { Headline } from 'asc-web-common';
import { useTranslation } from 'react-i18next';
import { typeUser, typeGuest } from './../../../../../helpers/customNames';

const Wrapper = styled.div`
  display: flex;
  align-Items: center;
`;

const HeaderContainer = styled(Headline)`
  margin-left: 16px;
  max-width: calc(100vw - 430px);

  @media ${utils.device.tablet} {
    max-width: calc(100vw - 64px);
  }
`;

const SectionHeaderContent = (props) => {
  const { profile, history, match } = props;
  const { type } = match.params;
  const { t } = useTranslation();

  const headerText = type
    ? type === "guest"
      ? t('CustomNewGuest', { typeGuest })
      : t('CustomNewEmployee', { typeUser })
    : profile
      ? profile.displayName
      : "";

  const onClickBack = useCallback(() => {
    history.goBack();
  }, [history]);

  return (
    <Wrapper>
      <IconButton
        iconName='ArrowPathIcon'
        color="#A3A9AE"
        size="16"
        hoverColor="#657077"
        isFill={true}
        onClick={onClickBack}
      />
      <HeaderContainer
        type='content'
        truncate={true}
      >
        {headerText}
      </HeaderContainer>
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