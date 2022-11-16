import styled, { css } from "styled-components";
import StyledButton from "@docspace/components/button/styled-button";
import Base from "@docspace/components/themes/base";

const activeCss = css`
  border-color: ${(props) => props.$currentColorScheme.main.buttons};

  background: ${(props) =>
    props.primary && props.$currentColorScheme.main.buttons};

  opacity: ${(props) => !props.isDisabled && "1"};

  filter: ${(props) =>
    props.primary &&
    (props.theme.isBase ? "brightness(90%)" : "brightness(82%)")};
  color: ${(props) => props.$currentColorScheme.text.buttons};
`;

const hoverCss = css`
  border-color: ${(props) => props.$currentColorScheme.main.buttons};

  background: ${(props) =>
    props.primary && props.$currentColorScheme.main.buttons};

  opacity: ${(props) => props.primary && !props.isDisabled && "0.85"};
  color: ${(props) => props.primary && props.$currentColorScheme.text.buttons};
`;

const getDefaultStyles = ({
  primary,
  $currentColorScheme,
  isDisabled,
  isLoading,
  isClicked,
  isHovered,
  disableHover,
}) =>
  $currentColorScheme &&
  css`
    background: ${primary && $currentColorScheme.main.buttons};
    opacity: ${primary && isDisabled && "0.6"};
    border-color: ${primary && $currentColorScheme.main.buttons};
    color: ${primary && $currentColorScheme.text.buttons};

    .loader {
      svg {
        color: ${primary && $currentColorScheme.text.buttons};
      }
      background-color: ${primary && $currentColorScheme.main.buttons};
    }

    ${!isDisabled &&
    !isLoading &&
    (isHovered
      ? hoverCss
      : !disableHover &&
        css`
          &:hover,
          &:focus {
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

StyledButton.defaultProps = { theme: Base };

export default styled(StyledButton)(getDefaultStyles);
