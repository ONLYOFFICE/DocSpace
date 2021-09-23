import React from "react";
import styled, { css } from "styled-components";
import equal from "fast-deep-equal/react";
import classnames from "classnames";
import PropTypes from "prop-types";
import { LayoutContextConsumer } from "studio/Layout/context";
import { isMobile } from "react-device-detect";
import { tablet, desktop } from "@appserver/components/utils/device";
import NoUserSelect from "@appserver/components/utils/commonStyles";
const StyledSectionHeader = styled.div`
  border-bottom: 1px solid #eceef1;
  height: 55px;
  margin-right: 24px;
  margin-top: -1px;
  padding-bottom: 9px;
  ${NoUserSelect}
  ${isMobile &&
  css`
    height: 49px;
    min-height: 48px;
    max-height: 49px;
    width: ${(props) => !props.isLoaded && "100%"};
    margin-top: 64px;
    @media ${tablet} {
      margin-top: 55px;
      padding-bottom: 18px;
    }
  `}

  @media ${tablet} {
    margin-right: 16px;
  }

  .section-header {
    ${isMobile &&
    css`
      max-width: calc(100vw - 32px);
      width: 100%;
    `}

    //padding-top: 4px;
      ${isMobile &&
    css`
      position: fixed;
      top: 56px;

      width: ${(props) =>
        props.isArticlePinned ? `calc(100% - 272px)` : "100%"};

      background-color: #fff;
      z-index: 149;
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
              position: absolute;
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

    const { isArticlePinned, isHeaderVisible, ...rest } = this.props;

    return (
      <LayoutContextConsumer>
        {(value) => (
          <StyledSectionHeader
            isArticlePinned={isArticlePinned}
            isSectionHeaderVisible={value.isVisible}
          >
            <div
              className={classnames("section-header hidingHeader", {
                "section-header--hidden":
                  value.isVisible === undefined ? false : !value.isVisible,
              })}
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
  isArticlePinned: PropTypes.bool,
  isHeaderVisible: PropTypes.bool,
};
export default SectionHeader;
