import React from "react";
import PropTypes from "prop-types";
import StyledText from "./styled-text"


const Text = ({
  title,
  tag,
  as,
  fontSize,
  fontWeight,
  color,
  textAlign,
  ...rest
}) => {

  return (
    <StyledText
      fontSizeProp={fontSize}
      fontWeightProp={fontWeight}
      colorProp={color}
      textAlign={textAlign}
      as={!as && tag ? tag : as}
      title={title}
      {...rest}
    />
  );
};

Text.propTypes = {
  as: PropTypes.string,
  tag: PropTypes.string,
  title: PropTypes.string,
  color: PropTypes.string,
  textAlign: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  backgroundColor: PropTypes.string,
  truncate: PropTypes.bool,
  isBold: PropTypes.bool,
  isInline: PropTypes.bool,
  isItalic: PropTypes.bool,
  display: PropTypes.string,
};

Text.defaultProps = {
  title: null,
  color: "#333333",
  textAlign: "left",
  fontSize: "13px",
  truncate: false,
  isBold: false,
  isInline: false,
  isItalic: false,
};

export default Text;
