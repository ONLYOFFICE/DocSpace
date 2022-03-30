import React from "react";
import styled, { css } from "styled-components";
import { tablet, size, mobile } from "@appserver/components/utils/device";
import {
  isIOS,
  isTablet,
  isSafari,
  isChrome,
  isMobileOnly,
  isMobile,
} from "react-device-detect";
import { Base } from "@appserver/components/themes";

const tabletProps = css`
  .section-header_header,
  .section-header_filter {
    display: none;
  }

  .section-body_header {
    display: block;
    position: sticky;
    top: 0;
    background: ${(props) => props.theme.section.header.background};
    z-index: 200;
    margin-right: -2px;

    ${isMobileOnly &&
    css`
      padding: 0 16px;
      margin: 0 -16px;
    `}
  }
  .section-body_filter {
    display: block;
    margin: ${(props) =>
      props.viewAs === "tile" ? "4px 0 18px" : "4px 0 30px"};
    margin-right: -1px;
  }
`;

const StyledSectionContainer = styled.section`
  padding: 0 0 0 20px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;

  ${(props) =>
    props.maintenanceExist &&
    isMobile &&
    css`
      .main-bar {
        animation-name: slideDown;
        -webkit-animation-name: slideDown;

        animation-duration: 0.3s;
        -webkit-animation-duration: 0.3s;

        animation-timing-function: linear;
        -webkit-animation-timing-function: linear;

        visibility: visible !important;
      }

      @keyframes slideDown {
        0% {
          transform: translateY(-100%);
        }
        50% {
          transform: translateY(-60%);
        }
        65% {
          transform: translateY(-30%);
        }
        80% {
          transform: translateY(-10%);
        }
        95% {
          transform: translateY(-5%);
        }
        100% {
          transform: translateY(0%);
        }
      }
    `}

  .main-bar,
  .main-bar--hidden {
    ${isMobile &&
    css`
      transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
    `}
  }
  .main-bar--hidden {
    ${isMobile &&
    css`
      top: -140px;
    `}
  }
  .layout-progress-bar {
    position: fixed;
    right: 15px;
    bottom: 21px;

    ${(props) =>
      !props.visible &&
      css`
        @media ${tablet} {
          bottom: 83px;
        }
      `}
  }

  .layout-progress-second-bar {
    position: fixed;
    right: 15px;
    bottom: 83px;

    ${(props) =>
      !props.visible &&
      css`
        @media ${tablet} {
          bottom: 145px;
        }
      `}
  }

  .section-header_header,
  .section-header_filter {
    display: block;
  }

  .section-body_header,
  .section-body_filter {
    display: none;
  }
  @media ${tablet} {
    padding: 0 0 0 16px;
    ${tabletProps};
  }
  ${isMobile &&
  css`
    ${tabletProps};
    min-width: 100px;
  `}
`;

StyledSectionContainer.defaultProps = { theme: Base };

class SectionContainer extends React.Component {
  /*shouldComponentUpdate() {
    return false;
  }*/
  componentDidUpdate() {
    const { pinned } = this.props;

    if (
      isIOS &&
      isTablet &&
      (isSafari || isChrome) &&
      window.innerWidth <= size.smallTablet &&
      pinned
    ) {
      this.props.unpinArticle();
    }
  }
  render() {
    //console.log("PageLayout Section render");

    return <StyledSectionContainer id="section" {...this.props} />;
  }
}

export default SectionContainer;
