import React from "react";
import styled, { css } from "styled-components";
import { tablet, size } from "@appserver/components/utils/device";
import {
  isIOS,
  isTablet,
  isSafari,
  isChrome,
  isMobile,
} from "react-device-detect";

const tabletProps = css`
  .section-header_filter {
    display: none;
  }

  .section-body_filter {
    display: block;
    margin: 0 0 25px;
  }
`;

const StyledSection = styled.section`
  padding: 0 0 0 24px;
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

class Section extends React.Component {
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

    return <StyledSection {...this.props} />;
  }
}

export default Section;
