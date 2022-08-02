import { Base } from "@docspace/components/themes";
import styled, { css } from "styled-components";

const StyledButton = styled.div`
  width: 32px;
  min-width: 32px;
  height: 32px;

  position: relative;

  border: ${(props) => props.theme.filterInput.button.border};
  border-radius: 3px;

  box-sizing: border-box;

  display: flex;
  align-items: center;
  justify-content: center;

  margin: 0;
  padding: 0;

  margin-left: 8px;

  cursor: pointer;

  &:hover {
    border: ${(props) => props.theme.filterInput.button.hoverBorder};
  }

  div {
    cursor: pointer;
  }

  ${(props) =>
    props.isOpen &&
    css`
      background: ${(props) => props.theme.filterInput.button.openBackground};
      pointer-events: none;

      svg {
        path {
          fill: ${(props) => props.theme.filterInput.button.openFill};
        }
      }

      .dropdown-container {
        margin-top: 5px;
        min-width: 200px;
        width: 200px;
      }
    `}

  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

StyledButton.defaultProps = { theme: Base };

export default StyledButton;
