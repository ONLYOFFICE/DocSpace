import styled, { css } from "styled-components";

export const ButtonsWrapper = styled.div`
  display: table;
  margin: auto;
`;

export const LoginContainer = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 120px auto 0 auto;
  max-width: 960px;

  .login-tooltip {
    padding-left: 4px;
    display: inline-block;
  }

  .buttonWrapper {
    margin-bottom: 8px;
    width: 320px;

    @media (max-width: 768px) {
      width: 480px;
    }

    @media (max-width: 414px) {
      width: 311px;
    }
  }

  @media (max-width: 768px) {
    padding: 0 16px;
    max-width: 480px;
  }

  @media (max-width: 414px) {
    margin: 72px auto 0 auto;
    max-width: 311px;
  }

  .greeting-title {
    width: 100%;
    padding-bottom: 32px;
  }

  .workspace-title {
    width: 100%;
    padding-bottom: 16px;
  }

  .more-label {
    padding-top: 18px;
  }

  .or-label {
    margin: 0 8px;
  }

  .code-input {
    margin-bottom: 32px;
  }

  .line {
    display: flex;
    width: 320px;
    align-items: center;
    color: #eceef1;
    padding-top: 35px;

    @media (max-width: 768px) {
      width: 480px;
    }

    @media (max-width: 414px) {
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

  .auth-form-container {
    margin: 32px 213px 0 213px;
    width: 320px;

    @media (max-width: 768px) {
      margin: 32px 0 0 0;
      width: 100%;
    }
    @media (max-width: 375px) {
      margin: 32px 0 0 0;
      width: 100%;
    }

    .login-forgot-wrapper {
      height: 36px;
      padding: 14px 0;

      .login-checkbox-wrapper {
        display: flex;

        .login-checkbox {
          float: left;
          span {
            font-size: 12px;
          }
        }
      }

      .login-link {
        line-height: 18px;
        margin-left: auto;
      }
    }

    .login-button {
      margin-bottom: 16px;
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
  }
`;

export const LoginFormWrapper = styled.div`
  display: grid;
  grid-template-rows: ${(props) =>
    props.enabledJoin
      ? props.isDesktop
        ? css`1fr 10px`
        : css`1fr 66px`
      : css`1fr`};
  width: 100%;
  height: calc(100vh-48px);
`;
