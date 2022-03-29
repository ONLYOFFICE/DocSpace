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

const tabletProps = css`
  .section-header_filter {
    display: none;
  }

  .section-body_filter {
    display: block;
    margin: ${(props) => (props.viewAs === "tile" ? "0 0 18px" : "0 0 30px")};
  }
`;

const StyledSectionContainer = styled.section`
  padding: 0 0 0 20px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;

  //width: ${(props) => `${props.widthProp}px`};
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

  .section-header_filter {
    display: block;
  }

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
