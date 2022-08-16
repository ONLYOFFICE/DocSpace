import styled, { css } from "styled-components";
import StyledButton from "@docspace/components/button/styled-button";

const activeCss = css`
  border-color: ${(props) => props.currentColorScheme.buttonsMain};
  background: ${(props) =>
    !!props.primary && props.currentColorScheme.buttonsMain};
  // filter: ${(props) => !!props.primary && "brightness(60%)"};
`;

const hoverCss = css`
  border-color: ${(props) => props.currentColorScheme.buttonsMain};
  background: ${(props) =>
    !!props.primary && props.currentColorScheme.buttonsMain};
  opacity: ${(props) => !!props.primary && "0.85"};
`;

const getDefaultStyles = ({
  primary,
  currentColorScheme,
  isDisabled,
  isLoading,
  isClicked,
  isHovered,
  disableHover,
}) =>
  currentColorScheme.buttonsMain &&
  css`
    background: ${!!primary && currentColorScheme.buttonsMain};
    opacity: ${!!primary && isDisabled && "0.6"};
    border-color: ${!!primary && currentColorScheme.buttonsMain};

    ${!isDisabled &&
    !isLoading &&
    (isHovered
      ? hoverCss
      : !disableHover &&
        css`
          &:hover {
            ${hoverCss}
          }
        `)}

    ${!isDisabled &&
    !isLoading &&
    (isClicked
      ? activeCss
      : css`
          &:active {
            ${activeCss}
          }
        `)}
  `;

export default styled(StyledButton)(getDefaultStyles);
