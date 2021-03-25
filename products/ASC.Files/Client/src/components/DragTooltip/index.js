import React from "react";
import { inject, observer } from "mobx-react";
import DragTooltip from "./DragTooltip";

const Tooltip = ({ dragging }) => dragging && <DragTooltip />;

export default inject(({ initFilesStore }) => ({
  dragging: initFilesStore.dragging,
}))(observer(Tooltip));
