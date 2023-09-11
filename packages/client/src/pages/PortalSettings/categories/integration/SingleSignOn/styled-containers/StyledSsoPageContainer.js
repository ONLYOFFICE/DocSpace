import styled, { css } from "styled-components";
import { UnavailableStyles } from "../../../../utils/commonSettingsStyles";

const StyledSsoPage = styled.div`
  box-sizing: border-box;
  outline: none;
  padding-top: 5px;

  .intro-text {
    width: 100%;
    max-width: 700px;
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
    padding-bottom: 18px;
  }

  .toggle {
    position: static;
    margin-top: 1px;
  }

  .toggle-caption {
    display: flex;
    flex-direction: column;
    gap: 4px;
    .toggle-caption_title {
      display: flex;
      .toggle-caption_title_badge {
        ${(props) =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                margin-right: 4px;
              `
            : css`
                margin-left: 4px;
              `}
        cursor: auto;
      }
    }
  }

  .field-input {
    ::placeholder {
      font-size: 13px;
      font-weight: 400;
    }
  }

  .field-label-icon {
    align-items: center;
    margin-bottom: 4px;
    max-width: 350px;
  }

  .field-label {
    display: flex;
    align-items: center;
    height: auto;
    font-weight: 600;
    line-height: 20px;
    overflow: visible;
    white-space: normal;
  }

  .xml-input {
    .field-label-icon {
      margin-bottom: 8px;
      max-width: 350px;
    }

    .field-label {
      font-weight: 400;
    }
  }

  .or-text {
    margin: 0 24px;
  }

  .radio-button-group {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
  }

  .combo-button-label {
    max-width: 100%;
  }

  .upload-button {
    height: 32px;
    width: 45px;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 9px;
          `
        : css`
            margin-left: 9px;
          `}
    overflow: inherit;
  }

  .save-button {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 8px;
          `
        : css`
            margin-right: 8px;
          `}
  }

  .download-button {
    width: fit-content;
  }

  .service-provider-settings {
    display: ${(props) => (!props.hideSettings ? "none" : "block")};
  }

  .sp-metadata {
    display: ${(props) => (!props.hideMetadata ? "none" : "block")};
  }

  .advanced-block {
    margin: 24px 0;

    .field-label {
      font-size: 15px;
      font-weight: 600;
    }
  }

  .metadata-field {
    display: flex;
    flex-direction: column;
    gap: 4px;
    margin-bottom: 16px;
    max-width: 350px;

    .input {
      width: 350px;
    }

    .label > div {
      display: inline-flex;
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-right: 4px;
            `
          : css`
              margin-left: 4px;
            `}
    }
  }

  ${(props) => !props.isSettingPaid && UnavailableStyles}
`;

export default StyledSsoPage;
