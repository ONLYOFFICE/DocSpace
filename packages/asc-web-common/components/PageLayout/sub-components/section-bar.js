import React from "react";
import equal from "fast-deep-equal/react";

class SectionBar extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const { children } = this.props;
    return <>{children}</>;
  }
}

SectionBar.displayName = "SectionBar";

export default SectionBar;
