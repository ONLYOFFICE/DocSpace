import React from "react";
import { inject, observer } from "mobx-react";
import DragTooltip from "./DragTooltip";

const Tooltip = ({ dragging }) => (dragging ? <DragTooltip /> : <></>);

export default inject(({ filesStore }) => ({
  dragging: filesStore.dragging && filesStore.selection[0],
}))(observer(Tooltip));
