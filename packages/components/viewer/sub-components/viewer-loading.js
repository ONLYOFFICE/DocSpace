import * as React from "react";

export default function ViewerLoading(props) {
  let cls = "circle-loading";
  return (
    <div className="loading-wrap" style={props.style}>
      <div className={cls}></div>
    </div>
  );
}
