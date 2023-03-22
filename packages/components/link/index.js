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
  /** Color of link */
  color: PropTypes.string,
  /** ont size of link */
  fontSize: PropTypes.string,
  /** Font weight of link */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Used as HTML `href` property */
  href: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Set font weight */
  isBold: PropTypes.bool,
  /** Set hovered state and effects of link */
  isHovered: PropTypes.bool,
  /** Set css-property 'opacity' to 0.5. Usually apply for users with "pending" status */
  isSemitransparent: PropTypes.bool,
  /** Activate or deactivate _text-overflow_ CSS property with ellipsis (' … ') value */
  isTextOverflow: PropTypes.bool,
  /** Disabled hover styles */
  noHover: PropTypes.bool,
  /** What the link will trigger when clicked. Only for \'action\' type of link */
  onClick: PropTypes.func,
  /** Used as HTML `rel` property */
  rel: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Used as HTML `tabindex` property */
  tabIndex: PropTypes.number,
  /** The _target_ attribute specifies where the linked document will open when the link is clicked. */
  target: PropTypes.oneOf(["_blank", "_self", "_parent", "_top"]),
  /** Used as HTML `title` property */
  title: PropTypes.string,
  /** Type of link */
  type: PropTypes.oneOf(["action", "page"]),

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
