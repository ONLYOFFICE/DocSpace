import React, { useCallback, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

import { isMobile } from "@docspace/components/utils/device";
import { classNames } from "../utils/classNames";
import StyledScrollbar from "./styled-scrollbar";
import { useTheme } from "styled-components";

const Scrollbar = React.forwardRef(
  ({ id, onScroll, autoHide, hideTimeout, ...props }, ref) => {
    const [isScrolling, setIsScrolling] = useState();
    const [isMouseOver, setIsMouseOver] = useState();
    const { interfaceDirection } = useTheme();
    const stopTimer = useRef();

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
        trackV: {
          width: "2px",
          background: "transparent",
        },
        trackH: {
          height: "2px",
          background: "transparent",
        },
        content: { outline: "none", WebkitOverflowScrolling: "auto" },
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
        trackV: {
          width: "3px",
          background: "transparent",
        },
        trackH: {
          height: "3px",
          background: "transparent",
        },
        content: { outline: "none", WebkitOverflowScrolling: "auto" },
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
        trackV: {
          width: "8px",
          background: "transparent",
        },
        trackH: {
          height: "8px",
          background: "transparent",
        },
        content: {
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
        trackV: {
          width: "5px",
          background: "transparent",
        },
        trackH: {
          height: "5px",
          background: "transparent",
        },
        content: { outline: "none", WebkitOverflowScrolling: "auto" },
      },
    };

    const stype = scrollbarType[props.stype];

    const thumbVStyles = stype ? stype.thumbV : {};
    const thumbHStyles = stype ? stype.thumbH : {};
    const trackCommonStyles = {
      padding: "0.05px",
      opacity: autoHide && !isScrolling ? 0 : 1,
      transition: "opacity 0.4s ease-in-out",
    };
    const trackVStyles = stype
      ? { ...stype.trackV, ...trackCommonStyles }
      : { ...trackCommonStyles };
    const trackHStyles = stype
      ? { ...stype.trackH, ...trackCommonStyles }
      : { ...trackCommonStyles };
    const contentStyles = stype ? stype.content : {};

    const showTrack = useCallback(() => {
      clearTimeout(stopTimer.current);
      setIsScrolling(true);
    }, [stopTimer]);

    const hideTrack = useCallback(() => {
      stopTimer.current = setTimeout(() => {
        setIsScrolling(false);
      }, hideTimeout);
    }, [stopTimer, hideTimeout]);

    const onScrollStart = useCallback(() => {
      if (autoHide) {
        showTrack();
      }
    }, [autoHide, showTrack]);

    const onScrollStop = useCallback(() => {
      if (autoHide && !isMouseOver) {
        hideTrack();
      }
    }, [autoHide, hideTrack, isMouseOver]);

    const onMouseEnter = useCallback(() => {
      if (autoHide) {
        showTrack();
        setIsMouseOver(true);
      }
    }, [autoHide, showTrack]);

    const onMouseLeave = useCallback(() => {
      if (autoHide) {
        hideTrack();
        setIsMouseOver(false);
      }
    }, [autoHide, hideTrack]);

    // onScroll handler placed here on Scroller element to get native event instead of parameters that library put
    const renderScroller = (props) => {
      const { elementRef, ...restProps } = props;
      return (
        <div {...restProps} id={id} ref={elementRef} onScroll={onScroll} />
      );
    };

    useEffect(() => {
      return () => clearTimeout(stopTimer.current);
    }, []);

    return (
      <StyledScrollbar
        disableTracksWidthCompensation
        rtl={isRtl}
        ref={ref}
        {...props}
        onScrollStop={onScrollStop}
        onScrollStart={onScrollStart}
        scrollerProps={{ renderer: renderScroller }}
        contentProps={{
          style: contentStyles,
          tabIndex: -1,
          className: classNames("scroll-body", props.scrollclass),
        }}
        thumbYProps={{
          className: "nav-thumb-vertical",
          style: thumbVStyles,
        }}
        thumbXProps={{
          className: "nav-thumb-horizontal",
          style: thumbHStyles,
        }}
        trackYProps={{
          style: trackVStyles,
          onMouseEnter,
          onMouseLeave,
        }}
        trackXProps={{
          style: {
            ...trackHStyles,
            direction: "ltr",
          },
          onMouseEnter,
          onMouseLeave,
        }}
      />
    );
  }
);

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
  autoHide: false,
  hideTimeout: 500,
};

export default Scrollbar;
