import styled from "styled-components";
import { Base } from "@docspace/components/themes";

const WhiteLabelWrapper = styled.div`
  .subtitle {
    margin-top: 5px;
    margin-bottom: 20px;
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
  }

  .paid-badge {
    cursor: auto;
  }

  .header-container {
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .wl-subtitle {
    margin-top: 8px;
    margin-bottom: 20px;
  }

  .wl-helper {
    display: flex;
    gap: 4px;
    align-items: center;
    margin-bottom: 16px;
  }

  .use-as-logo {
    margin-top: 12px;
    margin-bottom: 24px;
  }

  .input {
    max-width: 350px;
  }

  .logos-container {
    display: flex;
    flex-direction: column;
    gap: 40px;
  }

  .logo-wrapper {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  .logos-wrapper {
    display: flex;
    gap: 20px;
  }

  .logos-login-wrapper {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .logos-editor-wrapper {
    display: flex;
    gap: 8px;
    margin-bottom: 8px;
  }

  .logo-item {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-bottom: 10px;
  }

  .border-img {
    border: ${(props) =>
      props.theme.client.settings.common.whiteLabel.borderImg};
    box-sizing: content-box;
  }

  .logo-header {
    width: 211px;
    height: 24px;
    padding: 22px 20px;
  }

  .logo-compact {
    width: 28px;
    height: 28px;
    padding: 15px;
  }

  .logo-big {
    width: 384px;
    height: 42px;
    padding: 12px 20px;
  }

  .logo-about {
    width: 211px;
    height: 24px;
    padding: 12px 20px;
  }

  .logo-favicon {
    width: 30px;
    height: 30px;
    margin-bottom: 5px;
  }

  .logo-docs-editor {
    width: 154px;
    height: 27px;
    padding: 6px 9px 7px 9px;
  }

  .logo-embedded-editor {
    width: 154px;
    height: 27px;
    padding: 5px 8px 6px 8px;
    margin-bottom: 8px;
  }

  .background-green {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.greenBackgroundColor};
  }

  .background-blue {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.blueBackgroundColor};
  }

  .background-orange {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.orangeBackgroundColor};
  }

  .background-light {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.backgroundColorLight};
  }

  .background-dark {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.backgroundColorDark};
  }

  .background-white {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.backgroundColorWhite};
  }

  .hidden {
    display: none;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }
`;

WhiteLabelWrapper.defaultProps = { theme: Base };

export default WhiteLabelWrapper;
