import styled from "styled-components";

const StyledDragAndDrop = styled.div`
  border: ${props => props.drag && "1px dashed #bbb"};
`;

export default StyledDragAndDrop;
