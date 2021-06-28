import React from "react";
import { inject, observer } from "mobx-react";
import DragTooltip from "./DragTooltip";

const Tooltip = ({ dragging }) => (dragging ? <DragTooltip /> : <></>);

export default inject(({ filesStore }) => {
  const { dragging, selection, startDrag } = filesStore;
  return { dragging: dragging && selection[0] && startDrag };
})(observer(Tooltip));
