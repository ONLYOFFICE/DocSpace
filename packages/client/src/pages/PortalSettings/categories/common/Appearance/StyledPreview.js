import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledComponent = styled.div`
  display: inline-flex;
  width: 100%;

  .menu {
    width: ${props => (props.isViewTablet ? "61px" : "251px")};
    display: flex;
    flex-direction: column;
    padding: ${props =>
      props.isViewTablet ? "15px 0px 0px" : "21px 0px 17px"};

    height: 100%;
    background: ${props =>
      props.themePreview === "Light" ? "#f8f9f9" : "#292929"};
    ${props =>
      props.withBorder &&
      css`
        border-width: 1px;
        border-style: solid;
        border-radius: 16px 0px 0px 16px;
      `}
  }

  .tablet-header {
    padding: 0 15px 20px 15px;
  }

  .tablet-category {
    padding: 0 20px;
  }

  .line {
    width: 20px;
    height: 1px;
    background: ${props =>
      props.themePreview === "Light" ? "#eceef1" : "#474747"};
    margin: 0 20px 31px 20px;
  }

  .tablet-category-notice {
    padding: 20px 16px 20px 16px;

    circle {
      fill: ${props => props.colorPreview};
      stroke: ${props => props.themePreview === "Dark" && "#292929"};
    }
  }

  .bottom {
    padding-bottom: 31px;
  }

  .tablet-half {
    width: 20px;
    height: 10px;
    padding-top: 24px;
  }

  .section-flex-tablet {
    display: ${props => props.isViewTablet && "flex"};
    justify-content: ${props => props.isViewTablet && "space-between"};
  }

  .tile-half {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 16px;
            border-left: none !important;
          `
        : css`
            margin-left: 16px;
            border-right: none !important;
          `}
    width: 44% !important;
    border-radius: 12px 0 0 12px !important;
  }

  .section {
    position: relative;
    width: ${props => (props.isViewTablet ? "100%" : "56%")};
    ${props =>
      props.withBorder &&
      css`
        border-width: 1px;
        border-style: solid;
        border-left-style: none;
        border-radius: 0px 16px 16px 0px;
      `}
    background: ${props =>
      props.themePreview === "Light" ? "#FFFFFF" : "#333333"};
  }

  .section-header {
    display: flex;
    align-items: flex-start;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding: 26px 20px 28px 0px;
          `
        : css`
            padding: 26px 0px 28px 20px;
          `}
  }

  .section-header-loader {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 17px;
          `
        : css`
            padding-right: 17px;
          `}
    height: 16px;
  }

  .section-search {
    height: 30px;
    border-width: 1px;
    border-style: solid;
    border-radius: 3px 0px 0px 3px;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin: 0px 20px 24px 0px;
            border-left-style: none;
          `
        : css`
            margin: 0px 0px 24px 20px;
            border-right-style: none;
          `}
  }

  .section-search-loader {
    padding-top: 9px;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-right: 8px;
          `
        : css`
            padding-left: 8px;
          `}
  }

  .loader-search {
    margin-bottom: 2px;
  }

  .main-button-container {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin: 0px 0px 23px 20px;
          `
        : css`
            margin: 0px 20px 23px 0;
          `}
  }

  .main-button-preview {
    cursor: auto;
    background-color: ${props => props.colorPreview};
    border-radius: 3px;

    &:active {
      background-color: ${props => props.colorPreview} !important;
      opacity: none !important;
      filter: none !important;
    }
  }

  .color-badge rect {
    fill: ${props =>
      props.themePreview === "Dark" && props.selectThemeId === 7
        ? "#FFFFFF"
        : props.colorPreview} !important;
  }

  .color-loaders rect {
    fill: ${props =>
      props.themePreview === "Light"
        ? `${props.colorPreview} !important`
        : `#FFFFFF !important`};
  }

  .menu-section {
    &:not(:last-child) {
      padding-bottom: 26px;
    }
  }

  .header {
    margin: 0px 20px 23px 20px;
    height: 24px;
  }

  .loaders-theme {
    background-color: ${props =>
      props.themePreview === "Light" ? "#FFF" : "#858585"};
    border-radius: 3px;
  }

  .flex {
    display: flex;
    align-items: center;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding: 10px 0px 0px 32px;
          `
        : css`
            padding: 10px 32px 0px;
          `}

    &:not(:last-child) {
      padding-bottom: 10px;
    }
  }

  .padding-right {
    height: 16px;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 8px;
          `
        : css`
            padding-right: 8px;
          `}
  }

  .title-section {
    height: 12px;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin: 0px 0px 4px 32px;
          `
        : css`
            margin: 0px 32px 4px;
          `}
  }

  .menu-badge {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-right: 93px;
          `
        : css`
            padding-left: 93px;
          `}
    border: none;
    cursor: auto;
  }

  .select {
    padding-top: 9px;
    padding-bottom: 9px !important;
    background: ${props =>
      props.themePreview === "Light" ? "#f0f0f0" : "#333333"};
  }

  .section-tile {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding: ${props =>
              props.isViewTablet ? "0 20px 0 0 " : "0 0 0 20px"};
          `
        : css`
            padding: ${props =>
              props.isViewTablet ? "0 0 0 20px" : "0 20px 0"};
          `}
  }

  .border-color {
    border-color: ${props =>
      props.themePreview === "Light" ? "#d0d5da" : "#474747"};
  }

  .tile {
    border-width: 1px;
    border-style: solid;
    border-radius: 12px;
    margin-bottom: 16px;
    width: ${props => props.isViewTablet && "64%"};
  }

  .background {
    background: ${props =>
      props.themePreview === "Light" ? "#FFF" : "#292929"};
  }

  .tile-name {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 16px 16px 14px 16px;
    height: 30px;
  }

  .tablet-tile-name {
    width: 44% !important;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 16px;
          `
        : css`
            margin-left: 16px;
          `}
    border-right: none !important;
    border-radius: 12px 0 16px 0 !important;
  }

  .only-tile-name {
    width: ${props => props.isViewTablet && "66%"};
    border-top-width: 1px;
    border-right-width: 1px;
    border-left-width: 1px;
    border-style: solid;
    border-bottom: none;
    border-radius: 12px 12px 0px 0px;
  }

  .action-button {
    width: 72px;
    display: flex;
    justify-content: flex-end;
    align-items: center;
  }

  .tile-tag {
    display: flex;
    border-top-width: 1px;
    border-top-style: solid;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding: 16px 16px 16px 0px;
          `
        : css`
            padding: 16px 0px 16px 16px;
          `}
  }

  .tile-container {
    display: flex;
    align-items: center;
  }

  .tile-icon {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 12px;
          `
        : css`
            padding-right: 12px;
          `}
  }

  .section-badge {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 12px;
          `
        : css`
            padding-right: 12px;
          `}
  }

  .pin {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 14px;
          `
        : css`
            padding-right: 14px;
          `}

    path {
      fill: ${props =>
        props.themePreview === "Light"
          ? `${props.colorPreview} !important`
          : `#FFFFFF !important`};
    }
  }

  .menu-button > div {
    cursor: auto;
  }

  .main-button_text {
    color: #ffffff !important;
  }
`;

const StyledFloatingButton = styled.div`
  bottom: 24px;
  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          left: 24px;
        `
      : css`
          right: 24px;
        `}
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background-color: ${props => props.colorPreview};
  text-align: center;
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
`;

StyledFloatingButton.defaultProps = { theme: Base };

const IconBox = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;

  svg {
    path {
      fill: ${props => props.colorCheckImg};
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

export { StyledComponent, StyledFloatingButton, IconBox };
