import React from "react";
import styled, { css } from "styled-components";
import { tablet, size, mobile } from "@docspace/components/utils/device";
import {
  isIOS,
  isTablet,
  isSafari,
  isChrome,
  isMobileOnly,
  isMobile,
} from "react-device-detect";
import { Base } from "@docspace/components/themes";

const tabletProps = css`
  .section-body_header {
    position: sticky;
    top: 0;
    background: ${(props) => props.theme.section.header.background};
    z-index: 202;

    ${isMobileOnly &&
    css`
      padding: 0 16px;
      margin: 0 0 0 -16px;
    `}

    ${(props) =>
      (props.settingsStudio || props.viewAs == "settings") &&
      isMobileOnly &&
      css`
        background: ${(props) => props.theme.section.header.backgroundColor};
      `}
  }
  .section-body_filter {
    display: block;
    margin: 4px 0 30px;
  }
`;

const StyledSectionContainer = styled.section`
  padding: 0 0 0 20px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  user-select: none;

  width: 100%;
  max-width: 100%;

  @media ${tablet} {
    width: 100%;
    max-width: 100vw !important;
    padding: 0 0 0 16px;
  }

  ${isMobile &&
  css`
    width: 100% !important;
    max-width: 100vw !important;
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
  `}

  .layout-progress-bar {
    position: fixed;
    right: ${(props) =>
      props.isInfoPanelVisible && !isMobile ? "416px" : "15px"};
    bottom: 21px;
  }

  .layout-progress-second-bar {
    position: fixed;
    right: ${(props) =>
      props.isInfoPanelVisible && !isMobile ? "416px" : "15px"};
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
  render() {
    //console.log("PageLayout Section render");

    return <StyledSectionContainer id="section" {...this.props} />;
  }
}

export default SectionContainer;
