import React from "react";
import PropTypes from "prop-types";
import StyledBody from "./StyledBody";

class Body extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    const { children, className, style } = this.props;
    return (
      <StyledBody className={className} style={style}>
        {children}
      </StyledBody>
    );
  }
}

Body.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  style: PropTypes.object,
};

export default Body;
