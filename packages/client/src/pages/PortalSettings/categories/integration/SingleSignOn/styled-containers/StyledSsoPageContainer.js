import styled from "styled-components";
import { UnavailableStyles } from "../../../../utils/commonSettingsStyles";

const StyledSsoPage = styled.div`
  box-sizing: border-box;
  outline: none;
  padding-top: 5px;

  .intro-text {
    width: 100%;
    max-width: 700px;
    margin-bottom: 18px;
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
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
        margin-left: 4px;
        cursor: auto;
      }
    }
  }

  .tooltip-button,
  .icon-button {
    padding: 0 5px;
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

  .combo-button-label {
    max-width: 100%;
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

  ${(props) => !props.isSettingPaid && UnavailableStyles}
`;

export default StyledSsoPage;
