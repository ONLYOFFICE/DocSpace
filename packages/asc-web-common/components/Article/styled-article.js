import styled, { css } from "styled-components";

import {
  isMobile,
  isMobileOnly,
  isTablet,
  isDesktop,
} from "react-device-detect";
import {
  mobile,
  tablet,
  isMobile as isMobileUtils,
} from "@appserver/components/utils/device";

import Heading from "@appserver/components/heading";
import { Base } from "@appserver/components/themes";

import MenuIcon from "@appserver/components/public/static/images/menu.react.svg";
import CrossIcon from "@appserver/components/public/static/images/cross.react.svg";

const StyledArticle = styled.article`
  position: relative;
  overflow: hidden;
  background: ${(props) => props.theme.catalog.background};

  min-width: 251px;
  max-width: 251px;

  box-sizing: border-box;

  padding: 0 20px;

  @media ${tablet} {
    min-width: ${(props) => (props.showText ? "243px" : "60px")};
    max-width: ${(props) => (props.showText ? "243px" : "60px")};

    padding: 0 8px;
  }

  ${isMobile &&
  css`
    min-width: ${(props) => (props.showText ? "243px" : "60px")};
    max-width: ${(props) => (props.showText ? "243px" : "60px")};
    padding: 0 8px;
  `}

  @media ${mobile} {
    display: ${(props) => (props.articleOpen ? "flex" : "none")};
    flex-direction: column;
    min-width: 100%;
    width: 100%;
    position: fixed;
    margin: 0;

    height: calc(100% - 64px) !important;

    margin-top: 16px;
    padding: 0 8px;
  }

  ${isMobileOnly &&
  css`
    display: ${(props) => (props.articleOpen ? "flex" : "none")} !important;
    flex-direction: column;

    min-width: 100% !important;
    width: 100%;
    position: fixed;
    margin: 0;

    margin: 0;
    padding: 0;
    margin-top: ${(props) =>
      props.isBannerVisible ? "-16px" : "64px"} !important;
    height: calc(100% - 64px) !important;
  `}

  z-index: ${(props) =>
    props.showText && (isMobileOnly || isMobileUtils()) ? "230" : "205"};

  .article-body__scrollbar {
    .scroll-body {
      ${!isDesktop && "padding-top:  16px"};
      padding-right: 0px !important;

      @media ${mobile} {
        padding-bottom: 20px;
      }

      ${isMobileOnly &&
      css`
        padding-bottom: 20px;
      `}
    }
  }
`;

StyledArticle.defaultProps = { theme: Base };

const StyledArticleHeader = styled.div`
  padding: 16px 0 17px;
  margin-left: -1px;
  display: flex;
  justify-content: flex-start;
  align-items: center;

  @media ${tablet} {
    padding: 18px 0 19px;
    margin: 0;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};

    height: 61px;
    min-height: 61px;
    max-height: 61px;
    box-sizing: border-box;
  }

  @media ${mobile} {
    border-bottom: ${(props) => props.theme.catalog.header.borderBottom};
    padding: 12px 0 12px;
    // margin-bottom: 16px !important;
  }

  ${isTablet &&
  css`
    padding: 16px 0 17px;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};
    margin: 0;
  `}

  ${isMobileOnly &&
  css`
    border-bottom: ${(props) =>
      props.theme.catalog.header.borderBottom} !important;
    padding: 12px 0 12px !important;
    // margin-bottom: 16px !important;
  `}

  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

StyledArticleHeader.defaultProps = { theme: Base };

const StyledHeading = styled.div`
  margin: 0;
  padding: 0;
  cursor: pointer;

  @media ${tablet} {
    display: ${(props) => (props.showText ? "block" : "none")};
    margin-left: ${(props) => props.showText && "12px"};
  }

  ${isTablet &&
  css`
    display: ${(props) => (props.showText ? "block" : "none")};
    margin-left: ${(props) => props.showText && "12px"};
  `}

  @media ${mobile} {
    margin-left: 0;
  }

  ${isMobileOnly &&
  css`
    margin-left: 0 !important;
  `}
`;

const StyledIconBox = styled.div`
  display: none;
  align-items: center;
  height: 20px;

  @media ${tablet} {
    display: ${(props) => (props.showText ? "none" : "flex")};
  }

  @media ${mobile} {
    display: none;
  }

  ${isMobile &&
  css`
    display: flex !important;
  `}

  ${isMobileOnly &&
  css`
    display: none !important;
  `}
`;

const StyledMenuIcon = styled(MenuIcon)`
  display: block;
  width: 20px;
  height: 20px;

  cursor: pointer;

  path {
    fill: ${(props) => props.theme.catalog.header.iconFill};
  }
`;

StyledMenuIcon.defaultProps = { theme: Base };

const StyledArticleMainButton = styled.div`
  padding: 0px 0px 16px;
  max-width: 100%;

  @media ${tablet} {
    padding: 0;
    margin: 0;
  }

  ${isMobile &&
  css`
    padding: 0;
    margin: 0;
  `}
`;

const StyledControlContainer = styled.div`
  width: 17px;
  height: 17px;
  position: absolute;
  top: 37px;
  right: 10px;
  border-radius: 100px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 290;
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 17px;
  height: 17px;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

const StyledArticleProfile = styled.div`
  position: fixed;
  bottom: 0;
  padding: 16px 0;
  display: flex;
  align-items: center;
  flex-flow: row wrap;
  gap: 16px;
  width: ${(props) =>
    !props.tablet
      ? "211px"
      : props.tablet && props.showText
      ? "211px"
      : "44px"};

  ${(props) =>
    props.tablet &&
    css`
      padding: 14px 6px;
      justify-content: ${(props) => !props.showText && "center"};
    `}

  padding-left: 20px;
  margin-left: -20px;
  padding-right: 20px;
  margin-right: -20px;

  @media ${tablet} {
    padding-left: 8px;
    margin-left: -8px;
    padding-right: 8px;
    margin-right: -8px;

    ${(props) =>
      props.showText &&
      css`
        padding-left: 24px;
        margin-left: -24px;
        padding-right: 24px;
        margin-right: -24px;
      `}
  }

  border-top: ${(props) => props.theme.catalog.profile.borderTop};
  background-color: ${(props) => props.theme.catalog.profile.background};

  .userName {
    max-width: 109px;
    white-space: normal;
  }

  .option-button {
    margin-left: auto;

    .injected-svg {
      width: 16px;
      height: 16px;
    }

    .option-button-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 30px;
      height: 30px;
    }
  }
`;

StyledArticleProfile.defaultProps = { theme: Base };

export {
  StyledArticle,
  StyledArticleHeader,
  StyledHeading,
  StyledIconBox,
  StyledMenuIcon,
  StyledArticleMainButton,
  StyledControlContainer,
  StyledCrossIcon,
  StyledArticleProfile,
};
