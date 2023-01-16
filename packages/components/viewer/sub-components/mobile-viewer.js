import React from "react";
import { useGesture } from "@use-gesture/react";

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
}) {
  const imgRef = React.useRef(null);
  const lastTapTime = React.useRef(0);
  const unmount = React.useRef(false);
  const isDoubleTapRef = React.useRef(false);

  const [crop, setCrop] = React.useState({
    x: left,
    y: top,
    scale: 1,
    rotate: 0,
    opacity: 1,
  });

  React.useEffect(() => {
    unmount.current = false;

    return () => {
      unmount.current = true;
    };
  }, []);

  React.useEffect(() => {
    setCrop((pre) => ({
      ...pre,
      x: left,
      y: top,
    }));
  }, [left, top]);

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
      onDragStart: () => {
        const time = new Date().getTime();

        if (time - lastTapTime.current < 300) {
          lastTapTime.current = 0;
          isDoubleTapRef.current = true;
          const imageWidth = imgRef.current.width;
          const imageHeight = imgRef.current.height;
          const containerBounds = imgRef.current.parentNode.getBoundingClientRect();

          const deltaWidth = (containerBounds.width - imageWidth) / 2;
          const deltaHeight = (containerBounds.height - imageHeight) / 2;

          setCrop((pre) => ({
            ...pre,
            scale: 1,
            rotate: 0,
            x: deltaWidth,
            y: deltaHeight,
          }));
        } else {
          lastTapTime.current = time;
        }
      },
      onDrag: ({
        offset: [dx, dy],
        movement: [mdx, mdy],
        cancel,
        pinching,
        canceled,
      }) => {
        if (isDoubleTapRef.current || unmount.current) {
          isDoubleTapRef.current = false;
          return;
        }

        if (pinching || canceled) cancel();

        setCrop((crop) => ({
          ...crop,
          x:
            crop.scale === 1 &&
            ((isFistImage && mdx > 0) || (isLastImage && mdx < 0))
              ? crop.x
              : dx,
          y: dy,
          opacity:
            crop.scale === 1 && mdy > 0
              ? imgRef.current.height / 10 / mdy
              : crop.opacity,
        }));
      },

      onDragEnd: ({ cancel, canceled, movement: [mdx, mdy] }) => {
        if (unmount.current) {
          return;
        }

        if (canceled || isDoubleTapRef.current) {
          isDoubleTapRef.current = false;
          cancel();
        }

        if (crop.scale === 1) {
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
          x: crop.x,
          y: crop.y,
        });

        setCrop((pre) => ({ ...pre, ...newPoint, opacity: 1 }));
      },

      onPinch: ({
        origin: [ox, oy],
        offset: [dScale, dRotate],
        movement: [mScale, mRotate],
        memo,
        first,
      }) => {
        if (first) {
          const {
            width,
            height,
            x,
            y,
          } = imgRef.current.getBoundingClientRect();
          const tx = ox - (x + width / 2);
          const ty = oy - (y + height / 2);
          memo = [crop.x, crop.y, tx, ty];
        }

        const x = memo[0] - (mScale - 1) * memo[2];
        const y = memo[1] - (mScale - 1) * memo[3];

        setCrop((pre) => ({
          ...pre,
          x,
          y,
          scale: dScale,
          rotate: dRotate,
        }));

        return memo;
      },
      onPinchEnd: (event) => {
        console.log("onPinchEnd", event, unmount.current);

        if (unmount.current) {
          return;
        }

        const newPoint = maybeAdjustImage({
          x: crop.x,
          y: crop.y,
        });
        setCrop((pre) => ({ ...pre, ...newPoint }));
      },
    },
    {
      drag: {
        from: () => [crop.x, crop.y],
        axis: crop.scale === 1 ? "lock" : undefined,
        bounds: () => {
          if (crop.scale === 1) return undefined;

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
        from: () => [crop.scale, crop.rotate],
        threshold: [0.1, 5],
      },

      target: imgRef,
    }
  );

  return (
    <img
      src={src}
      className={className}
      ref={imgRef}
      style={{
        width: `${width}px`,
        height: `${height}px`,
        opacity: `${crop.opacity}`,
        // transition: "all 0.2s linear",
        transform: `translate(${crop.x}px, ${crop.y}px) rotate(${crop.rotate}deg) scale(${crop.scale})`,
        willChange: "transform",
        touchAction: "none",
        MozUserSelect: "none",
        userSelect: "none",
      }}
    />
  );
}

export default MobileViewer;
