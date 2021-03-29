import React from "react";
import { inject, observer } from "mobx-react";
import DragTooltip from "./DragTooltip";

const Tooltip = ({ dragging }) => (dragging ? <DragTooltip /> : <></>);

export default inject(({ initFilesStore, filesStore }) => ({
  dragging: initFilesStore.dragging && filesStore.selection[0],
}))(observer(Tooltip));
