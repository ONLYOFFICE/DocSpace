import styled, { css } from "styled-components";

const StyledTags = styled.div`
  width: 100%;
  ${(props) =>
    props.tagsWidth &&
    css`
      max-width: ${(props) => props.tagsWidth}px;
    `}
  display: flex;

  align-items: center;

  overflow: hidden;
`;

export default StyledTags;
