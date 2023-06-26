import React from "react";
import PropTypes from "prop-types";
import { useTheme } from "styled-components";

import { isMobile } from "@docspace/components/utils/device";
import StyledScrollbar from "./styled-scrollbar";
import { classNames } from "../utils/classNames";
import CustomScrollbars from "./customScrollbar2";

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
  const theme = useTheme();
  const isRtl = theme.interfaceDirection === "rtl";
  const renderNavThumbVertical = ({ style, ...props }) => {
    return (
      <div
        {...props}
        className="nav-thumb-vertical"
        style={{ ...style, ...thumbV }}
      />
    );
  };

  const renderNavThumbHorizontal = ({ style, ...props }) => {
    return (
      <div
        className="nav-thumb-horizontal"
        {...props}
        style={{ ...style, ...thumbH }}
      />
    );
  };

  const renderView = ({ style, ...rest }) => {
    return (
      <div
        {...rest}
        style={{
          ...style,
          ...view,
          margin: isRtl ? "0 0 -15px -15px" : "0 -15px -15px 0",
        }}
        tabIndex={-1}
        className={classNames("scroll-body", props.scrollclass)}
      />
    );
  };

  const renderTrackNavVertical = ({ style }) => {
    return isRtl ? (
      <div
        className="track-vertical"
        style={{ ...style, left: "2px", top: "2px", bottom: "2px" }}
      />
    ) : (
      <div
        className="track-vertical"
        style={{ ...style, right: "2px", top: "2px", bottom: "2px" }}
      />
    );
  };
  return (
    <>
      <CustomScrollbars>
        ================================================================ Lorem
        ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
        incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
        quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
        consequat. Duis aute irure dolor in reprehenderit in voluptate velit
        esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat
        cupidatat non proident, sunt in culpa qui officia deserunt mollit anim
        id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit,
        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut
        enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut
        aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit
        in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui
        officia deserunt mollit anim id est laborum.
        ================================================================
      </CustomScrollbars>
      <StyledScrollbar
        renderView={renderView}
        renderThumbVertical={renderNavThumbVertical}
        renderThumbHorizontal={renderNavThumbHorizontal}
        renderTrackVertical={renderTrackNavVertical}
        {...props}
        ref={ref}
      />
    </>
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
