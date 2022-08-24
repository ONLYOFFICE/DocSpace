import styled, { css } from "styled-components";
import { tablet, hugeMobile } from "@docspace/components/utils/device";

export const ButtonsWrapper = styled.div`
  display: flex;
  flex-direction: column;
  margin: 0 213px 0 213px;
  width: 320px;

  @media ${tablet} {
    width: 100%;
  }
`;

export const LoginContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 56px auto 0 auto;
  max-width: 960px;

  .remember-wrapper {
    max-width: 142px;
    display: flex;
    flex-direction: row;
    align-items: center;
  }

  .buttonWrapper {
    margin-bottom: 8px;
    width: 100%;
  }

  @media ${tablet} {
    max-width: 480px;
  }

  @media ${hugeMobile} {
    margin: 0 auto 0 auto;
    max-width: 311px;
  }

  .greeting-title {
    width: 100%;
    padding-bottom: 32px;

    @media ${hugeMobile} {
      padding-top: 32px;
    }
  }

  .more-label {
    padding-top: 18px;
  }

  .or-label {
    margin: 0 8px;
  }

  .line {
    display: flex;
    width: 320px;
    align-items: center;
    color: #eceef1;
    padding-top: 35px;

    @media ${tablet} {
      width: 480px;
    }

    @media ${hugeMobile} {
      width: 311px;
    }
  }

  .line:before,
  .line:after {
    content: "";
    flex-grow: 1;
    background: #eceef1;
    height: 1px;
    font-size: 0px;
    line-height: 0px;
    margin: 0px;
  }

  .code-input-container {
    margin-top: 32px;
    text-align: center;
  }

  .not-found-code {
    margin-top: 32px;
  }

  .code-input-bar {
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 6px;
    margin-top: 16px;
    padding: 14px 12px;
    text-align: center;
    font-weight: 600;
    font-size: 11px;
    line-height: 12px;
    svg {
      margin: 8px;
    }
  }

  .code-input-bar.warning {
    background: #f7e6be;
    margin-bottom: 16px;
  }

  .code-input-bar.error {
    background: #f7cdbe;
  }

  .auth-form-container {
    margin: 32px 213px 0 213px;
    width: 320px;

    @media ${tablet} {
      margin: 32px 0 0 0;
      width: 100%;
    }
    @media ${hugeMobile} {
      margin: 32px 0 0 0;
      width: 100%;
    }

    .login-forgot-wrapper {
      margin-bottom: 14px;
      .login-checkbox-wrapper {
        display: flex;
        //align-items: center;

        .login-checkbox {
          display: flex;
          align-items: flex-start;

          label {
            justify-content: center;
          }
        }

        .remember-helper-wrapper {
          display: flex;
          gap: 4px;
        }
      }

      .login-link {
        line-height: 18px;
        margin-left: auto;
      }
    }

    .login-button {
      margin-top: 8px;
    }

    .login-button-dialog {
      margin-right: 8px;
    }

    .login-bottom-border {
      width: 100%;
      height: 1px;
      background: #eceef1;
    }

    .login-bottom-text {
      margin: 0 8px;
    }

    .login-or-access {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 6px;
      margin-top: 24px;
    }
  }

  .logo-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 46px;
    padding-bottom: 64px;

    @media ${hugeMobile} {
      display: none;
    }
  }
`;

interface ILoginFormWrapperProps {
  enabledJoin?: boolean;
  isDesktop?: boolean;
}

export const LoginFormWrapper = styled.div`
  display: grid;
  grid-template-rows: ${(props: ILoginFormWrapperProps) =>
    props.enabledJoin
      ? props.isDesktop
        ? css`1fr 10px`
        : css`1fr 68px`
      : css`1fr`};
  width: 100%;
  height: 100vh;
`;
