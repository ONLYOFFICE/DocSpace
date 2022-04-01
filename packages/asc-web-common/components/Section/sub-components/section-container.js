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
  .section-body_header {
    display: block;
    position: sticky;
    top: 0;
    background: ${(props) => props.theme.section.header.background};
    z-index: 20;

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
  }
`;

const StyledSectionContainer = styled.section`
  padding: 0 0 0 20px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;

  width: calc(100vw - 256px);
  max-width: calc(100vw - 256px);

  @media ${tablet} {
    width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"};
    max-width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"};
    padding: 0 0 0 16px;
  }

  ${isMobile &&
  css`
    width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"} !important;
    max-width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"} !important;
    padding: 0 0 0 16px;
    ${tabletProps};
    min-width: 100px;
  `}

  @media ${mobile} {
    width: 100vw !important;
    max-width: 100vw !important;
  }

  ${isMobileOnly &&
  css`
    width: 100vw !important;
    max-width: 100vw !important;
    margin-top: 48px !important;
  `}

  .layout-progress-bar {
    position: fixed;
    right: 15px;
    bottom: 21px;
  }

  .layout-progress-second-bar {
    position: fixed;
    right: 15px;
    bottom: 83px;
  }

  ${(props) =>
    !props.isSectionHeaderAvailable &&
    css`
      width: 100vw !important;
      max-width: 100vw !important;
      box-sizing: border-box;
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
