import styled, { css } from "styled-components";

const StyledTags = styled.div`
  width: 100%;
  ${(props) =>
    props.tileWidth &&
    css`
      max-width: ${(props) => props.tileWidth}px;
    `}
  display: flex;

  align-items: center;

  overflow: hidden;
`;

export default StyledTags;
