import styled, { css } from 'styled-components';

const StyledButton = styled.div`
  width: 32px;
  min-width: 32px;
  height: 32px;

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
`;

export default StyledButton;
