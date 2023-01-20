import styled, { css } from "styled-components";
import { tablet, hugeMobile } from "@docspace/components/utils/device";
import { isIOS, isFirefox, isMobileOnly } from "react-device-detect";

export const Wrapper = styled.div`
  height: ${isIOS && !isFirefox ? "calc(var(--vh, 1vh) * 100)" : "100vh"};
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: row;
  box-sizing: border-box;

  ${isMobileOnly &&
  css`
    height: auto;
    min-height: 100%;
    width: 100%;
  `}

  background-image: url("/static/images/background.pattern.react.svg");
  background-repeat: no-repeat;
  background-attachment: fixed;
  background-size: 100% 100%;

  @media ${tablet} {
    background-size: cover;
  }

  @media ${hugeMobile} {
    background-image: none;
  }
`;

export const WizardContainer = styled.div`
  margin: 0 auto;
  margin-top: 100px;
  display: flex;
  flex-direction: column;
  align-items: center;

  @media ${tablet} {
    width: 100%;
    max-width: 480px;
  }

  @media ${hugeMobile} {
    width: calc(100% - 32px);
  }

  .docspace-logo {
    display: flex;
    align-items: center;
    justify-content: center;
    padding-bottom: 64px;
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

  .wizard-password {
    width: 100%;
    padding-top: 16px;
  }
`;

export const StyledLink = styled.div`
  width: 100%;
  display: flex;
  gap: 8px;
  align-items: center;
  padding-top: 10px;
  padding-bottom: 20px;

  .generate-password-link {
    color: #657077;
  }

  .icon-button_svg {
    svg > g > path {
      fill: #657077;
    }
  }
`;

export const StyledInfo = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 83px 1fr;
  align-items: center;
  padding-bottom: 12px;

  .machine-name {
    padding-left: 16px;
  }
`;

export const StyledAcceptTerms = styled.div`
  width: 100%;
  display: flex;
  align-items: center;
  gap: 0.3em;
  padding-top: 12px;
  padding-bottom: 24px;

  .wizard-checkbox svg {
    margin-right: 8px;
  }
`;
