import React, { useCallback } from "react";
import { connect } from "react-redux";
import { IconButton } from "asc-web-components";
import { Headline } from 'asc-web-common';
import { withRouter } from "react-router";
// import { useTranslation } from 'react-i18next';
import styled from "styled-components";

const Wrapper = styled.div`
  display: flex;
  align-items: center;

  .arrow-button {
    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }
`;

const textStyle = {
  marginLeft: "16px",
  marginRight: "16px"
};

const SectionHeaderContent = props => {
  const { history, settings } = props;
  //const { t } = useTranslation();

  const onClickBack = useCallback(() => {
    history.push(settings.homepage);
  }, [history, settings]);

  return (
    <Wrapper>
      <div style={{ width: "16px" }}>
        <IconButton
          iconName="ArrowPathIcon"
          color="#A3A9AE"
          size="16"
          hoverColor="#657077"
          isFill={true}
          onClick={onClickBack}
          className="arrow-button"
        />
      </div>
      <Headline type="content" truncate={true} style={textStyle}>
        Add users to the portal (Development)
      </Headline>
    </Wrapper>
  );
};

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));
