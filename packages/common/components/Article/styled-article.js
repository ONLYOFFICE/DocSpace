import styled, { css } from "styled-components";

import { isMobile, isMobileOnly, isTablet } from "react-device-detect";
import {
  mobile,
  tablet,
  isMobile as isMobileUtils,
} from "@docspace/components/utils/device";

import { Base } from "@docspace/components/themes";
import MenuIcon from "PUBLIC_DIR/images/menu.react.svg";
import CrossIcon from "PUBLIC_DIR/images/cross.react.svg";

const StyledArticle = styled.article`
  position: relative;
  overflow: hidden;
  background: ${(props) => props.theme.catalog.background};

  min-width: 251px;
  max-width: 251px;

  box-sizing: border-box;

  user-select: none;

  //padding: 0 20px;

  border-right: ${(props) => props.theme.catalog.verticalLine};

  @media ${tablet} {
    min-width: ${(props) => (props.showText ? "243px" : "60px")};
    max-width: ${(props) => (props.showText ? "243px" : "60px")};

    //padding: 0 8px;
  }

  ${isMobile &&
  css`
    min-width: ${(props) => (props.showText ? "243px" : "60px")};
    max-width: ${(props) => (props.showText ? "243px" : "60px")};
    //padding: 0 8px;
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
    //padding: 0 8px;
  }

  ${isMobileOnly &&
  css`
    display: ${(props) => (props.articleOpen ? "flex" : "none")} !important;
    flex-direction: column;

    min-width: 100% !important;
    width: 100%;
    position: fixed;
    margin: 0;
    padding: 0;
    top: 64px;
    height: calc(100% - 64px) !important;

    border-right: none;
  `}

  z-index: ${(props) =>
    props.showText && (isMobileOnly || isMobileUtils()) ? "230" : "205"};

  .article-body__scrollbar {
    ${isMobileOnly &&
    css`
      margin-top: 32px !important;
    `}

    .scroll-body {
      overflow-x: hidden !important;
      height: ${(props) =>
        `calc(100% - ${props.$withMainButton ? "200px" : "150px"})`};
      padding: 0 20px !important;

      @media ${tablet} {
        height: calc(100% - 150px);
        padding: 0 8px !important;
      }

      ${isTablet &&
      css`
        padding: 0 8px !important;
      `}

      @media ${mobile} {
        height: calc(100% - 20px) !important;
        padding-bottom: 20px;
      }

      ${isMobileOnly &&
      css`
        height: calc(100% - 20px) !important;
        padding-bottom: 20px;
      `}
    }
  }
`;

StyledArticle.defaultProps = { theme: Base };

const StyledArticleHeader = styled.div`
  height: 24px;
  padding: 24px 21px 21px 20px;
  margin: 0;
  display: flex;
  justify-content: flex-start;
  align-items: center;

  @media ${tablet} {
    padding: 18px 8px 19px;
    margin: 0;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};

    height: 61px;
    min-height: 61px;
    max-height: 61px;
    box-sizing: border-box;
  }

  ${isTablet &&
  css`
    padding: 18px 8px 19px;
    margin: 0;
    justify-content: ${(props) => (props.showText ? "flex-start" : "center")};

    height: 61px;
    min-height: 61px;
    max-height: 61px;
    box-sizing: border-box;
  `}

  @media ${mobile} {
    border-bottom: ${(props) => props.theme.catalog.header.borderBottom};
    padding: 12px 0 12px;
  }

  ${isMobileOnly &&
  css`
    border-bottom: ${(props) =>
      props.theme.catalog.header.borderBottom} !important;
    padding: 12px 0 12px !important;
  `}

  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

StyledArticleHeader.defaultProps = { theme: Base };

const StyledHeading = styled.div`
  height: 24px;

  margin: 0;
  padding: 0;
  cursor: pointer;

  img.logo-icon_svg {
    height: 24px;
    width: 211px;
  }

  .logo-icon_svg {
    svg {
      path:last-child {
        fill: ${(props) => props.theme.client.home.logoColor};
      }
    }
  }

  @media ${tablet} {
    display: ${(props) => (props.showText ? "block" : "none")};
    margin-left: 9px;
  }

  ${isTablet &&
  css`
    display: ${(props) => (props.showText ? "block" : "none")};
    margin-left: 9px !important;
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
  cursor: pointer;
  display: none;
  align-items: center;
  height: 20px;

  img {
    height: 24px;
  }

  @media ${tablet} {
    display: ${(props) => (props.showText ? "none" : "flex")};
  }

  ${isTablet &&
  css`
    display: ${(props) => (props.showText ? "none" : "flex")};
  `}

  @media ${mobile} {
    display: none;
  }

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
  padding: 16px 20px;
  height: 40px !important;
  display: flex;
  align-items: center;
  justify-content: center;

  border-top: ${(props) => props.theme.catalog.profile.borderTop};
  border-right: ${(props) => props.theme.catalog.verticalLine}
  background-color: ${(props) => props.theme.catalog.profile.background};

  @media ${tablet} {
    padding: 16px 14px;
  }

  ${
    isTablet &&
    css`
      padding: 16px 14px;
    `
  }

  .profile-avatar {
    cursor: pointer;
  }

  .option-button {
    margin-left: auto;
    height: 32px;
    width: 32px;

    .injected-svg {
      width: 16px;
      height: 16px;
    }

    .option-button-icon {
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }
`;

StyledArticleProfile.defaultProps = { theme: Base };

const StyledUserName = styled.div`
  display: flex;
  flex-direction: ${(props) => (props.length > 18 ? "column" : "row")};
  max-width: 131px;
  min-width: 131px;
  padding-left: 12px;
  cursor: pointer;
`;

const StyledProfileWrapper = styled.div`
  z-index: 209;
  position: fixed;
  bottom: 0;
  left: 0;
  min-width: 251px;
  max-width: 251px;
  background-color: ${(props) => props.theme.catalog.profile.background};

  @media ${tablet} {
    min-width: ${(props) => (props.showText ? "243px" : "60px")};
    max-width: ${(props) => (props.showText ? "243px" : "60px")};
  }

  ${isTablet &&
  css`
    min-width: ${(props) => (props.showText ? "243px" : "60px")};
    max-width: ${(props) => (props.showText ? "243px" : "60px")};
  `}
`;
const StyledArticlePaymentAlert = styled.div`
  border: ${(props) =>
    props.isFreeTariff
      ? props.theme.catalog.paymentAlert.border
      : props.theme.catalog.paymentAlert.warningBorder};

  border-radius: 6px;
  margin: 32px 0px;
  padding: 12px;
  cursor: pointer;
  display: grid;

  grid-template-columns: 1fr 16px;

  .article-payment_border {
    color: ${(props) =>
      props.isFreeTariff
        ? props.theme.catalog.paymentAlert.color
        : props.theme.catalog.paymentAlert.warningColor};
  }
`;

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
  StyledUserName,
  StyledProfileWrapper,
  StyledArticlePaymentAlert,
};
