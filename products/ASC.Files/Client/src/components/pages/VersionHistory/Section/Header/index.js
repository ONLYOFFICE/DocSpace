import React from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import { Headline } from 'asc-web-common';
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";

const StyledContainer = styled.div`

  @media (min-width: 1024px) {
    ${props => props.isHeaderVisible && css`width: calc(100% + 76px);`}
  }

  .group-button-menu-container {
    margin: 0 -16px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    
    @media (max-width: 1024px) {
      & > div:first-child {
      position: absolute;
      top: 56px;
      z-index: 180;
      }
    }

    @media (min-width: 1024px) {
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

      @media (max-width: 1024px) {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 16px 8px 16px;
          margin-right: -16px;
        }
      }
    }
  }
`;

const SectionHeaderContent = props => {

  const { title } = props;

  return (
    <StyledContainer isHeaderVisible={true}>
        <Headline className='headline-header' type="content" truncate={true}>{title}</Headline>
    </StyledContainer>
  );
};

const mapStateToProps = state => {
  return {
  };
};

export default connect(
  mapStateToProps
)(withTranslation()(withRouter(SectionHeaderContent)));
