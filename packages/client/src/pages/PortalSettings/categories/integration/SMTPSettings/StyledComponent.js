import styled, { css } from "styled-components";

const StyledComponent = styled.div`
  .smtp-settings_description {
    margin-bottom: 20px;
    max-width: 700px;
  }
  .smtp-settings_title {
    display: flex;

    span {
      margin-left: 2px;
    }
  }
  .smtp-settings_input {
    margin-bottom: 16px;
    margin-top: 4px;
    max-width: 350px;

    .field-label-icon {
      display: none;
    }
  }
  .smtp-settings_auth {
    margin: 24px 0;

    .smtp-settings_login {
      margin-top: 16px;
    }
    .smtp-settings_toggle {
      position: static;
    }
  }
`;

const ButtonStyledComponent = styled.div`
  margin-top: 20px;
`;
export { StyledComponent, ButtonStyledComponent };
