import styled, { css } from "styled-components";
import StyledButton from "@docspace/components/button/styled-button";
import Base from "@docspace/components/themes/base";

const activeCss = css`
  border-color: ${(props) =>
    props.theme.isBase
      ? props.$currentColorScheme.main.buttons
      : props.primary
      ? props.$currentColorScheme.main.buttons
      : props.theme.button.border.baseActive};

  background: ${(props) =>
    props.primary && props.$currentColorScheme.main.buttons};

  opacity: ${(props) => !props.isDisabled && "1"};

  filter: ${(props) =>
    props.primary &&
    (props.theme.isBase ? "brightness(90%)" : "brightness(82%)")};
  color: ${(props) =>
    props.$currentColorScheme.id > 7 &&
    props.primary &&
    props.$currentColorScheme.textColor};
`;

const hoverCss = css`
  border-color: ${(props) =>
    props.theme.isBase
      ? props.$currentColorScheme.main.buttons
      : props.primary
      ? props.$currentColorScheme.main.buttons
      : props.theme.button.border.baseHover};

  background: ${(props) =>
    props.primary && props.$currentColorScheme.main.buttons};

  opacity: ${(props) => props.primary && !props.isDisabled && "0.85"};
  color: ${(props) =>
    props.$currentColorScheme.id > 7 &&
    props.primary &&
    props.$currentColorScheme.textColor};
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
    color: ${$currentColorScheme.id > 7 &&
    primary &&
    $currentColorScheme.textColor};

    .loader {
      svg {
        color: ${$currentColorScheme.id > 7 &&
        primary &&
        $currentColorScheme.textColor};
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
