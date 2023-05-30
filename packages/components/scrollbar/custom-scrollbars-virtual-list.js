/* eslint-disable react/prop-types */
import React from "react";
import Scrollbar from "../scrollbar";

export class CustomScrollbars extends React.Component {
  refSetter = (scrollbarsRef, forwardedRef) => {
    const isFuntion = typeof forwardedRef === "function";

    const ref = scrollbarsRef?.view ?? null;

    if (isFuntion) {
      forwardedRef(ref);
    } else {
      forwardedRef = ref;
    }
  };

  render() {
    const { onScroll, forwardedRef, style, children, className, stype } =
      this.props;
    //console.log("CustomScrollbars", this.props);
    return (
      <Scrollbar
        ref={(scrollbarsRef) => this.refSetter(scrollbarsRef, forwardedRef)}
        style={{ ...style, overflow: "hidden" }}
        onScroll={onScroll}
        stype={stype}
        className={className}
      >
        {children}
        <div className="additional-scroll-height"></div>
      </Scrollbar>
    );
  }
}

CustomScrollbars.defaultProps = {
  stype: "mediumBlack",
};

const CustomScrollbarsVirtualList = React.forwardRef((props, ref) => (
  <CustomScrollbars {...props} forwardedRef={ref} />
));

export default CustomScrollbarsVirtualList;
