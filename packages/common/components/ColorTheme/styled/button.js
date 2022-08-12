import styled, { css } from "styled-components";
import StyledButton from "@appserver/components/button/styled-button";

const getDefaultStyles = ({ primary, currentColorScheme, isDisabled }) => css`
  background: ${!!primary && currentColorScheme.buttonsMain};

  opacity: ${!!primary && isDisabled && "0.6"};
  border-color: ${!!primary && currentColorScheme.buttonsMain};

  &:hover {
    background: ${!!primary && currentColorScheme.buttonsMain};
    border-color: ${currentColorScheme.buttonsMain};
  }
`;

export default styled(StyledButton)(getDefaultStyles);
