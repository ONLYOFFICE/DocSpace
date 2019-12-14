import styled, { css } from "styled-components";

const StyledFooter = styled.div`
  box-sizing: border-box;
  border-top: 1px solid #eceef1;
  padding: 16px;
  height: 69px;

  ${props =>
    !props.isVisible &&
    css`
      display: none;
    `}
`;

export default StyledFooter;