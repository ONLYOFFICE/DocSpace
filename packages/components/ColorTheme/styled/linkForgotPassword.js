import styled, { css } from "styled-components";
import LoginContainer from "./sub-components/LoginContainer";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    .login-link {
      color: ${$currentColorScheme?.main?.accent};
    }
  `;

export default styled(LoginContainer)(getDefaultStyles);
