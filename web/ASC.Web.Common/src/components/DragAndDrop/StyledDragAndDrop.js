import styled from "styled-components";

const StyledDragAndDrop = styled.div`
  user-select: none;
  border: ${props => props.drag ? "1px dashed #bbb" : "1px solid transparent"};
  background: ${props => props.dragging ? "#F8F7BF" : "none !important"};
`;

export default StyledDragAndDrop;
