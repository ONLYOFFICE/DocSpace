import React from "react";
import styled, { css } from "styled-components";
import { utils } from "asc-web-components";
import equal from "fast-deep-equal/react";
import classnames from "classnames";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { LayoutContextConsumer } from "../../Layout/context";

import { getIsLoaded } from "../../../store/auth/selectors";
import { isMobile } from "react-device-detect";

const { tablet, desktop } = utils.device;

const StyledSectionHeader = styled.div`
  border-bottom: 1px solid #eceef1;
  height: 55px;
  margin-right: 24px;
  margin-top: -1px;
  ${(props) =>
    props.isLoaded &&
    isMobile &&
    css`
      //position: absolute;
      //top: 56px;
      margin-top: 71px;
      height: 49px;
      width: ${(props) => !props.isLoaded && "100%"};
    `}

  @media ${tablet} {
    margin-right: 16px;
    border-bottom: none;

    ${(props) =>
      props.borderBottom &&
      `
      border-bottom: 1px solid #eceef1;
      padding-bottom: 16px
    `};
  }

  .section-header {
    ${isMobile &&
    css`
      max-width: calc(100vw - 32px);
      width: 100%;
    `}

    //padding-top: 4px;
      ${(props) =>
      props.isLoaded &&
      isMobile &&
      css`
        position: fixed;
        top: 56px;
        width: ${(props) =>
          props.isArticlePinned ? `calc(100% - 272px)` : "100%"};
        /* ${(props) =>
          props.sectionWidth &&
          props.isArticlePinned &&
          css`
            max-width: ${props.sectionWidth + "px"};
          `} */

        background-color: #fff;
        z-index: ${(props) => (!props.isHeaderVisible ? "149" : "190")};
        padding-right: 16px;
      `}
  }
  ${isMobile &&
  css`
    .section-header,
    .section-header--hidden {
      &,
      .group-button-menu-container > div:first-child {
        transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      }
      .group-button-menu-container {
        padding-bottom: 0 !important;
        > div:first-child {
          top: ${(props) =>
            !props.isSectionHeaderVisible ? "56px" : "0px"} !important;

          @media ${desktop} {
            ${isMobile &&
            css`
              position: absolute !important;
            `}
          }
        }
      }
    }
  `}
  .section-header--hidden {
    ${isMobile &&
    css`
      top: -61px;
    `}
  }
`;

class SectionHeader extends React.Component {
  constructor(props) {
    super(props);

    this.focusRef = React.createRef();
  }

  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout SectionHeader render");
    // eslint-disable-next-line react/prop-types

    const {
      isArticlePinned,
      borderBottom,
      isHeaderVisible,
      dispatch,
      isLoaded,
      sectionWidth,
      ...rest
    } = this.props;

    return (
      <LayoutContextConsumer>
        {(value) => (
          <StyledSectionHeader
            isHeaderVisible={isHeaderVisible}
            isArticlePinned={isArticlePinned}
            borderBottom={borderBottom}
            isLoaded={isLoaded}
            isSectionHeaderVisible={value.isVisible}
            sectionWidth={sectionWidth}
          >
            <div
              className={classnames(
                "section-header needToCancelAnimationWithTransition",
                {
                  "section-header--hidden": !value.isVisible,
                }
              )}
              {...rest}
            />
          </StyledSectionHeader>
        )}
      </LayoutContextConsumer>
    );
  }
}

SectionHeader.displayName = "SectionHeader";
SectionHeader.propTypes = {
  isLoaded: PropTypes.bool,
};

const mapStateToProps = (state) => {
  return {
    isLoaded: getIsLoaded(state),
  };
};
export default connect(mapStateToProps)(SectionHeader);
