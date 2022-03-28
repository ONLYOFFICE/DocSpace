import React from "react";
import styled, { css } from "styled-components";
import equal from "fast-deep-equal/react";
import classnames from "classnames";
import PropTypes from "prop-types";
import { LayoutContextConsumer } from "studio/Layout/context";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, desktop, mobile } from "@appserver/components/utils/device";
import NoUserSelect from "@appserver/components/utils/commonStyles";
import { getBannerAttribute } from "@appserver/components/utils/banner";

import Base from "@appserver/components/themes/base";

const StyledSectionHeader = styled.div`
  position: relative;
  height: 53px;
  margin-right: 20px;
  ${NoUserSelect}

  @media ${tablet} {
    height: 61px;
    margin-right: 16px;
  }

  ${isMobile &&
  css`
    margin-right: 0px !important;
  `}

  ${isMobile &&
  css`
    min-height: ${(props) =>
      props.maintenanceExist ? "61px" : "61px"} !important;
    width: ${(props) => !props.isLoaded && "100%"};
    margin-top: 62px;
    @media (max-width: 1180px) {
      margin-top: ${(props) => props.marginTop};
    }
  `}

  .section-header {
    height: 53px;
    ${isMobile &&
    css`
      max-width: calc(100vw - 32px);
      width: 100%;
    `}
    ${isMobile &&
    css`
      position: fixed;
      top: ${(props) => props.top};
      width: ${(props) =>
        props.isArticlePinned ? `calc(100% - 272px)` : "100%"};
      background-color: ${(props) =>
        props.theme.section.header.backgroundColor};
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
        padding-bottom: 0;
        > div:first-child {
          top: ${(props) => (!props.isSectionHeaderVisible ? "56px" : "0px")};
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

StyledSectionHeader.defaultProps = { theme: Base };

class SectionHeader extends React.Component {
  constructor(props) {
    super(props);

    this.state = { orientationChanged: false };
  }

  componentDidMount() {
    window.addEventListener("orientationchange", this.orientationChangeHandler);
  }

  orientationChangeHandler = () => {
    this.setState((state) => ({
      orientationChanged: !state.orientationChanged,
    }));
  };

  render() {
    // console.log("PageLayout SectionHeader render");
    // eslint-disable-next-line react/prop-types

    const {
      isArticlePinned,
      isHeaderVisible,
      viewAs,
      maintenanceExist,
      snackbarExist,
      ...rest
    } = this.props;

    let top = "48px";
    let marginTop = "46px";

    const mainBar = document.getElementById("main-bar");

    if (maintenanceExist) {
      const { sectionHeaderTop, sectionHeaderMarginTop } = getBannerAttribute();

      top = sectionHeaderTop;
      marginTop = sectionHeaderMarginTop;
    }

    return (
      <LayoutContextConsumer>
        {(value) => (
          <StyledSectionHeader
            isArticlePinned={isArticlePinned}
            isSectionHeaderVisible={value.isVisible}
            viewAs={viewAs}
            maintenanceExist={maintenanceExist && mainBar}
            top={top}
            marginTop={marginTop}
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
