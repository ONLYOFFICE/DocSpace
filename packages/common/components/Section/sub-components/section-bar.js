import React from "react";
import equal from "fast-deep-equal/react";

class SectionBar extends React.Component {
  componentWillUnmount() {
    this.props.setMaintenanceExist && this.props.setMaintenanceExist(false);
  }

  render() {
    const { children } = this.props;
    return <>{children}</>;
  }
}

SectionBar.displayName = "SectionBar";

export default SectionBar;
