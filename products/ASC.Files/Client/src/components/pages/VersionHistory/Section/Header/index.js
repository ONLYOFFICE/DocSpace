import React from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import { Headline, store } from "asc-web-common";
import { IconButton, utils } from "asc-web-components";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";

const { tablet, desktop } = utils.device;
const { getSettings } = store.auth.selectors;

const StyledContainer = styled.div`
  display: grid;
  grid-template-columns: auto 1fr auto auto;
  align-items: center;

  .arrow-button {
    margin-left: -8px;
    margin-right: 24px;
    min-width: 17px;

    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }

  .group-button-menu-container {
    margin: 0 -16px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    @media ${tablet} {
      & > div:first-child {
        position: absolute;
        top: 56px;
        z-index: 180;
      }
    }

    @media ${desktop} {
      margin: 0 -24px;
    }
  }

  .header-container {
    position: relative;

    display: flex;
    align-items: center;
    max-width: calc(100vw - 32px);

    .action-button {
      margin-left: 16px;

      @media ${tablet} {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 16px 8px 16px;
          margin-right: -16px;
        }
      }
    }
  }
  .headline-header {
    @media ${desktop} {
      margin-left: -9px;
    }
  }
`;

const SectionHeaderContent = (props) => {
  const { title } = props;

  const onClickBack = () => {
    const { history, settings } = props;
    history.push(settings.homepage);
  };

  return (
    <StyledContainer>
      <IconButton
        iconName="ArrowPathIcon"
        size="17"
        color="#A3A9AE"
        hoverColor="#657077"
        isFill={true}
        onClick={onClickBack}
        className="arrow-button"
      />

      <Headline className="headline-header" type="content" truncate={true}>
        {title}
      </Headline>
    </StyledContainer>
  );
};

const mapStateToProps = (state) => {
  return {
    settings: getSettings(state),
  };
};

export default connect(mapStateToProps)(
  withTranslation()(withRouter(SectionHeaderContent))
);
