import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledCircleWrap = styled.div`
  width: 16px;
  height: 16px;
  background: none;
  border-radius: 50%;
  cursor: pointer;
`;

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    .circle__mask .circle__fill {
      background-color: ${$currentColorScheme.main.accent} !important;
    }

    .loading-button {
      color: ${$currentColorScheme.main.accent};
    }
  `;

StyledCircleWrap.defaultProps = {
  theme: Base,
};

export default styled(StyledCircleWrap)(getDefaultStyles);
