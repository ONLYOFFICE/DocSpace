import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

class SectionBodyContent extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const { children } = this.props;
    //console.log(" SectionBodyContent render");
    return <>{children}</>;
  }
}

SectionBodyContent.displayName = "SectionBodyContent";

SectionBodyContent.propTypes = {
  children: PropTypes.any,
};

export default SectionBodyContent;
