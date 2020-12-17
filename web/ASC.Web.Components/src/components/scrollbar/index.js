import React from "react";
import PropTypes from "prop-types";
import { Scrollbars } from "react-custom-scrollbars";

const Scrollbar = React.forwardRef((props, ref) => {
  //console.log("Scrollbar render");
  const scrollbarType = {
    smallWhite: {
      thumbV: {
        backgroundColor: "rgba(256, 256, 256, 0.2)",
        width: "2px",
        marginLeft: "2px",
        borderRadius: "inherit",
      },
      thumbH: {
        backgroundColor: "rgba(256, 256, 256, 0.2)",
        height: "2px",
        marginTop: "2px",
        borderRadius: "inherit",
      },
      view: { outline: "none", WebkitOverflowScrolling: "auto" },
    },
    smallBlack: {
      thumbV: {
        backgroundColor: "rgba(0, 0, 0, 0.1)",
        width: "3px",
        marginLeft: "2px",
        borderRadius: "inherit",
      },
      thumbH: {
        backgroundColor: "rgba(0, 0, 0, 0.1)",
        height: "3px",
        marginTop: "2px",
        borderRadius: "inherit",
      },
      view: { outline: "none", WebkitOverflowScrolling: "auto" },
    },
    mediumBlack: {
      thumbV: {
        backgroundColor: "rgba(0, 0, 0, 0.1)",
        width: "8px",
        borderRadius: "inherit",
      },
      thumbH: {
        backgroundColor: "rgba(0, 0, 0, 0.1)",
        height: "8px",
        borderRadius: "inherit",
      },
      view: {
        paddingRight: "16px",
        outline: "none",
        WebkitOverflowScrolling: "auto",
      },
    },
    preMediumBlack: {
      thumbV: {
        backgroundColor: "rgba(0, 0, 0, 0.1)",
        width: "5px",
        borderRadius: "inherit",
        cursor: "default",
      },
      thumbH: {
        backgroundColor: "rgba(0, 0, 0, 0.1)",
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

  const renderView = ({ style, ...props }) => (
    <div
      {...props}
      style={{ ...style, ...view }}
      tabIndex={-1}
      className={"scroll-body"}
    />
  );

  return (
    <Scrollbars
      renderView={renderView}
      renderThumbVertical={renderNavThumbVertical}
      renderThumbHorizontal={renderNavThumbHorizontal}
      {...props}
      ref={ref}
    />
  );
});

Scrollbar.propTypes = {
  stype: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Scrollbar.defaultProps = {
  stype: "smallBlack",
};

export default Scrollbar;
