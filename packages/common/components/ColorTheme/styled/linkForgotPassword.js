import styled, { css } from "styled-components";
import { LoginContainer } from "@docspace/login/src/client/components/StyledLogin";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    .login-link {
      color: ${$currentColorScheme.accentColor};
    }
  `;

export default styled(LoginContainer)(getDefaultStyles);
