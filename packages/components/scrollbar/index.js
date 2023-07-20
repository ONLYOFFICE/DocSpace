import React from "react";
import PropTypes from "prop-types";

import { isMobile } from "@docspace/components/utils/device";
import { classNames } from "../utils/classNames";
import StyledScrollbar from "./styled-scrollbar";
import { useTheme } from "styled-components";

const Scrollbar = React.forwardRef(({ id, onScroll, ...props }, ref) => {
  const { interfaceDirection } = useTheme();
  const isRtl = interfaceDirection === "rtl";
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
        paddingRight: !isRtl && (isMobile() ? "8px" : "17px"),
        paddingLeft: isRtl && (isMobile() ? "8px" : "17px"),
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

  // onScroll handler placed here on Scroller element to get native event instead of parameters that library put
  const renderScroller = (props) => {
    const { elementRef, ...restProps } = props;
    return <div {...restProps} id={id} ref={elementRef} onScroll={onScroll} />;
  };

  return (
    <StyledScrollbar
      disableTracksWidthCompensation
      rtl={isRtl}
      ref={ref}
      {...props}
      scrollerProps={{ renderer: renderScroller }}
      contentProps={{
        style: view,
        tabIndex: -1,
        className: classNames("scroll-body", props.scrollclass),
      }}
      thumbYProps={{
        className: "nav-thumb-vertical",
        style: thumbV,
      }}
      thumbXProps={{
        className: "nav-thumb-horizontal",
        style: thumbH,
      }}
      trackYProps={{
        style: { width: thumbV.width, background: "transparent" },
      }}
      trackXProps={{
        style: {
          height: thumbH.height,
          background: "transparent",
          direction: "ltr", // Required to make the horizontal thumb work properly
        },
      }}
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
