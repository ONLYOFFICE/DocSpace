import * as React from "react";

export const ActionType = {
  zoomIn: 1,
  zoomOut: 2,
  prev: 3,
  next: 4,
  rotateLeft: 5,
  rotateRight: 6,
  reset: 7,
  close: 8,
  scaleX: 9,
  scaleY: 10,
  download: 11,
};

export default function Icon(props) {
  let prefixCls = "react-viewer-icon";

  return (
    <i className={`${prefixCls} ${prefixCls}-${ActionType[props.type]}`}></i>
  );
}
