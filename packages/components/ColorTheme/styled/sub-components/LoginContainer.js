import styled from "styled-components";
import { tablet, hugeMobile } from "@docspace/components/utils/device";
import Base from "@docspace/components/themes/base";

const LoginContainer = styled.div`
  user-select: none;
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 56px auto 0 auto;
  max-width: 960px;
  z-index: 0;

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

  .socialButton {
    min-height: 40px;
  }

  .or-label,
  .login-or-access-text {
    min-height: 18px;
  }

  .login-or-access-text {
    text-transform: lowercase;
    color: ${(props) => props.theme.login.orTextColor};
  }

  .recover-link {
    min-height: 19px;
  }

  .greeting-title {
    width: 100%;
    padding-bottom: 32px;
    min-height: 32px;
    color: ${(props) => props.theme.login.headerColor};

    @media ${hugeMobile} {
      padding-top: 32px;
    }
  }

  .more-label {
    padding-top: 18px;
  }

  .or-label {
    color: ${(props) => props.theme.login.orTextColor};
    margin: 0 32px;
  }

  .line {
    display: flex;
    width: 320px;
    align-items: center;
    color: ${(props) => props.theme.login.orLineColor};
    padding: 32px 0;

    @media ${tablet} {
      width: 416px;
    }

    @media ${hugeMobile} {
      width: 311px;
    }
  }

  .line:before,
  .line:after {
    content: "";
    flex-grow: 1;
    background: ${(props) => props.theme.login.orLineColor};
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
    color: #333;
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
    width: 320px;

    @media ${tablet} {
      width: 100%;
    }

    .field-body {
      input,
      .password-input > div {
        background: ${(props) => props.theme.input.backgroundColor};
        color: ${(props) => props.theme.input.color};
        border-color: ${(props) => props.theme.input.borderColor};
      }
    }

    .login-forgot-wrapper {
      margin-bottom: 14px;
      .login-checkbox-wrapper {
        display: flex;
        //align-items: center;

        .login-checkbox {
          display: flex;
          align-items: flex-start;

          svg {
            margin-right: 8px !important;
            rect {
              fill: ${(props) => props.theme.checkbox.fillColor};
              stroke: ${(props) => props.theme.checkbox.borderColor};
            }

            path {
              fill: ${(props) => props.theme.checkbox.arrowColor};
            }
          }

          .help-button {
            svg {
              path {
                fill: ${(props) => props.theme.login.helpButton};
              }
            }
          }

          .checkbox-text {
            color: ${(props) => props.theme.checkbox.arrowColor};
          }

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

      & > :first-child {
        margin-top: 24px;
      }
    }
  }

  .logo-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 46px;
    padding-bottom: 40px;

    svg {
      path:last-child {
        fill: ${(props) => props.theme.client.home.logoColor};
      }
    }

    @media ${hugeMobile} {
      display: none;
    }
  }

  .workspace-title {
    color: ${(props) => props.theme.login.titleColor};
    margin-bottom: 16px;

    @media ${hugeMobile} {
      margin-top: 32px;
    }
  }

  .code-description {
    color: ${(props) => props.theme.login.textColor};
  }
`;

LoginContainer.defaultProps = { theme: Base };

export default LoginContainer;
