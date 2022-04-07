import styled, { css } from "styled-components";

import { isMobile, isMobileOnly, isTablet } from "react-device-detect";
import {
  mobile,
  tablet,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
  isDesktop as isDesktopUtils,
} from "@appserver/components/utils/device";

import Heading from "@appserver/components/heading";
import { Base } from "@appserver/components/themes";

import MenuIcon from "@appserver/components/public/static/images/menu.react.svg";
import CrossIcon from "@appserver/components/public/static/images/cross.react.svg";

const StyledArticle = styled.article`
  position: relative;
  overflow: hidden;
  background: ${(props) => props.theme.catalog.background};

  min-width: 256px;
  max-width: 256px;

  @media ${tablet} {
    min-width: ${(props) => (props.showText ? "240px" : "52px")};
    max-width: ${(props) => (props.showText ? "240px" : "52px")};
  }

  ${isMobile &&
  css`
    min-width: ${(props) => (props.showText ? "240px" : "52px")};
    max-width: ${(props) => (props.showText ? "240px" : "52px")};
  `}

  @media ${mobile} {
    display: ${(props) => (props.articleOpen ? "flex" : "none")};
    min-width: 100vw;
    width: 100vw;
    height: calc(100vh - 64px) !important;
    margin: 0;
    padding: 0;
    padding-bottom: 0px;
  }

  ${isMobileOnly &&
  css`
    display: ${(props) => (props.articleOpen ? "flex" : "none")} !important;
    min-width: 100vw !important;
    width: 100vw;
    position: fixed;
    margin-top: 64px !important;
    height: calc(100vh - 64px) !important;
    margin: 0;
    padding: 0;
    padding-bottom: 0px;
  `}

  @media ${mobile} {
    position: fixed;
    margin-top: 16px;
    height: calc(100vh - 64px) !important;
    z-index: 400;
  }

  z-index: ${(props) =>
    props.showText && (isMobileOnly || isMobileUtils()) ? "205" : "100"};

  .resizable-block {
    overflow: hidden;

    display: flex;
    flex-direction: column;

    min-width: ${(props) => (props.showText ? "256px" : "52px")};
    width: ${(props) => (props.showText ? "256px" : "52px")};

    height: 100% !important;

    background: ${(props) => props.theme.catalog.background};

    padding-bottom: 0px;

    .resizable-border {
      div {
        cursor: ew-resize !important;
      }
    }
    @media ${tablet} {
      min-width: ${(props) => (props.showText ? "240px" : "52px")};
      max-width: ${(props) => (props.showText ? "240px" : "52px")};
      .resizable-border {
        display: none;
      }
    }

    @media ${mobile} {
      display: ${(props) => (props.articleOpen ? "flex" : "none")};
      min-width: 100vw;
      width: 100vw;
      height: calc(100vh - 64px) !important;
      margin: 0;
      padding: 0;
      padding-bottom: 0px;
    }

    ${isMobile &&
    css`
      min-width: ${(props) => (props.showText ? "240px" : "52px")};
      max-width: ${(props) => (props.showText ? "240px" : "52px")};
      .resizable-border {
        display: none;
      }
    `}

    ${isMobileOnly &&
    css`
      display: ${(props) => (props.articleOpen ? "flex" : "none")};
      min-width: 100vw !important;
      width: 100vw;
      height: calc(100vh - 64px) !important;
      margin: 0;
      padding: 0;
      padding-bottom: 0px;
    `}
  }

  .article-body__scrollbar {
    .scroll-body {
      padding-right: 0px !important;
    }
  }
`;

StyledArticle.defaultProps = { theme: Base };

const StyledArticleHeader = styled.div`
  padding: 11px 20px 14px;
  margin-left: -1px;
  display: flex;
  justify-content: flex-start;
  align-items: center;

  @media ${tablet} {
    padding: 16px 16px 17px;
    margin: 0;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};
  }

  @media ${mobile} {
    border-bottom: ${(props) => props.theme.catalog.header.borderBottom};
    padding: 12px 16px 12px;
    margin-bottom: 16px !important;
  }

  ${isTablet &&
  css`
    padding: 16px 16px 17px;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};
    margin: 0;
  `}

  ${isMobileOnly &&
  css`
    border-bottom: ${(props) =>
      props.theme.catalog.header.borderBottom} !important;
    padding: 12px 16px 12px !important;
    margin-bottom: 16px !important;
  `}
`;

StyledArticleHeader.defaultProps = { theme: Base };

const StyledHeading = styled(Heading)`
  margin: 0;
  padding: 0;
  font-weight: bold;
  line-height: 28px;
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
  height: 28px;

  @media ${tablet} {
    display: flex;
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
  padding: 0px 20px 16px;
  max-width: 216px;

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
  background: ${(props) => props.theme.catalog.control.background};
  width: 24px;
  height: 24px;
  position: absolute;
  top: 30px;
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
  width: 12px;
  height: 12px;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
`;

StyledCrossIcon.defaultProps = { theme: Base };

export {
  StyledArticle,
  StyledArticleHeader,
  StyledHeading,
  StyledIconBox,
  StyledMenuIcon,
  StyledArticleMainButton,
  StyledControlContainer,
  StyledCrossIcon,
};
