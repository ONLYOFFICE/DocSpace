import * as React from "react";
import classnames from "classnames";
import ViewerLoading from "./viewer-loading";
import { useSwipeable } from "../../react-swipeable";
import { isMobile } from "react-device-detect";

export default function ViewerImage(props) {
  const {
    dispatch,
    createAction,
    actionType,
    playlist,
    playlistPos,
    containerSize,
  } = props;
  const navMenuHeight = 53;
  const isMouseDown = React.useRef(false);
  const isZoomingRef = React.useRef(true);
  const imgRef = React.useRef(null);
  const unMountedRef = React.useRef(false);

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
      point.y = heightOverhang + navMenuHeight / 2;
    } else if (
      CompareTo(
        containerBounds.bottom,
        imageBounds.bottom + navMenuHeight / 2
      ) &&
      isHeightOutContainer
    ) {
      point.y =
        -(imageBounds.height - containerBounds.height - navMenuHeight / 2) +
        heightOverhang;
    } else if (!isHeightOutContainer) {
      point.y =
        (containerBounds.height - imageBounds.height) / 2 +
        heightOverhang +
        navMenuHeight / 2;
    }

    return point;
  };

  const handlers = useSwipeable({
    onSwiping: (e) => {
      if (
        e.piching ||
        !isZoomingRef.current ||
        unMountedRef.current ||
        !imgRef.current
      )
        return;

      const opacity =
        props.scaleX !== 1 && props.scaleY !== 1
          ? 1
          : props.opacity - Math.abs(e.deltaX) / 500;

      const direction =
        Math.abs(e.deltaX) > Math.abs(e.deltaY) ? "horizontal" : "vertical";

      let Point = {
        x: props.left + (e.deltaX * props.scaleX) / 15,
        y: props.top + (e.deltaY * props.scaleY) / 15,
      };

      const newPoint = maybeAdjustImage(Point);

      console.log(newPoint);

      // let swipeLeft = 0;

      // const isEdgeImage =
      //   (playlistPos === 0 && e.deltaX > 0) ||
      //   (playlistPos === playlist.length - 1 && e.deltaX < 0);

      // if (props.width < window.innerWidth) {
      //   swipeLeft =
      //     direction === "horizontal"
      //       ? isEdgeImage
      //         ? props.left
      //         : e.deltaX > 0
      //         ? props.left + 2
      //         : props.left - 2
      //       : props.left;
      // } else {
      //   swipeLeft =
      //     direction === "horizontal" ? (isEdgeImage ? 0 : e.deltaX) : 0;
      // }

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
      if (e.deltaX <= -100) props.onNextClick();
    },
    onSwipedRight: (e) => {
      if (
        (props.scaleX !== 1 && props.scaleY !== 1) ||
        e.piching ||
        !isZoomingRef.current
      )
        return;
      if (e.deltaX >= 100) props.onPrevClick();
    },
    onSwipedDown: (e) => {
      // if (unMountedRef.current) return;
      // console.log("onSwiped");
      // let Point = {
      //   x: props.left + (e.deltaX * props.scaleX) / 15,
      //   y: props.top + (e.deltaY * props.scaleY) / 15,
      // };
      // const newPoint = maybeAdjustImage(props.scaleX, props.scaleY, Point);
      // return dispatch(
      //   createAction(actionType.update, {
      //     left: newPoint.x,
      //     top: newPoint.y,
      //     deltaX: 0,
      //     deltaY: 0,
      //     opacity: 1,
      //   })
      // );
    },

    onZoom: (event) => {
      if (unMountedRef.current || !imgRef.current) return;

      const { handleZoom, handleResetZoom } = props;
      const { scale, middleSegment } = event;

      const zoomCondition = scale > 1;
      const direct = zoomCondition ? 1 : -1;
      const zoom = Math.abs(1 - scale) * 50;

      if (zoom < 0.25) return;

      if (isZoomingRef.current) {
        isZoomingRef.current = false;
        const [scaleX] = handleZoom(
          middleSegment.x,
          middleSegment.y,
          direct,
          zoom
        );

        setTimeout(() => {
          if (scaleX < 1) handleResetZoom();
          isZoomingRef.current = true;
        }, 200);
      }
    },
    onTouchEndOrOnMouseUp: (e) => {
      if (
        (props.scaleX !== 1 && props.scaleY !== 1) ||
        e.piching ||
        !isZoomingRef.current
      )
        return;
      if (e.deltaY > 70) props.onMaskClick();
    },
    onSwiped: () => {
      console.log("onTouchEndOrOnMouseUp");
      let Point = {
        x: props.left,
        y: props.top,
      };
      setTimeout(() => {
        if (unMountedRef.current) return;
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
    if (e.target === imgRef.current) return;
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
    top: isMobile ? navMenuHeight : 0,
  };

  let imgNode = null;

  if (props.imgSrc !== "") {
    imgNode = isMobile ? (
      <img
        className={imgClass}
        src={props.imgSrc}
        ref={imgRef}
        style={{
          position: "absolute",
          width: `${props.width}px`,
          height: `${props.height}px`,
          opacity: `${props.opacity}`,
          transition: "all .5s ease-out",
          top: props.top - navMenuHeight / 2,
          left: props.left,
          transform: `rotate(${props.rotate}deg) scaleX(${props.scaleX}) scaleY(${props.scaleY})`,
          willChange: "transform",
        }}
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
