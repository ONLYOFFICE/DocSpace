import React from "react";
import { useGesture } from "@use-gesture/react";
import { useSpring, animated } from "@react-spring/web";
import styled from "styled-components";

const ImageWrapper = styled.div`
  width: 100%;
  height: 100%;
  overflow: hidden;
  touch-action: none;
`;

function MobileViewer({
  src,
  width,
  height,
  className,
  left,
  top,
  onPrev,
  onNext,
  onMask,
  isFistImage,
  isLastImage,
  setPanelVisible,
}) {
  const imgRef = React.useRef(null);
  const containerRef = React.useRef(null);

  const unmountRef = React.useRef(false);

  const lastTapTimeRef = React.useRef(0);
  const isDoubleTapRef = React.useRef(false);
  const setTimeoutIDTapRef = React.useRef();
  const startAngleRef = React.useRef(0);

  const [scale, setScale] = React.useState(1);

  const [style, api] = useSpring(() => ({
    x: left,
    y: top,
    scale: 1,
    rotate: 0,
    opacity: 1,
    width: width,
    height: height,
    touchAction: "none",
    willChange: "transform, opacity, contents",
  }));
  React.useEffect(() => {
    const point = maybeAdjustImage({
      x: left,
      y: top,
    });

    api.start({ ...point });
  }, [left, top]);

  React.useEffect(()=>{
    api.set({
      width,
      height,
      x:left,
      y:top,
    })
  },[height, width])

  React.useEffect(() => {
    unmountRef.current = false;

    return () => {
      setTimeoutIDTapRef.current && clearTimeout(setTimeoutIDTapRef.current);
      unmountRef.current = true;
    };
  }, []);

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

  useGesture(
    {
      onDragStart: ({ pinching }) => {
        if (pinching) return;

        setScale(style.scale.get());
      },
      onDrag: ({
        offset: [dx, dy],
        movement: [mdx, mdy],
        cancel,
        pinching,
        canceled,
      }) => {
        if (isDoubleTapRef.current || unmountRef.current) {
          isDoubleTapRef.current = false;
          return;
        }

        if (pinching || canceled) cancel();

        api.start({
          x:
            style.scale.get() === 1 &&
            ((isFistImage && mdx > 0) || (isLastImage && mdx < 0))
              ? style.x.get()
              : dx,
          y: dy,
          opacity:
            style.scale.get() === 1 && mdy > 0
              ? imgRef.current.height / 10 / mdy
              : style.opacity.get(),
          immediate: true,
          config: {
            duration: 0,
          },
        });
      },

      onDragEnd: ({ cancel, canceled, movement: [mdx, mdy] }) => {
        if (unmountRef.current) {
          return;
        }

        if (canceled || isDoubleTapRef.current) {
          isDoubleTapRef.current = false;
          cancel();
        }
        if (style.scale.get() === 1) {
          if (mdx < -imgRef.current.width / 4) {
            return onNext();
          } else if (mdx > imgRef.current.width / 4) {
            return onPrev();
          }
          if (mdy > 150) {
            return onMask();
          }
        }

        const newPoint = maybeAdjustImage({
          x: style.x.get(),
          y: style.y.get(),
        });

        api.start({
          ...newPoint,
          opacity: 1,
        });
      },

      onPinchStart: ({ event, cancel }) => {
        if (event.target === containerRef.current) {
          cancel();
        } else {
          const roundedAngle = Math.round(style.rotate.get());
          startAngleRef.current = roundedAngle - (roundedAngle % 90);
        }
      },

      onPinch: ({
        origin: [ox, oy],
        offset: [dScale, dRotate],
        movement: [mScale],
        memo,
        first,
        canceled,
        event,
      }) => {
        if (canceled || event.target === containerRef.current) return;

        if (first) {
          const {
            width,
            height,
            x,
            y,
          } = imgRef.current.getBoundingClientRect();
          const tx = ox - (x + width / 2);
          const ty = oy - (y + height / 2);
          memo = [style.x.get(), style.y.get(), tx, ty];
        }

        const x = memo[0] - (mScale - 1) * memo[2];
        const y = memo[1] - (mScale - 1) * memo[3];

        api.start({
          x,
          y,
          scale: dScale,
          rotate: dRotate,
          delay: 0,
        });
        return memo;
      },
      onPinchEnd: ({
        movement: [, mRotate],
        direction: [, dirRotate],
        canceled,
      }) => {
        if (unmountRef.current || canceled) {
          return;
        }
        const rotate =
          Math.abs(mRotate / 90) > 1 / 3
            ? Math.trunc(
                startAngleRef.current +
                  90 *
                    Math.max(Math.trunc(Math.abs(mRotate) / 90), 1) *
                    dirRotate
              )
            : startAngleRef.current;

        const newPoint = maybeAdjustImage({
          x: style.x.get(),
          y: style.y.get(),
        });

        api.start({
          rotate,
          ...newPoint,
          delay: 0,
          onResolve: () => {
            const newPoint = maybeAdjustImage({
              x: style.x.get(),
              y: style.y.get(),
            });

            api.start({
              ...newPoint,
              immediate: true,
              delay: 0,
            });
          },
        });
      },
      onClick: () => {
        const time = new Date().getTime();

        if (time - lastTapTimeRef.current < 300) {
          //on Double Tap
          lastTapTimeRef.current = 0;
          isDoubleTapRef.current = true;
          const imageWidth = imgRef.current.width;
          const imageHeight = imgRef.current.height;
          const containerBounds = imgRef.current.parentNode.getBoundingClientRect();

          const deltaWidth = (containerBounds.width - imageWidth) / 2;
          const deltaHeight = (containerBounds.height - imageHeight) / 2;
          api.start({
            scale: 1,
            x: deltaWidth,
            y: deltaHeight,
            rotate: 0,
            immediate: true,
          });

          clearTimeout(setTimeoutIDTapRef.current);
        } else {
          lastTapTimeRef.current = time;
          setTimeoutIDTapRef.current = setTimeout(() => {
            // onTap

            setPanelVisible((visible) => {
              let display = visible;
              const displayVisible =
                JSON.parse(localStorage.getItem("displayVisible")) || null;

              if (displayVisible !== null) {
                display = !displayVisible;
              }

              localStorage.setItem("displayVisible", display);
              return !visible;
            });
          }, 300);
        }
      },
    },
    {
      drag: {
        from: () => [style.x.get(), style.y.get()],
        axis: scale === 1 ? "lock" : undefined,
        bounds: () => {
          if (style.scale.get() === 1) return undefined;

          const imageBounds = imgRef.current.getBoundingClientRect();
          const containerBounds = imgRef.current.parentNode.getBoundingClientRect();

          const originalWidth = imgRef.current.clientWidth;
          const widthOverhang = (imageBounds.width - originalWidth) / 2;

          const originalHeight = imgRef.current.clientHeight;
          const heightOverhang = (imageBounds.height - originalHeight) / 2;

          const isWidthOutContainer =
            imageBounds.width >= containerBounds.width;

          const isHeightOutContainer =
            imageBounds.height >= containerBounds.height;

          const bounds = {
            right: isWidthOutContainer
              ? widthOverhang
              : containerBounds.width - imageBounds.width + widthOverhang,
            left: isWidthOutContainer
              ? -(imageBounds.width - containerBounds.width) + widthOverhang
              : widthOverhang,
            bottom: isHeightOutContainer
              ? heightOverhang
              : containerBounds.height - imageBounds.height + heightOverhang,
            top: isHeightOutContainer
              ? -(imageBounds.height - containerBounds.height) + heightOverhang
              : heightOverhang,
          };
          return bounds;
        },
      },
      pinch: {
        scaleBounds: { min: 0.5, max: 5 },
        rubberband: false,
        from: () => [style.scale.get(), style.rotate.get()],
        threshold: [0.1, 5],
      },

      target: containerRef,
    }
  );

  return (
    <ImageWrapper ref={containerRef}>
      <animated.img
        src={src}
        className={className}
        ref={imgRef}
        style={style}
      />
    </ImageWrapper>
  );
}

export default MobileViewer;
