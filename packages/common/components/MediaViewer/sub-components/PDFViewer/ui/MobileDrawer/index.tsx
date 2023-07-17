import React, { useEffect, useRef, useState } from "react";

import { useDrag } from "@use-gesture/react";
import { useSpring, config } from "@react-spring/web";

import Bookmarks from "../Bookmarks";

import {
  MobileDrawerContainer,
  MobileDrawerHeader,
  MobileDrawerWrapper,
} from "./MobileDrawer.styled";
import { Thumbnails } from "../Sidebar/Sidebar.styled";

import MobileDrawerProps from "./MobileDrawer.props";

import ViewTilesIcon from "PUBLIC_DIR/images/view-tiles.react.svg";
import ViewRowsIcon from "PUBLIC_DIR/images/view-rows.react.svg";
import CrossIcon from "PUBLIC_DIR/images/cross.react.svg";

function MobileDrawer({
  bookmarks,
  isOpenMobileDrawer,
  navigate,
  resizePDFThumbnail,
  setIsOpenMobileDrawer,
}: MobileDrawerProps) {
  const [height, setheight] = useState(window.innerHeight);

  const containerRef = useRef<HTMLDivElement>(null);

  const [style, api] = useSpring(() => ({ y: height, opacity: 1 }));

  const [toggle, setToggle] = useState<boolean>(false);

  useEffect(() => {
    if (isOpenMobileDrawer) open();
  }, [isOpenMobileDrawer]);

  useEffect(() => {
    document.addEventListener("touchstart", handleClickOutside);
    window.addEventListener("resize", handleResize);

    return () => {
      // Unbind the event listener on clean up
      document.removeEventListener("touchstart", handleClickOutside);
      window.removeEventListener("resize", handleResize);
    };
  }, [isOpenMobileDrawer]);

  const handleToggle = () => {
    setToggle((prev) => !prev);
  };

  const handleResize = () => {
    const innerHeight = window.innerHeight;

    setheight(innerHeight);
    if (isOpenMobileDrawer) {
      open(false, innerHeight);

      setTimeout(() => {
        resizePDFThumbnail();
      });
    } else close(0, innerHeight);
  };

  const open = (canceled = false, innerHeight?: number) => {
    api.start({
      y: (innerHeight ?? height) * 0.2,
      opacity: 1,
      immediate: false,
      config: canceled ? config.wobbly : config.stiff,
    });
  };

  const close = (velocity = 0, innerHeight?: number) => {
    api.start({
      y: innerHeight ?? height,
      opacity: 0,
      immediate: false,
      config: { ...config.stiff, velocity },
    });
    setIsOpenMobileDrawer(false);
  };

  const bind = useDrag(
    ({
      last,
      velocity: [, vy],
      direction: [, dy],
      movement: [, my],
      cancel,
      canceled,
    }) => {
      if (my < -70) {
        cancel();
      }

      if (last) {
        my > height * 0.2 || (vy > 0.5 && dy > 0) ? close(vy) : open(canceled);
      } else {
        api.start({
          y: my + height * 0.2,
          opacity: Math.max(1 - my / height, 0),
          immediate: true,
        });
      }
    },
    {
      from: () => [0, style.y.get()],
      filterTaps: true,
      bounds: { top: 0 },
      rubberband: true,
    }
  );

  const handleClickOutside = (event: TouchEvent) => {
    if (
      isOpenMobileDrawer &&
      containerRef.current &&
      event.target instanceof Node &&
      !containerRef.current.contains(event.target)
    ) {
      close();
    }
  };

  const handleClose = (event: TouchEvent) => {
    event.stopPropagation();
    close();
  };

  const visibility = isOpenMobileDrawer ? "visible" : "hidden";

  return (
    <MobileDrawerContainer ref={containerRef}>
      <MobileDrawerWrapper
        style={{
          height,
          visibility,
          ...style,
        }}
      >
        <MobileDrawerHeader {...bind()}>
          {bookmarks.length > 0 &&
            React.createElement(toggle ? ViewTilesIcon : ViewRowsIcon, {
              onClick: handleToggle,
            })}
          <CrossIcon
            className="mobile-drawer_cross-icon"
            onClick={handleClose}
          />
        </MobileDrawerHeader>
        <div style={{ height: height * 0.8 - 64 }}>
          {toggle && <Bookmarks bookmarks={bookmarks} navigate={navigate} />}
          <Thumbnails id="viewer-thumbnail" visible={!toggle} />
        </div>
      </MobileDrawerWrapper>
    </MobileDrawerContainer>
  );
}

export default MobileDrawer;
