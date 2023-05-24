import styled, { css } from "styled-components";
const StyledIconWrapper = styled.div`
  width: 17px;
  display: flex;
  align-items: ${(props) => (props.isRoot ? "center" : "flex-end")};
  justify-content: center;

  svg {
    path {
      fill: ${(props) => props.theme.navigation.icon.fill};
    }

    circle {
      stroke: ${(props) => props.theme.navigation.icon.fill};
    }

    path:first-child {
      fill: none !important;
      stroke: ${(props) => props.theme.navigation.icon.stroke};
    }
  }
`;

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    svg {
      path:nth-child(2) {
        fill: ${$currentColorScheme.main.accent};
      }
      circle {
        stroke: ${$currentColorScheme.main.accent};
      }
    }
  `;

export default styled(StyledIconWrapper)(getDefaultStyles);
