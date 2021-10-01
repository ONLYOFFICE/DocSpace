import styled from "styled-components";

import Base from "../themes/base";
import { tablet } from "../utils/device";

const StyledDragAndDrop = styled.div`
  /*-webkit-touch-callout: none;
-webkit-user-select: none;
-moz-user-select: none;
-ms-user-select: none;
user-select: none;*/
  height: ${(props) => props.theme.dragAndDrop.height};
  border: ${(props) =>
    props.drag
      ? props.theme.dragAndDrop.border
      : props.theme.dragAndDrop.transparentBorder};
  margin-left: -2px;
  position: relative;

  @media ${tablet} {
    margin-left: 0;
  }
  outline: none;
  background: ${(props) =>
    props.dragging
      ? props.isDragAccept
        ? props.theme.dragAndDrop.acceptBackground
        : props.theme.dragAndDrop.background
      : "none !important"};

  .droppable-hover {
    background: ${(props) => props.theme.dragAndDrop.acceptBackground};
  }
`;

StyledDragAndDrop.defaultProps = { theme: Base };

export default StyledDragAndDrop;
