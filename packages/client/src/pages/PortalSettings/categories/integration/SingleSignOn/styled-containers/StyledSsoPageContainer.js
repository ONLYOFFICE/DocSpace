import styled from "styled-components";

const StyledSsoPage = styled.div`
  box-sizing: border-box;
  outline: none;
  max-width: 700px;
  padding-top: 5px;

  .intro-text {
    margin-bottom: 18px;
    max-width: 700px;
  }

  .toggle {
    position: static;
    margin-top: 1px;
  }

  .toggle-caption {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .tooltip-button,
  .icon-button {
    padding: 0 5px;
  }

  .hide-button {
    margin-left: 12px;
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
    margin-left: 24px;
  }

  .combo-button-label {
    max-width: 100%;
  }

  .checkbox-input {
    margin: 6px 8px 6px 0;
  }

  .upload-button {
    height: 32px;
    width: 45px;
    margin-left: 9px;
    overflow: inherit;
  }

  .save-button {
    margin-right: 8px;
  }

  .download-button {
    width: fit-content;
  }

  .xml-upload-file {
    .text-input {
      display: none;
    }

    .icon {
      position: static;
    }
  }

  .separator {
    margin: 24px 0;
    height: 1px;
    border: none;
    background-color: #eceef1;
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
  }
`;

export default StyledSsoPage;
