import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledButton = styled.div`
  display: inline-block;
  background: ${(props) => props.theme.selectorAddButton.background};
  border: ${(props) => props.theme.selectorAddButton.border};
  box-sizing: ${(props) => props.theme.selectorAddButton.boxSizing};
  border-radius: ${(props) => props.theme.selectorAddButton.borderRadius};
  height: ${(props) => props.theme.selectorAddButton.height};
  width: ${(props) => props.theme.selectorAddButton.width};
  padding: ${(props) => props.theme.selectorAddButton.padding};

  cursor: ${(props) => (!props.isDisabled ? "pointer" : "default")};

  svg {
    path {
      ${(props) =>
        !props.isDisabled &&
        css`
          fill: ${props.theme.selectorAddButton.iconColor};
        `}
    }
  }

  &:hover {
    background: ${(props) =>
      !props.isDisabled && props.theme.selectorAddButton.hoverBackground};

    svg {
      path {
        ${(props) =>
          !props.isDisabled &&
          css`
            fill: ${props.theme.selectorAddButton.iconColorHover};
          `}
      }
    }
  }

  &:active {
    background: ${(props) =>
      !props.isDisabled && props.theme.selectorAddButton.activeBackground};
    svg {
      path {
        ${(props) =>
          !props.isDisabled &&
          css`
            fill: ${props.theme.selectorAddButton.iconColorActive};
          `}
      }
    }
  }

  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

StyledButton.defaultProps = { theme: Base };

export default StyledButton;
