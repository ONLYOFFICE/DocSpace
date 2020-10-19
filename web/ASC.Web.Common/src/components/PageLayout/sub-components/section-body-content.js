import React from "react";
import PropTypes from "prop-types";
import isEqual from "lodash/isEqual";

class SectionBodyContent extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    const { children } = this.props;
    //console.log("PageLayout SectionBodyContent render");
    return <>{children}</>;
  }
}

SectionBodyContent.displayName = "SectionBodyContent";

SectionBodyContent.propTypes = {
  children: PropTypes.any,
};

export default SectionBodyContent;
