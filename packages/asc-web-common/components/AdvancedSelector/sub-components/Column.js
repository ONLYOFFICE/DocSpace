import React from "react";
import PropTypes from "prop-types";
import StyledColumn from "./StyledColumn";

class Column extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    const { children, className, style, size } = this.props;
    return (
      <StyledColumn className={className} style={style} size={size}>
        {children}
      </StyledColumn>
    );
  }
}

Column.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  style: PropTypes.object,
  size: PropTypes.oneOf(["compact", "full"]),
};

export default Column;
