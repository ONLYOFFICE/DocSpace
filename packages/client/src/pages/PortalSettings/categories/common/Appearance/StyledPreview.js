import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";
import {
  getCorrectBorderRadius,
  getCorrectFourValuesStyle,
} from "@docspace/components/utils/rtlUtils";

const StyledComponent = styled.div`
  display: inline-flex;
  width: 100%;

  .menu {
    width: ${(props) => (props.isViewTablet ? "61px" : "251px")};
    display: flex;
    flex-direction: column;
    padding: ${(props) =>
      props.isViewTablet ? "15px 0px 0px" : "21px 0px 17px"};

    height: 100%;
    background: ${(props) =>
      props.themePreview === "Light" ? "#f8f9f9" : "#292929"};
    ${(props) =>
      props.withBorder &&
      css`
        border-width: 1px;
        border-style: solid;
        border-radius: ${getCorrectBorderRadius(
          "16px 0px 0px 16px",
          props.theme.interfaceDirection
        )};
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
    background: ${(props) =>
      props.themePreview === "Light" ? "#eceef1" : "#474747"};
    margin: 0 20px 31px 20px;
  }

  .tablet-category-notice {
    padding: 20px 16px 20px 16px;

    ${({ theme }) =>
      theme.interfaceDirection === "rtl" && "transform: scaleX(-1);"}

    circle {
      fill: ${(props) => props.colorPreview};
      stroke: ${(props) => props.themePreview === "Dark" && "#292929"};
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
    display: ${(props) => props.isViewTablet && "flex"};
    justify-content: ${(props) => props.isViewTablet && "space-between"};
  }

  .tile-half {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 16px;
            border-left: none !important;
            border-radius: 0px 12px 12px 0 !important;
          `
        : css`
            margin-left: 16px;
            border-right: none !important;
            border-radius: 12px 0 0 12px !important;
          `}
    width: 44% !important;
  }

  .section {
    position: relative;
    width: ${(props) => (props.isViewTablet ? "100%" : "56%")};
    ${(props) =>
      props.withBorder &&
      css`
        border-width: 1px;
        border-style: solid;
        ${(props) =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                margin-right: 12px;
                border-radius: 16px 0px 0px 16px;
                border-right-style: none;
              `
            : css`
                margin-left: 12px;
                border-radius: 0px 16px 16px 0px;
                border-left-style: none;
              `}
      `}
    background: ${(props) =>
      props.themePreview === "Light" ? "#FFFFFF" : "#333333"};
  }

  .section-header {
    display: flex;
    align-items: flex-start;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding: 26px 20px 28px 0px;
          `
        : css`
            padding: 26px 0px 28px 20px;
          `}
  }

  .section-header-loader {
    ${(props) =>
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
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin: 0px 20px 24px 0px;
            border-left-style: none;
            border-radius: 0px 3px 3px 0px;
          `
        : css`
            margin: 0px 0px 24px 20px;
            border-right-style: none;
            border-radius: 3px 0px 0px 3px;
          `}
  }

  .section-search-loader {
    padding-top: 9px;
    ${(props) =>
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
    margin: 0px 20px 23px;
  }

  .main-button-preview {
    cursor: auto;
    background-color: ${(props) => props.colorPreview};
    border-radius: 3px;

    &:active {
      background-color: ${(props) => props.colorPreview} !important;
      opacity: none !important;
      filter: none !important;
    }
  }

  .color-badge rect {
    fill: ${(props) =>
      props.themePreview === "Dark" && props.selectThemeId === 7
        ? "#FFFFFF"
        : props.colorPreview} !important;
  }

  .color-loaders rect {
    fill: ${(props) =>
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
    background-color: ${(props) =>
      props.themePreview === "Light" ? "#FFF" : "#858585"};
    border-radius: 3px;
  }

  .flex {
    display: flex;
    align-items: center;
    padding: 10px 32px 0px;

    &:not(:last-child) {
      padding-bottom: 10px;
    }
  }

  .padding-right {
    height: 16px;
    ${(props) =>
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
    margin: 0px 32px 4px;
  }

  .menu-badge {
    ${(props) =>
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
    background: ${(props) =>
      props.themePreview === "Light" ? "#f0f0f0" : "#333333"};
  }

  .section-tile {
    padding: ${({ isViewTablet, theme }) => {
      const value = isViewTablet ? "0 0 0 20px" : "0 20px 0";

      return getCorrectFourValuesStyle(value, theme.interfaceDirection);
    }};
  }

  .border-color {
    border-color: ${(props) =>
      props.themePreview === "Light" ? "#d0d5da" : "#474747"};
  }

  .tile {
    border-width: 1px;
    border-style: solid;
    border-radius: 12px;
    margin-bottom: 16px;
    width: ${(props) => props.isViewTablet && "64%"};
  }

  .background {
    background: ${(props) =>
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
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 16px;
            border-left: none !important;
            border-radius: 0 12px 0 16px !important;
          `
        : css`
            margin-left: 16px;
            border-right: none !important;
            border-radius: 12px 0 16px 0 !important;
          `}
  }

  .only-tile-name {
    width: ${(props) => props.isViewTablet && "66%"};
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
    padding: ${({ theme }) =>
      getCorrectFourValuesStyle(
        `16px 0px 16px 16px`,
        theme.interfaceDirection
      )};
  }

  .tile-container {
    display: flex;
    align-items: center;
  }

  .tile-icon {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 12px;
          `
        : css`
            padding-right: 12px;
          `}
  }

  .section-badge {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 12px;
          `
        : css`
            padding-right: 12px;
          `}
  }

  .pin {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 14px;
          `
        : css`
            padding-right: 14px;
          `}

    path {
      fill: ${(props) =>
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
  ${(props) =>
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
  background-color: ${(props) => props.colorPreview};
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
      fill: ${(props) => props.colorCheckImg};
    }
  }
`;

const StyledMobilePreview = styled.div`
  height: 293px;
  border-radius: 16px;
  padding: 0px 16px;
  background: ${({ themePreview }) =>
    themePreview === "Light" ? "#FFFFFF" : "#333333"};

  border: ${({ themePreview }) =>
    themePreview === "Light" ? "1px solid #d0d5da" : "1px solid transparent"};

  .section-search {
    height: 30px;
    display: flex;
    align-items: center;
    border: 1px solid;
    border-radius: 3px;
    padding-left: 8px;
  }

  .main-button-preview {
    cursor: auto;
    background-color: ${(props) => props.colorPreview};
    border-radius: 3px;

    &:active {
      background-color: ${(props) => props.colorPreview} !important;
      opacity: none !important;
      filter: none !important;
    }
  }

  .color-badge rect {
    fill: ${(props) =>
      props.themePreview === "Dark" && props.selectThemeId === 7
        ? "#FFFFFF"
        : props.colorPreview} !important;
  }

  .color-loaders rect {
    fill: ${(props) =>
      props.themePreview === "Light"
        ? `${props.colorPreview} !important`
        : `#FFFFFF !important`};
  }

  .menu-section {
    &:not(:last-child) {
      padding-bottom: 26px;
    }
  }

  .loaders-theme {
    background-color: ${(props) =>
      props.themePreview === "Light" ? "#FFF" : "#545454"};
    border-radius: 3px;
  }

  .loaders-tile-theme {
    background: ${(props) =>
      props.themePreview === "Light" ? "#F1F1F1" : "#333333"};

    border-radius: 3px;
  }

  .loaders-tile-text-theme {
    background: ${(props) =>
      props.themePreview === "Light" ? "#D0D5DA" : "#858585"};

    border-radius: 3px;
  }

  .loaders-theme-avatar {
    background-color: ${(props) =>
      props.themePreview === "Light" ? "#FFF" : "#545454"};
    border-radius: 50px;
  }

  .border-color {
    border-color: ${(props) =>
      props.themePreview === "Light" ? "#d0d5da" : "#474747"};
  }

  .tile {
    border-width: 1px;
    border-style: solid;
    border-radius: 12px;
    width: ${(props) => props.isViewTablet && "64%"};
    margin-top: 24px;
  }

  .background {
    background: ${(props) =>
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
    margin-left: 16px;
    border-right: none !important;
    border-radius: 12px 0 16px 0 !important;
  }

  .only-tile-name {
    width: ${(props) => props.isViewTablet && "66%"};
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
    padding: 16px 0px 16px 16px;
  }

  .tile-container {
    display: flex;
    align-items: center;
  }

  .pin {
    padding-right: 14px;

    path {
      fill: ${(props) =>
        props.themePreview === "Light"
          ? `${props.colorPreview} !important`
          : `#FFFFFF !important`};
    }
  }

  .menu-button > div {
    cursor: auto;
  }

  .preview_mobile-header {
    height: 48px;
    display: grid;
    align-items: center;
    grid-template-columns: 34px 1fr 32px;
    gap: 16px;

    margin: 0 -16px;
    padding: 0 16px;

    background: ${({ themePreview }) =>
      themePreview === "Light" ? "#FFFFFF" : "#282828"};

    border-radius: 16px 16px 0px 0px;
  }

  .preview_mobile-navigation {
    height: 53px;
    display: flex;
    align-items: center;

    .header {
      width: 45%;
    }
  }

  .color-badge rect {
    fill: ${({ themePreview, selectThemeId, colorPreview }) =>
      themePreview === "Dark" && selectThemeId === 7
        ? "#FFFFFF"
        : colorPreview} !important;
  }
  .section-badge {
    padding-right: 12px;
  }

  .tile-icon {
    padding-right: 12px;
  }

  .floating-button {
    position: relative;
    margin-left: auto;
    right: 0px;
    bottom: 48px;
  }

  .icon-button_svg {
    svg {
      path {
        fill: #a3a9ae;
      }
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

export { StyledComponent, StyledFloatingButton, IconBox, StyledMobilePreview };
