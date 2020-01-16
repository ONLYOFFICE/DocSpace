import React, { useCallback } from 'react';
import styled from 'styled-components';
import { connect } from 'react-redux';
import { withRouter } from "react-router";
import { IconButton } from 'asc-web-components';
import { Headline } from 'asc-web-common';
import { useTranslation } from 'react-i18next';
import { typeUser, typeGuest } from './../../../../../helpers/customNames';

const Wrapper = styled.div`
  display: flex;
  align-items: center;

  .arrow-button {
    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }

  .header-headline {
      margin-left: 16px;
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
      ? `${t('EditProfile')} (${profile.displayName})`
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
        className="arrow-button"
      />
      <Headline
        className='header-headline'
        type='content'
        truncate={true}
      >
        {headerText}
      </Headline>
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