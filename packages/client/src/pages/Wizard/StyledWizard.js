import styled, { css } from "styled-components";
import { tablet, hugeMobile } from "@docspace/components/utils/device";
import { isIOS, isFirefox, isMobileOnly } from "react-device-detect";
import BackgroundPatternReactSvgUrl from "PUBLIC_DIR/images/background.pattern.react.svg?url";

export const Wrapper = styled.div`
  height: ${isIOS && !isFirefox ? "calc(var(--vh, 1vh) * 100)" : "100vh"};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: column;
  box-sizing: border-box;

  ${isMobileOnly &&
  css`
    height: auto;
    min-height: 100%;
    width: 100%;
  `}

  .bg-cover {
    background-image: url("${BackgroundPatternReactSvgUrl}");
    background-repeat: no-repeat;
    background-attachment: fixed;
    background-size: cover;
    position: fixed;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    z-index: -1;

    @media ${hugeMobile} {
      background-image: none;
    }
  }
`;

export const StyledContent = styled.div`
  min-height: 100vh;
  flex: 1 0 auto;
  flex-direction: column;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  -webkit-box-orient: vertical;
  -webkit-box-direction: normal;
`;

export const WizardContainer = styled.div`
  margin: 56px auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;

  @media ${tablet} {
    width: 100%;
    max-width: 480px;
  }

  @media ${hugeMobile} {
    margin-top: 32px;
    width: calc(100% - 32px);
  }

  .docspace-logo {
    display: flex;
    align-items: center;
    justify-content: center;
    padding-bottom: 40px;
  }

  .welcome-text {
    padding-bottom: 32px;
  }

  .form-header {
    padding-bottom: 24px;
  }

  .password-field-wrapper {
    width: 100%;
  }

  .wizard-field {
    width: 100%;
  }

  .password-field {
    margin: 0px !important;
  }

  .license-filed {
    width: 100%;
    margin-bottom: 20px;
  }
`;

export const StyledLink = styled.div`
  width: 100%;
  display: flex;
  gap: 8px;
  align-items: center;
  padding-bottom: 16px;
  padding-top: 8px;

  .generate-password-link {
    color: ${(props) => props.theme.client.wizard.generatePasswordColor};
  }

  .icon-button_svg {
    svg > g > path {
      fill: ${(props) => props.theme.client.wizard.generatePasswordColor};
    }
  }
`;

export const StyledInfo = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 59px 1fr;
  align-items: center;
  gap: 16px;

  margin-bottom: 4px;

  .machine-name {
    padding-bottom: 4px;
    padding-top: 4px;
    padding-left: 8px;
    line-height: 20px;
  }

  .combo-button {
    padding-left: 8px;
  }

  .combo-button-label {
    max-width: 220px;

    @media ${tablet} {
      max-width: 300px;
    }

    @media ${hugeMobile} {
      max-width: 220px;
    }
  }
`;

export const StyledAcceptTerms = styled.div`
  width: 100%;
  display: flex;
  align-items: center;
  gap: 0.3em;
  padding-top: 20px;
  padding-bottom: 24px;

  .wizard-checkbox svg {
    margin-right: 8px;
  }
`;
