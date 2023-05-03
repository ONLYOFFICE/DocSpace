import styled, { css } from "styled-components";

const StyledComponent = styled.div`
  .smtp-settings_description {
    margin-bottom: 20px;
  }
  .smtp-settings_input {
    margin-bottom: 16px;
    margin-top: 4px;
    max-width: 350px;
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

export default StyledComponent;
