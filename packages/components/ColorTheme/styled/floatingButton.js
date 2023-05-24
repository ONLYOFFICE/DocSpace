import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledCircleWrap = styled.div`
  position: relative;
  z-index: 500;
  width: 48px;
  height: 48px;
  background: ${(props) =>
    props.color ? props.color : props.theme.floatingButton.backgroundColor};
  border-radius: 50%;
  cursor: pointer;
  box-shadow: ${(props) => props.theme.floatingButton.boxShadow};
`;

const getDefaultStyles = ({ $currentColorScheme, color, displayProgress }) =>
  $currentColorScheme &&
  css`
    background: ${color || $currentColorScheme.main.accent} !important;

    .circle__background {
      background: ${color || $currentColorScheme.main.accent} !important;
    }

    .icon-box {
      svg {
        path {
          fill: ${$currentColorScheme.text.accent};
        }
      }
    }

    .circle__mask .circle__fill {
      background-color: ${!displayProgress
        ? "transparent !important"
        : $currentColorScheme.text.accent};
    }
  `;

StyledCircleWrap.defaultProps = { theme: Base };

export default styled(StyledCircleWrap)(getDefaultStyles);
