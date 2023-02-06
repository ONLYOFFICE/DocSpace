import * as React from "react";
import classnames from "classnames";
import ViewerLoading from "./viewer-loading";
import { useSwipeable } from "../../react-swipeable";
import { isMobileOnly } from "react-device-detect";

import MobileViewer from "./mobile-viewer";
function ViewerImage(props) {
  const {
    dispatch,
    createAction,
    actionType,
    playlist,
    playlistPos,
    containerSize,
    setPanelVisible,
  } = props;

  const isMouseDown = React.useRef(false);
  const isZoomingRef = React.useRef(true);
  const imgRef = React.useRef(null);
  const dirRef = React.useRef("");
  const swipedRef = React.useRef(false);

  const unMountedRef = React.useRef(false);
  const startPostionRef = React.useRef({ x: 0, y: 0 });
  const isDoubleTapRef = React.useRef(false);

  React.useEffect(() => {
    unMountedRef.current = false;

    return () => (unMountedRef.current = true);
  }, []);

  const prePosition = React.useRef({
    x: 0,
    y: 0,
  });
  const [position, setPosition] = React.useState({
    x: 0,
    y: 0,
  });

  const CompareTo = (a, b) => {
    return Math.trunc(a) > Math.trunc(b);
  };

  const maybeAdjustImage = (point) => {
    const imageBounds = imgRef.current.getBoundingClientRect();
    const containerBounds = imgRef.current.parentNode.getBoundingClientRect();

    const originalWidth = imgRef.current.clientWidth;
    const widthOverhang = (imageBounds.width - originalWidth) / 2;

    const originalHeight = imgRef.current.clientHeight;
    const heightOverhang = (imageBounds.height - originalHeight) / 2;

    const isWidthOutContainer = imageBounds.width >= containerBounds.width;

    const isHeightOutContainer = imageBounds.height >= containerBounds.height;

    if (
      CompareTo(imageBounds.left, containerBounds.left) &&
      isWidthOutContainer
    ) {
      point.x = widthOverhang;
    } else if (
      CompareTo(containerBounds.right, imageBounds.right) &&
      isWidthOutContainer
    ) {
      point.x = -(imageBounds.width - containerBounds.width) + widthOverhang;
    } else if (!isWidthOutContainer) {
      point.x = (containerBounds.width - imageBounds.width) / 2 + widthOverhang;
    }

    if (
      CompareTo(imageBounds.top, containerBounds.top) &&
      isHeightOutContainer
    ) {
      point.y = heightOverhang;
    } else if (
      CompareTo(containerBounds.bottom, imageBounds.bottom) &&
      isHeightOutContainer
    ) {
      point.y = -(imageBounds.height - containerBounds.height) + heightOverhang;
    } else if (!isHeightOutContainer) {
      point.y =
        (containerBounds.height - imageBounds.height) / 2 + heightOverhang;
    }

    return point;
  };

  const handlers = useSwipeable({
    onSwipeStart: (e) => {
      dirRef.current = e.dir;
      swipedRef.current = false;
      startPostionRef.current = {
        x: props.left,
        y: props.top,
      };
    },
    onSwiping: (e) => {
      if (
        e.piching ||
        !isZoomingRef.current ||
        unMountedRef.current ||
        !imgRef.current ||
        isDoubleTapRef.current
      )
        return;

      let newPoint = {
        x: 0,
        y: 0,
      };

      const isFistImage = playlistPos === 0;
      const isLastImage = playlistPos === playlist.length - 1;

      const containerBounds = imgRef.current.parentNode.getBoundingClientRect();
      const imageBounds = imgRef.current.getBoundingClientRect();

      const deltaWidth = (containerBounds.width - imageBounds.width) / 2;
      const deltaHeight = (containerBounds.height - imageBounds.height) / 2;

      const originalWidth = imgRef.current.clientWidth;
      const widthOverhang = (imageBounds.width - originalWidth) / 2;

      const originalHeight = imgRef.current.clientHeight;
      const heightOverhang = (imageBounds.height - originalHeight) / 2;

      if (props.scaleX * props.scaleY <= 1) {
        switch (dirRef.current) {
          case "Down":
            newPoint.x = props.left;
            newPoint.y = e.deltaY + deltaHeight + heightOverhang;

            break;
          case "Left":
            newPoint.x = isLastImage
              ? props.left
              : e.deltaX + deltaWidth + widthOverhang;
            newPoint.y = props.top;
            break;
          case "Right":
            newPoint.x = isFistImage
              ? props.left
              : e.deltaX + deltaWidth + widthOverhang;
            newPoint.y = props.top;
            break;
          default:
            newPoint.x = props.left;
            newPoint.y = props.top;
            break;
        }
      } else {
        const isWidthOutContainer = imageBounds.width >= containerBounds.width;

        const isHeightOutContainer =
          imageBounds.height >= containerBounds.height;

        const [vx, vy] = e.vxvy;

        const absVx = Math.abs(vx) > 0 ? Math.abs(vx) + 1 : 0;
        const absVy = Math.abs(vy) > 0 ? Math.abs(vy) + 1 : 0;

        const isImageHeightInsideContainer =
          imageBounds.top > containerBounds.top &&
          containerBounds.bottom > imageBounds.bottom;
        const isImageWidhtInsideContainer =
          imageBounds.left > containerBounds.left &&
          containerBounds.right > imageBounds.right;

        const left = imageBounds.left + e.deltaX * absVx;
        const right = imageBounds.right + e.deltaX * absVx;

        if (isWidthOutContainer) {
          if (left > containerBounds.left) {
            newPoint.x = widthOverhang;
          } else if (right < containerBounds.right) {
            newPoint.x =
              -(imageBounds.width - containerBounds.width) + widthOverhang;
          } else {
            newPoint.x = e.deltaX * absVx + startPostionRef.current.x;
          }
        } else if (isImageWidhtInsideContainer) {
          newPoint.x = props.left;
        } else {
          newPoint.x = e.deltaX * absVx + startPostionRef.current.x;
        }

        const top = imageBounds.top + e.deltaY * absVy;
        const bottom = imageBounds.bottom + e.deltaY * absVy;

        if (isHeightOutContainer) {
          if (top > containerBounds.top) {
            newPoint.y = heightOverhang;
          } else if (bottom < containerBounds.bottom) {
            newPoint.y =
              -(imageBounds.height - containerBounds.height) + heightOverhang;
          } else {
            newPoint.y = e.deltaY * absVy + startPostionRef.current.y;
          }
        } else if (isImageHeightInsideContainer) {
          newPoint.y = props.top;
        } else {
          newPoint.y = e.deltaY * absVy + startPostionRef.current.y;
        }
      }

      const opacity =
        props.scaleX !== 1 && props.scaleY !== 1
          ? 1
          : dirRef.current === "Down"
          ? 2 -
            (imageBounds.height / 2 + props.top) / (containerBounds.height / 2)
          : 1;

      const direction =
        Math.abs(e.deltaX) > Math.abs(e.deltaY) ? "horizontal" : "vertical";

      return dispatch(
        createAction(actionType.update, {
          left: newPoint.x,
          top: newPoint.y,
          opacity: direction === "vertical" && e.deltaY > 0 ? opacity : 1,
          deltaX: 0,
          deltaY: 0,
        })
      );
    },
    onSwipedLeft: (e) => {
      if (
        (props.scaleX !== 1 && props.scaleY !== 1) ||
        e.piching ||
        !isZoomingRef.current
      )
        return;
      if (e.deltaX <= -100 && playlistPos !== playlist.length - 1) {
        swipedRef.current = true;
        props.onNextClick();
      }
    },
    onSwipedRight: (e) => {
      if (
        (props.scaleX !== 1 && props.scaleY !== 1) ||
        e.piching ||
        !isZoomingRef.current
      )
        return;

      if (e.deltaX >= 100 && playlistPos !== 0) {
        swipedRef.current = true;
        props.onPrevClick();
      }
    },
    onSwipedDown: (e) => {
      if (unMountedRef.current) return;

      if (e.deltaY > 200 && props.scaleX * props.scaleY === 1) {
        swipedRef.current = true;
        props.onMaskClick();
      }
    },
    onTap: (e) => {
      setPanelVisible((visible) => !visible);
    },
    onSwiped: (e) => {
      if (unMountedRef.current || isDoubleTapRef.current) return;
      console.log("onSwiped");
      let Point = {
        x: props.left,
        y: props.top,
      };
      dirRef.current = "";

      setTimeout(() => {
        if (unMountedRef.current || swipedRef.current) return;
        const newPoint = maybeAdjustImage(Point);
        return dispatch(
          createAction(actionType.update, {
            left: newPoint.x,
            top: newPoint.y,
            deltaX: 0,
            deltaY: 0,
            opacity: 1,
          })
        );
      }, 200);
    },
    onTouchEndOrOnMouseUp: () => {
      dirRef.current = "";
      isDoubleTapRef.current = false;
    },
  });

  React.useEffect(() => {
    return () => {
      bindEvent(true);
      bindWindowResizeEvent(true);
    };
  }, []);

  React.useEffect(() => {
    bindWindowResizeEvent();

    return () => {
      bindWindowResizeEvent(true);
    };
  });

  React.useEffect(() => {
    if (props.visible && props.drag) {
      bindEvent();
    }
    if (!props.visible && props.drag) {
      handleMouseUp({});
    }
    return () => {
      bindEvent(true);
    };
  }, [props.drag, props.visible]);

  React.useEffect(() => {
    let diffX = position.x - prePosition.current.x;
    let diffY = position.y - prePosition.current.y;
    prePosition.current = {
      x: position.x,
      y: position.y,
    };
    props.onChangeImgState(
      props.width,
      props.height,
      props.top + diffY,
      props.left + diffX
    );
  }, [position]);

  function handleResize(e) {
    props.onResize();
  }

  function handleMouseDown(e) {
    if (e.button !== 0) {
      return;
    }
    if (!props.visible || !props.drag) {
      return;
    }
    e.preventDefault();
    e.stopPropagation();
    isMouseDown.current = true;
    prePosition.current = {
      x: e.nativeEvent.clientX,
      y: e.nativeEvent.clientY,
    };
  }

  const handleMouseMove = (e) => {
    if (isMouseDown.current) {
      setPosition({
        x: e.clientX,
        y: e.clientY,
      });
    }
  };

  function handleResize(e) {
    props.onResize();
  }

  function handleMouseUp(e) {
    isMouseDown.current = false;
  }

  function onClose(e) {
    if (e.target === imgRef.current || e.nativeEvent.pointerType === "touch")
      return;
    props.onMaskClick();
  }

  function bindWindowResizeEvent(remove) {
    let funcName = "addEventListener";
    if (remove) {
      funcName = "removeEventListener";
    }
    window[funcName]("resize", handleResize, false);
  }

  function bindEvent(remove) {
    let funcName = "addEventListener";
    if (remove) {
      funcName = "removeEventListener";
    }

    document[funcName]("click", handleMouseUp, false);
    document[funcName]("mousemove", handleMouseMove, false);
  }

  let imgStyle = {
    width: `${props.width}px`,
    height: `${props.height}px`,
    opacity: `${props.opacity}`,
    transition: `${props.withTransition ? "all .26s ease-out" : "none"}`,
    transform: `
translateX(${props.left !== null ? props.left + "px" : "auto"}) translateY(${
      props.top
    }px)
    rotate(${props.rotate}deg) scaleX(${props.scaleX}) scaleY(${props.scaleY})`,
  };

  const imgClass = classnames(`${props.prefixCls}-image`, {
    drag: props.drag,
    [`${props.prefixCls}-image-transition`]: !isMouseDown.current,
  });

  let styleIndex = {
    zIndex: props.zIndex,
  };

  let imgNode = null;

  if (props.imgSrc !== "") {
    imgNode = isMobileOnly ? (
      <MobileViewer
        className={imgClass}
        src={props.imgSrc}
        width={props.width}
        height={props.height}
        left={props.left}
        top={props.top}
        onPrev={props.onPrevClick}
        onNext={props.onNextClick}
        onMask={props.onMaskClick}
        isFistImage={playlistPos === 0}
        isLastImage={playlistPos === playlist.length - 1}
        setPanelVisible={setPanelVisible}
      />
    ) : (
      <img
        className={imgClass}
        src={props.imgSrc}
        style={imgStyle}
        ref={imgRef}
        onMouseDown={handleMouseDown}
      />
    );
  }
  if (props.loading) {
    imgNode = (
      <div
        style={{
          display: "flex",
          height: `${window.innerHeight}px`,
          justifyContent: "center",
          alignItems: "center",
        }}
      >
        <ViewerLoading />
      </div>
    );
  }

  if (isMobileOnly) {
    return (
      <div
        className={`${props.prefixCls}-canvas`}
        onClick={onClose}
        style={styleIndex}
      >
        {imgNode}
      </div>
    );
  }

  return (
    <div
      className={`${props.prefixCls}-canvas`}
      onClick={onClose}
      style={styleIndex}
      {...handlers}
    >
      {imgNode}
    </div>
  );
}

export default React.memo(ViewerImage);
