import styled, { css } from "styled-components";
import { LoginContainer } from "@appserver/login/src/StyledLogin";

const getDefaultStyles = ({ currentColorScheme }) => css`
  .login-link {
    color: ${currentColorScheme.accentColor};
  }
`;

export default styled(LoginContainer)(getDefaultStyles);
