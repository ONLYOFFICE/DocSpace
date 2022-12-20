import * as React from "react";
import classnames from "classnames";
import ViewerLoading from "./viewer-loading";
import { useSwipeable } from "react-swipeable";

export default function ViewerImage(props) {
  const { dispatch, createAction, actionType } = props;
  const isMouseDown = React.useRef(false);
  const imgRef = React.useRef(null);
  const prePosition = React.useRef({
    x: 0,
    y: 0,
  });
  const [position, setPosition] = React.useState({
    x: 0,
    y: 0,
  });

  const tpCache = [];

  const handlers = useSwipeable({
    onSwiping: (e) => {
      const opacity = props.opacity - Math.abs(e.deltaX) / 500;

      const direction =
        Math.abs(e.deltaX) > Math.abs(e.deltaY) ? "horizontal" : "vertical";

      return dispatch(
        createAction(actionType.update, {
          left: direction === "horizontal" ? e.deltaX : 0,
          opacity: direction === "vertical" && e.deltaY > 0 ? opacity : 1,
          top:
            direction === "vertical"
              ? e.deltaY >= 0
                ? props.currentTop + e.deltaY
                : props.currentTop
              : props.currentTop,
          deltaY: direction === "vertical" ? (e.deltaY > 0 ? e.deltaY : 0) : 0,
          deltaX: direction === "horizontal" ? e.deltaX : 0,
        })
      );
    },
    onSwipedLeft: (e) => {
      if (e.deltaX <= -100) props.onNextClick();
    },
    onSwipedRight: (e) => {
      if (e.deltaX >= 100) props.onPrevClick();
    },
    onSwipedDown: (e) => {
      if (e.deltaY > 70) props.onMaskClick();
    },
    onSwiped: (e) => {
      if (Math.abs(e.deltaX) < 100) {
        return dispatch(
          createAction(actionType.update, {
            left: 0,
            top: props.currentTop,
            deltaY: 0,
            deltaX: 0,
            opacity: 1,
          })
        );
      }
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
    document.addEventListener("touchmove", (e) => {
      if (e.targetTouches.length === 2 && e.changedTouches.length === 2) {
        const point1 = tpCache.findLastIndex(
          (tp) => tp.identifier === e.targetTouches[0].identifier
        );
        const point2 = tpCache.findLastIndex(
          (tp) => tp.identifier === e.targetTouches[1].identifier
        );

        const startPointY1 = tpCache[point1].clientY;
        const startPointY2 = tpCache[point2].clientY;

        if (point1 >= 0 && point2 >= 0) {
          const diffX1 = tpCache[point1].clientX - e.targetTouches[0].clientX;
          const diffX2 = tpCache[point2].clientX - e.targetTouches[1].clientX;
          const diffY1 = tpCache[point1].clientY - e.targetTouches[0].clientY;
          const diffY2 = tpCache[point2].clientY - e.targetTouches[1].clientY;

          const zoom = (Math.abs(diffX1) + Math.abs(diffX2)) / 100;
          const imgCenterXY = props.getImageCenterXY();

          if (
            (startPointY1 < startPointY2 && diffX1 > diffX2) ||
            (startPointY1 > startPointY2 && diffX1 < diffX2)
          ) {
            props.handleZoom(
              imgCenterXY.x,
              imgCenterXY.y,
              1,
              0.01,
              imgRef.current.clientWidth,
              imgRef.current.clientHeight
            );
          } else {
            props.handleZoom(
              imgCenterXY.x,
              imgCenterXY.y,
              -1,
              zoom,
              imgRef.current.clientWidth
            );
          }
        } else {
          tpCache = [];
        }
      }
    });
  }, []);

  React.useEffect(() => {
    document.addEventListener("touchstart", (e) => {
      if (e.targetTouches.length === 2) {
        for (let i = 0; i < e.targetTouches.length; i++) {
          tpCache.push(e.targetTouches[i]);
        }
      }
    });
  }, []);

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

  let style = {
    zIndex: props.zIndex,
  };

  let imgNode = null;

  if (props.imgSrc !== "") {
    imgNode = (
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
      style={style}
      {...handlers}
    >
      {imgNode}
    </div>
  );
}
