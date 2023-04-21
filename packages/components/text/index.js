import React from "react";
import PropTypes from "prop-types";
import StyledText from "./styled-text";

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
  /** Sets the tag through which the component is rendered */
  as: PropTypes.string,
  /** Accepts the tag id */
  tag: PropTypes.string,
  /** Sets background color */
  backgroundColor: PropTypes.string,
  /** Specifies the text color */
  color: PropTypes.string,
  /** Sets the 'display' property */
  display: PropTypes.string,
  /** Sets the font size */
  fontSize: PropTypes.string,
  /** Sets the font weight */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Sets font weight value to bold */
  isBold: PropTypes.bool,
  /** Sets the 'display: inline-block' property */
  isInline: PropTypes.bool,
  /** Sets the font style */
  isItalic: PropTypes.bool,
  /** Sets the line height */
  lineHeight: PropTypes.string,
  /** Disables text selection */
  noSelect: PropTypes.bool,
  /** Sets the 'text-align' property */
  textAlign: PropTypes.string,
  /** Title */
  title: PropTypes.string,
  /** Disables word wrapping */
  truncate: PropTypes.bool,
};

Text.defaultProps = {
  title: null,
  textAlign: "left",
  fontSize: "13px",
  truncate: false,
  isBold: false,
  isInline: false,
  isItalic: false,
  noSelect: false,
};

export default React.memo(Text);
