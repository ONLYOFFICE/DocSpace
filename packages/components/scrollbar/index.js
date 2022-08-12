import React from "react";
import PropTypes from "prop-types";
import { isMobile } from "@docspace/components/utils/device";
import StyledScrollbar from "./styled-scrollbar";
const Scrollbar = React.forwardRef((props, ref) => {
  const scrollbarType = {
    smallWhite: {
      thumbV: {
        width: "2px",
        marginLeft: "2px",
        borderRadius: "inherit",
      },
      thumbH: {
        height: "2px",
        marginTop: "2px",
        borderRadius: "inherit",
      },
      view: { outline: "none", WebkitOverflowScrolling: "auto" },
    },
    smallBlack: {
      thumbV: {
        width: "3px",
        marginLeft: "2px",
        borderRadius: "inherit",
      },
      thumbH: {
        height: "3px",
        marginTop: "2px",
        borderRadius: "inherit",
      },
      view: { outline: "none", WebkitOverflowScrolling: "auto" },
    },
    mediumBlack: {
      thumbV: {
        width: "8px",
        borderRadius: "inherit",
      },
      thumbH: {
        height: "8px",
        borderRadius: "inherit",
      },
      view: {
        paddingRight: isMobile() ? "8px" : "17px",
        outline: "none",
        WebkitOverflowScrolling: "auto",
      },
    },
    preMediumBlack: {
      thumbV: {
        width: "5px",
        borderRadius: "inherit",
        cursor: "default",
      },
      thumbH: {
        height: "5px",
        borderRadius: "inherit",
        cursor: "default",
      },
      view: { outline: "none", WebkitOverflowScrolling: "auto" },
    },
  };

  const stype = scrollbarType[props.stype];

  const thumbV = stype ? stype.thumbV : {};
  const thumbH = stype ? stype.thumbH : {};
  const view = stype ? stype.view : {};

  const renderNavThumbVertical = ({ style, ...props }) => (
    <div
      {...props}
      className="nav-thumb-vertical"
      style={{ ...style, ...thumbV }}
    />
  );

  const renderNavThumbHorizontal = ({ style, ...props }) => (
    <div
      className="nav-thumb-horizontal"
      {...props}
      style={{ ...style, ...thumbH }}
    />
  );

  const renderView = ({ style, ...rest }) => (
    <div
      {...rest}
      style={{ ...style, ...view }}
      tabIndex={-1}
      className={`${props.scrollclass} scroll-body`}
    />
  );

  return (
    <StyledScrollbar
      renderView={renderView}
      renderThumbVertical={renderNavThumbVertical}
      renderThumbHorizontal={renderNavThumbHorizontal}
      {...props}
      ref={ref}
    />
  );
});

Scrollbar.propTypes = {
  /** Scrollbar style type */
  stype: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id  */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Scrollbar.defaultProps = {
  stype: "mediumBlack",
};

export default Scrollbar;
