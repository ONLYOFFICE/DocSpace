import React from "react";
import PropTypes from "prop-types";
import StyledText from "./styled-link";

// eslint-disable-next-line react/display-name
const Link = ({
  isTextOverflow,
  children,
  noHover,
  enableUserSelect,
  ...rest
}) => {
  // console.log("Link render", rest);

  return (
    <StyledText
      tag="a"
      isTextOverflow={isTextOverflow}
      noHover={noHover}
      truncate={isTextOverflow}
      enableUserSelect={enableUserSelect}
      {...rest}
    >
      {children}
    </StyledText>
  );
};

Link.propTypes = {
  children: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** Link color */
  color: PropTypes.string,
  /** Link font size */
  fontSize: PropTypes.string,
  /** Link font weight  */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Line height of the link */
  lineHeight: PropTypes.string,
  /** Used as HTML `href` property */
  href: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Sets font weight */
  isBold: PropTypes.bool,
  /** Sets hovered state and link effects */
  isHovered: PropTypes.bool,
  /** Sets the 'opacity' css-property to 0.5. Usually applied for the users with "pending" status */
  isSemitransparent: PropTypes.bool,
  /** Activates or deactivates _text-overflow_ CSS property with ellipsis (' … ') value */
  isTextOverflow: PropTypes.bool,
  /** Disables hover styles */
  noHover: PropTypes.bool,
  /** Sets a callback function that is triggered when the link is clicked. Only for \'action\' type of link */
  onClick: PropTypes.func,
  /** Used as HTML `rel` property */
  rel: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Used as HTML `tabindex` property */
  tabIndex: PropTypes.number,
  /** Specifies where the linked document will open once the link is clicked. */
  target: PropTypes.oneOf(["_blank", "_self", "_parent", "_top"]),
  /** Used as HTML `title` property */
  title: PropTypes.string,
  /** Link type */
  type: PropTypes.oneOf(["action", "page"]),
  /** Label */
  label: PropTypes.string,
  /** Allows enabling UserSelect */
  enableUserSelect: PropTypes.bool,
};

Link.defaultProps = {
  className: "",
  fontSize: "13px",
  href: undefined,
  isBold: false,
  isHovered: false,
  isSemitransparent: false,
  isTextOverflow: false,
  noHover: false,
  rel: "noopener noreferrer",
  tabIndex: -1,
  type: "page",
  enableUserSelect: false,
};

export default Link;
