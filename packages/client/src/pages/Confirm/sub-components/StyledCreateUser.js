import styled from "styled-components";
import Box from "@docspace/components/box";
import { hugeMobile, tablet } from "@docspace/components/utils/device";

export const ButtonsWrapper = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;

  .buttonWrapper {
    margin-bottom: 8px;
    width: 100%;
  }
`;

export const ConfirmContainer = styled(Box)`
  margin: 56px auto;
  display: flex;
  flex: 1fr 1fr;
  gap: 80px;
  flex-direction: row;
  justify-content: center;

  @media ${tablet} {
    display: flex;
    flex: 1fr;
    flex-direction: column;
    align-items: center;
    gap: 32px;
  }

  @media ${hugeMobile} {
    margin: 0 auto;
    width: 100%;
    flex: 1fr;
    flex-direction: column;
    gap: 24px;

    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `padding-left: 8px;`
        : `padding-right: 8px;`}
  }
`;

export const GreetingContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: left;
  height: 100%;
  width: 496px;

  @media ${tablet} {
    width: 480px;
  }

  @media ${hugeMobile} {
    width: 100%;
  }

  .greeting-title {
    width: 100%;
    padding-bottom: 32px;

    @media ${tablet} {
      text-align: center;
    }

    @media ${hugeMobile} {
      padding-bottom: 24px;
    }
  }

  .greeting-block {
    display: flex;
    flex-direction: row;

    .user-info {
      display: flex;
      flex-direction: column;

      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `margin-right: 12px;`
          : `margin-left: 12px;`}
      justify-content: center;
    }

    .avatar {
      height: 54px;
      width: 54px;
    }
  }

  .tooltip {
    position: relative;
    display: inline-block;
    margin-top: 16px;
    border: 1px solid ${(props) => props.theme.invitePage.borderColor};
    padding: 16px;
    border-radius: 6px;
  }

  .tooltip .tooltiptext {
    margin: 0;
    width: 100%;
    white-space: pre-line;
  }

  .docspace-logo {
    width: 100%;
    padding-bottom: 32px;

    .injected-svg {
      height: 44px;
    }

    @media ${tablet} {
      display: flex;
      align-items: center;
      justify-content: center;
      padding-bottom: 40px;
    }
  }
`;

export const RegisterContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;
  width: 100%;

  .or-label {
    text-transform: uppercase;
    margin: 0 8px;
  }

  .more-label {
    padding-top: 18px;
  }

  .line {
    display: flex;
    width: 100%;
    align-items: center;
    color: ${(props) => props.theme.invitePage.borderColor};;
    padding-top: 35px;
    margin-bottom: 32px;
  }

  .line:before,
  .line:after {
    content: "";
    flex-grow: 1;
    background: ${(props) => props.theme.invitePage.borderColor};
    height: 1px;
    font-size: 0px;
    line-height: 0px;
    margin: 0px;
  }

  .auth-form-container {
    width: 100%;

    .password-field{
        margin-bottom: 24px;
    }

    @media ${tablet} {
      width: 100%;
    }
    @media ${hugeMobile} {
      width: 100%;
    }
  }

  .password-field-wrapper {
    width: 100%;
  }
}`;
