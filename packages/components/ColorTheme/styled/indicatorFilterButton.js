import styled, { css } from "styled-components";

const StyledIndicator = styled.div`
  border-radius: 50%;
  width: 8px;
  height: 8px;
  background: ${(props) => props.theme.filterInput.filter.indicatorColor};
  position: absolute;
  top: 25px;
  left: 25px;

  z-index: 10;
`;

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    background: ${$currentColorScheme.main.accent};

    &:hover {
      background: ${$currentColorScheme.main.accent};
    }
  `;

export default styled(StyledIndicator)(getDefaultStyles);
