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
