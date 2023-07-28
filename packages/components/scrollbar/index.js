import React, { useCallback, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

import { classNames } from "../utils/classNames";
import { isMobile } from "@docspace/components/utils/device";
import StyledScrollbar from "./styled-scrollbar";
import { useTheme } from "styled-components";

const scrollbarTypes = {
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

const Scrollbar = React.forwardRef((props, ref) => {
  const {
    id,
    onScroll,
    autoHide,
    hideTrackTimer,
    scrollclass,
    stype,
    ...rest
  } = props;

  const [isScrolling, setIsScrolling] = useState();
  const [isMouseOver, setIsMouseOver] = useState();
  const { interfaceDirection } = useTheme();
  const timerId = useRef();

  const isRtl = interfaceDirection === "rtl";

  const scrollbarType = scrollbarTypes[stype] ?? {};

  const tracksCommonStyles = {
    opacity: autoHide && !isScrolling ? 0 : 1,
    transition: "opacity 0.4s ease-in-out",
  };

  const showTrack = useCallback(() => {
    clearTimeout(timerId.current);
    setIsScrolling(true);
  }, [timerId]);

  const hideTrack = useCallback(() => {
    timerId.current = setTimeout(() => {
      setIsScrolling(false);
    }, hideTrackTimer);
  }, [timerId, hideTrackTimer]);

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
  const renderScroller = useCallback(
    (libProps) => {
      const { elementRef, ...restLibProps } = libProps;
      return (
        <div
          {...restLibProps}
          id={id}
          className={classNames("scroller", scrollclass)}
          ref={elementRef}
          onScroll={onScroll}
        />
      );
    },
    [id, onScroll, scrollclass]
  );

  useEffect(() => {
    return () => clearTimeout(timerId.current);
  }, []);

  return (
    <StyledScrollbar
      {...rest}
      disableTracksWidthCompensation
      rtl={isRtl}
      ref={ref}
      onScrollStop={onScrollStop}
      onScrollStart={onScrollStart}
      scrollerProps={{ renderer: renderScroller }}
      contentProps={{
        tabIndex: -1,
        className: "scroll-body",
        style: {
          ...scrollbarType.content,
          paddingRight: !isRtl && (isMobile() ? "8px" : "17px"),
          paddingLeft: isRtl && (isMobile() ? "8px" : "17px"),
        },
      }}
      thumbYProps={{
        className: "nav-thumb-vertical",
        style: scrollbarType.thumbV,
      }}
      thumbXProps={{
        className: "nav-thumb-horizontal",
        style: scrollbarType.thumbH,
      }}
      // Add 1px margin to vertical track to avoid scrollbar lib crashing when event.clientX equals 0
      trackYProps={{
        style: {
          ...scrollbarType.trackV,
          ...tracksCommonStyles,
          marginLeft: isRtl ? "1px" : "0",
          marginRight: isRtl ? "0" : "1px",
        },
        onMouseEnter,
        onMouseLeave,
      }}
      trackXProps={{
        style: {
          ...scrollbarType.trackH,
          ...tracksCommonStyles,
          direction: "ltr",
        },
        onMouseEnter,
        onMouseLeave,
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
  /** Enable tracks auto hiding.  */
  autoHide: PropTypes.bool,
  /** Track auto hiding delay in ms.  */
  hideTrackTimer: PropTypes.number,
};

Scrollbar.defaultProps = {
  stype: "mediumBlack",
  autoHide: false,
  hideTrackTimer: 500,
};

export default Scrollbar;
