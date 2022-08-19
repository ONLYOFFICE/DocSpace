import styled from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledComponent = styled.div`
  display: inline-flex;
  max-width: 575px;
  width: 100%;

  .menu {
    width: 251px;
    display: flex;
    flex-direction: column;
    padding: 21px 0px 17px;
    height: 100%;
    background: ${(props) =>
      props.themePreview === "Light" ? "#f8f9f9" : "#292929"};
    border-width: 1px;
    border-style: solid;
    border-radius: 16px 0px 0px 16px;
  }

  .section {
    width: 56%;
    border-width: 1px;
    border-style: solid;
    border-left-style: none;
    border-radius: 0px 16px 16px 0px;
    background: ${(props) =>
      props.themePreview === "Light" ? "#FFFFFF" : "#333333"};
  }

  .section-header {
    display: flex;
    align-items: flex-start;
    padding: 26px 0px 28px 20px;
  }

  .section-header-loader {
    padding-right: 17px;
    height: 16px;
  }

  .section-search {
    height: 30px;
    border-width: 1px;
    border-style: solid;
    border-radius: 3px 0px 0px 3px;
    border-right-style: none;
    margin: 0px 0px 24px 20px;
  }

  .section-search-loader {
    padding-top: 9px;
    padding-left: 8px;
  }

  .loader-search {
    margin-bottom: 2px;
  }

  .main-button-container {
    margin: 0px 20px 24px;
  }

  .main-button-preview {
    cursor: auto;
    background-color: ${(props) => props.colorPreview};
  }

  .color-badge > div {
    background-color: ${(props) =>
      props.themePreview === "Dark" && props.selectThemeId === 7
        ? "#FFFFFF"
        : props.colorPreview} !important;
  }

  .color-badge > p {
    color: ${(props) => props.color} !important;
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

    padding: 10px 20px 0px;

    &:not(:last-child) {
      padding-bottom: 10px;
    }
  }

  .padding-right {
    height: 16px;
    padding-right: 8px;
  }

  .title-section {
    height: 12px;
    margin: 0px 20px 4px;
  }

  .menu-badge {
    padding-left: 93px;
    border: none;
    cursor: auto;
  }

  .select {
    background: ${(props) =>
      props.themePreview === "Light" ? "#f0f0f0" : "#333333"};
  }

  .section-tile {
    padding: 0px 20px 0px;
  }

  .border-color {
    border-color: ${(props) =>
      props.themePreview === "Light" ? "#d0d5da" : "#474747"};
  }

  .tile {
    border-width: 1px;
    border-style: solid;
    border-radius: 12px;
    margin-bottom: 15px;
  }

  .background {
    background: ${(props) =>
      props.themePreview === "Light" ? "#FFF" : "#292929"};
  }

  .tile-name {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 16px 22px 16px 16px;
    height: 30px;
  }

  .half {
    border-top-width: 1px;
    border-right-width: 1px;
    border-left-width: 1px;
    border-style: solid;
    border-bottom: none;
    border-radius: 12px 12px 0px 0px;
  }

  .action-button {
    width: 66px;
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

  .tile-icon {
    padding-right: 12px;
  }

  .section-badge {
    border: none;
    cursor: auto;
    padding-right: 10px;
  }

  .pin {
    padding-right: 4px;

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
`;

StyledComponent.defaultProps = { theme: Base };

export { StyledComponent };
