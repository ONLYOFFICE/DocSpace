import styled, { css } from "styled-components";

const StyledButton = styled.div`
  width: 32px;
  min-width: 32px;
  height: 32px;

  position: relative;

  border: 1px solid #d0d5da;
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
    border: 1px solid #a3a9ae;
  }

  div {
    cursor: pointer;
  }

  ${(props) =>
    props.isOpen &&
    css`
      background: #a3a9ae;
      pointer-events: none;

      svg {
        path {
          fill: #ffffff;
        }
      }

      .dropdown-container {
        margin-top: 5px;
        min-width: 200px;
        width: 200px;
      }
    `}
`;

export default StyledButton;
