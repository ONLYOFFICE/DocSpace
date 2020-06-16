import styled from "styled-components";

const StyledDragAndDrop = styled.div`
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  border: ${props => props.drag ? "1px dashed #bbb" : "1px solid transparent"};
  background: ${props => props.dragging ? "#F8F7BF" : "none !important"};
`;

export default StyledDragAndDrop;
