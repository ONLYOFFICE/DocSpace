import styled from "styled-components";
import { Base } from "@docspace/components/themes";

const WhiteLabelWrapper = styled.div`
  .subtitle {
    margin-top: 5px;
    margin-bottom: 20px;
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
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.backgroundColor};
  }

  .logo-compact {
    width: 26px;
    height: 26px;
    padding: 16px;
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
    width: 32px;
    height: 32px;
  }

  .logo-docs-editor {
    width: 86px;
    height: 20px;
    padding: 10px;
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

  .hidden {
    display: none;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }
`;

WhiteLabelWrapper.defaultProps = { theme: Base };

export default WhiteLabelWrapper;
