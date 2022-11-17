import * as React from "react";
import classnames from "classnames";
import ViewerLoading from "./viewer-loading";
import { useSwipeable } from "react-swipeable";

export default function ViewerImage(props) {
  const isMouseDown = React.useRef(false);
  const prePosition = React.useRef({
    x: 0,
    y: 0,
  });
  const [position, setPosition] = React.useState({
    x: 0,
    y: 0,
  });

  const handlers = useSwipeable({
    onSwipedLeft: (e) => {
      props.onNextClick();
    },
    onSwipedRight: (e) => {
      props.onPrevClick();
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
    <div className={`${props.prefixCls}-canvas`} style={style} {...handlers}>
      {imgNode}
    </div>
  );
}
