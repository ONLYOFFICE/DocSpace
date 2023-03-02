import { useGesture } from "@use-gesture/react";
import { isMobile, isDesktop } from "react-device-detect";
import { useSpring, config, easings } from "@react-spring/web";
import React, {
  SyntheticEvent,
  useEffect,
  useLayoutEffect,
  useRef,
  useState,
} from "react";

import ViewerLoader from "../ViewerLoader";
import ImageViewerToolbar from "../ImageViewerToolbar";

import {
  Image,
  ImageViewerContainer,
  ImageWrapper,
} from "./ImageViewer.styled";

import ImageViewerProps from "./ImageViewer.props";
import {
  ImperativeHandle,
  ToolbarItemType,
} from "../ImageViewerToolbar/ImageViewerToolbar.props";
import { ToolbarActionType, KeyboardEventKeys } from "../../helpers";

const MaxScale = 5;
const MinScale = 0.5;
const DefaultSpeedScale = 0.5;
const RatioWheel = 400;

type BoundsType = {
  top: number;
  bottom: number;
  right: number;
  left: number;
};

function ImageViewer({
  src,
  onPrev,
  onNext,
  onMask,
  isFistImage,
  isLastImage,
  panelVisible,
  setPanelVisible,
  generateContextMenu,
  setIsOpenContextMenu,
  resetToolbarVisibleTimer,
  mobileDetails,
  toolbar,
}: ImageViewerProps) {
  const imgRef = useRef<HTMLImageElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const unmountRef = useRef<boolean>(false);

  const lastTapTimeRef = useRef<number>(0);
  const isDoubleTapRef = useRef<boolean>(false);
  const setTimeoutIDTapRef = useRef<NodeJS.Timeout>();
  const startAngleRef = useRef<number>(0);
  const scaleRef = useRef<number>(1);
  const toolbarRef = useRef<ImperativeHandle>(null);

  const [scale, setScale] = useState(1);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [backgroundBlack, setBackgroundBlack] = useState<boolean>(() => false);

  const [style, api] = useSpring(() => ({
    width: 0,
    height: 0,
    x: 0,
    y: 0,
    scale: 5,
    rotate: 0,
    opacity: 1,
  }));

  useEffect(() => {
    unmountRef.current = false;

    window.addEventListener("resize", Resize);

    return () => {
      setTimeoutIDTapRef.current && clearTimeout(setTimeoutIDTapRef.current);
      window.removeEventListener("resize", Resize);
      unmountRef.current = true;
    };
  }, []);

  useLayoutEffect(() => {
    if (unmountRef.current) return;
    setIsLoading(true);
  }, [src]);

  useEffect(() => {
    document.addEventListener("keydown", onKeyDown);

    return () => {
      document.removeEventListener("keydown", onKeyDown);
    };
  }, []);

  function Resize() {
    if (!imgRef.current || isLoading) return;

    const naturalWidth = imgRef.current.naturalWidth;
    const naturalHeight = imgRef.current.naturalHeight;

    const imagePositionAndSize = getImagePositionAndSize(
      naturalWidth,
      naturalHeight
    );
    if (imagePositionAndSize) {
      api.set(imagePositionAndSize);
    }
  }

  const RestartScaleAndSize = () => {
    if (!imgRef.current || style.rotate.isAnimating) return;

    const naturalWidth = imgRef.current.naturalWidth;
    const naturalHeight = imgRef.current.naturalHeight;

    const imagePositionAndSize = getImagePositionAndSize(
      naturalWidth,
      naturalHeight
    );
    toolbarRef.current?.setPercentValue(1);

    api.start({
      ...imagePositionAndSize,
      scale: 1,
    });
  };

  function getImagePositionAndSize(
    imageNaturalWidth: number,
    imageNaturalHeight: number
  ) {
    if (!containerRef.current) return;

    const {
      width: containerWidth,
      height: containerHeight,
    } = containerRef.current.getBoundingClientRect();

    let width = Math.min(containerWidth, imageNaturalWidth);
    let height = (width / imageNaturalWidth) * imageNaturalHeight;

    if (height > containerHeight) {
      height = containerHeight;
      width = (height / imageNaturalHeight) * imageNaturalWidth;
    }
    const x = (containerWidth - width) / 2;
    const y = (containerHeight - height) / 2;

    return { width, height, x, y };
  }

  function imageLoaded(event: SyntheticEvent<HTMLImageElement, Event>) {
    const naturalWidth = (event.target as HTMLImageElement).naturalWidth;
    const naturalHeight = (event.target as HTMLImageElement).naturalHeight;

    const positionAndSize = getImagePositionAndSize(
      naturalWidth,
      naturalHeight
    );

    if (!positionAndSize) return;

    api.set({
      ...positionAndSize,
      scale: 1,
      rotate: 0,
    });

    setIsLoading(false);
  }

  const CompareTo = (a: number, b: number) => {
    return Math.trunc(a) > Math.trunc(b);
  };

  const getSizeByAngel = (
    width: number,
    height: number,
    angle: number
  ): [number, number] => {
    const { abs, cos, sin, PI } = Math;

    const angleByRadians = (PI / 180) * angle;

    const c = cos(angleByRadians);
    const s = sin(angleByRadians);
    const halfw = 0.5 * width;
    const halfh = 0.5 * height;
    const widthAngle = 2 * (abs(c * halfw) + abs(s * halfh));
    const heightAngle = 2 * (abs(s * halfw) + abs(c * halfh));

    return [widthAngle, heightAngle];
  };

  const getBounds = (
    diffScale: number = 1,
    angle: number = 0
  ): BoundsType | null => {
    if (!imgRef.current || !containerRef.current) return null;

    let imageBounds = imgRef.current.getBoundingClientRect();
    const containerBounds = containerRef.current.getBoundingClientRect();

    const [width, height] = getSizeByAngel(
      imageBounds.width,
      imageBounds.height,
      angle
    );

    if (diffScale !== 1)
      imageBounds = {
        ...imageBounds,
        width: width * diffScale,
        height: height * diffScale,
      };
    else {
      imageBounds = {
        ...imageBounds,
        width,
        height,
      };
    }
    const originalWidth = imgRef.current.clientWidth;
    const widthOverhang = (imageBounds.width - originalWidth) / 2;

    const originalHeight = imgRef.current.clientHeight;
    const heightOverhang = (imageBounds.height - originalHeight) / 2;

    const isWidthOutContainer = imageBounds.width >= containerBounds.width;

    const isHeightOutContainer = imageBounds.height >= containerBounds.height;

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
  };

  const maybeAdjustBounds = (
    x: number,
    y: number,
    diffScale: number,
    angle: number = 0
  ) => {
    const bounds = getBounds(diffScale, angle);

    if (!bounds) return { x, y };

    const { left, right, top, bottom } = bounds;

    if (x > right) {
      x = right;
    } else if (x < left) {
      x = left;
    }

    if (y > bottom) {
      y = bottom;
    } else if (y < top) {
      y = top;
    }

    return { x, y };
  };

  const maybeAdjustImage = (point: { x: number; y: number }) => {
    if (!imgRef.current || !containerRef.current) return;

    // debugger;

    const imageBounds = imgRef.current.getBoundingClientRect();
    const containerBounds = containerRef.current.getBoundingClientRect();

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
      point.x = containerBounds.width - imageBounds.width + widthOverhang;
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
      point.y = containerBounds.height - imageBounds.height + heightOverhang;
    } else if (!isHeightOutContainer) {
      point.y =
        (containerBounds.height - imageBounds.height) / 2 + heightOverhang;
    }

    return point;
  };

  const rotateImage = (dir: number) => {
    if (style.rotate.isAnimating) return;

    const rotate = style.rotate.get() + dir * 90;

    const point = maybeAdjustImage(
      maybeAdjustBounds(style.x.get(), style.y.get(), 1, rotate)
    );

    api.start({
      ...point,
      rotate,
      config: {
        // easing: easings.easeInBack,
        duration: 200,
      },
      onResolve(result) {
        api.start({
          ...maybeAdjustImage({
            x: result.value.x,
            y: result.value.y,
          }),
          config: {
            duration: 100,
          },
        });
      },
    });
  };

  const zoomOut = () => {
    if (
      style.scale.isAnimating ||
      style.scale.get() <= MinScale ||
      !imgRef.current ||
      !containerRef.current
    )
      return;

    const { width, height, x, y } = imgRef.current.getBoundingClientRect();
    const {
      width: containerWidth,
      height: containerHeight,
    } = containerRef.current.getBoundingClientRect();

    const scale = Math.max(style.scale.get() - DefaultSpeedScale, MinScale);

    const tx = ((containerWidth - width) / 2 - x) / style.scale.get();
    const ty = ((containerHeight - height) / 2 - y) / style.scale.get();

    let dx = style.x.get() + DefaultSpeedScale * tx;
    let dy = style.y.get() + DefaultSpeedScale * ty;

    const ratio = scale / style.scale.get();

    const point = maybeAdjustImage(maybeAdjustBounds(dx, dy, ratio));
    toolbarRef.current?.setPercentValue(scale);

    api.start({
      scale,
      ...point,
      config: {
        duration: 300,
      },
      onResolve: (result) => {
        api.start({
          ...maybeAdjustImage({
            x: result.value.x,
            y: result.value.y,
          }),
          config: {
            ...config.default,
            duration: 100,
          },
        });
      },
    });
  };

  const zoomIn = () => {
    if (
      style.scale.isAnimating ||
      style.scale.get() >= MaxScale ||
      !imgRef.current ||
      !containerRef.current
    )
      return;

    const { width, height, x, y } = imgRef.current.getBoundingClientRect();
    const {
      width: containerWidth,
      height: containerHeight,
    } = containerRef.current.getBoundingClientRect();

    const tx = ((containerWidth - width) / 2 - x) / style.scale.get();
    const ty = ((containerHeight - height) / 2 - y) / style.scale.get();

    const dx = style.x.get() - DefaultSpeedScale * tx;
    const dy = style.y.get() - DefaultSpeedScale * ty;

    const scale = Math.min(style.scale.get() + DefaultSpeedScale, MaxScale);
    toolbarRef.current?.setPercentValue(scale);
    api.start({
      x: dx,
      y: dy,
      scale,
      config: {
        duration: 300,
      },
    });
  };

  const onKeyDown = (event: KeyboardEvent) => {
    const { code, ctrlKey } = event;

    switch (code) {
      case KeyboardEventKeys.ArrowLeft:
      case KeyboardEventKeys.ArrowRight:
        if (document.fullscreenElement) return;
        if (ctrlKey) {
          const dir = code === KeyboardEventKeys.ArrowRight ? 1 : -1;
          rotateImage(dir);
        }
        break;
      case KeyboardEventKeys.ArrowUp:
        zoomIn();
        break;
      case KeyboardEventKeys.ArrowDown:
        zoomOut();
        break;
      case KeyboardEventKeys.Digit1:
      case KeyboardEventKeys.Numpad1:
        if (ctrlKey) {
          RestartScaleAndSize();
        }
        break;
      default:
        break;
    }
  };

  const handleDoubleTapOrClick = (
    event:
      | TouchEvent
      | MouseEvent
      | React.MouseEvent<HTMLImageElement, MouseEvent>
  ) => {
    if (style.scale.get() !== 1) {
      RestartScaleAndSize();
    } else {
      zoomOnDoubleTap(event);
    }
  };

  const zoomOnDoubleTap = (
    event:
      | TouchEvent
      | MouseEvent
      | React.MouseEvent<HTMLImageElement, MouseEvent>,
    scale = 1
  ) => {
    if (
      !imgRef.current ||
      style.scale.get() >= MaxScale ||
      style.scale.isAnimating
    )
      return;

    const isTouch = "touches" in event;

    const pageX = isTouch ? event.touches[0].pageX : event.pageX;
    const pageY = isTouch ? event.touches[0].pageY : event.pageY;

    const { width, height, x, y } = imgRef.current.getBoundingClientRect();
    const tx = (pageX - (x + width / 2)) / style.scale.get();
    const ty = (pageY - (y + height / 2)) / style.scale.get();

    const dx = style.x.get() - scale * tx;
    const dy = style.y.get() - scale * ty;

    const ratio = scale / style.scale.get();

    const newScale = Math.min(style.scale.get() + scale, MaxScale);

    const point = maybeAdjustImage(maybeAdjustBounds(dx, dy, ratio));

    toolbarRef.current?.setPercentValue(newScale);

    api.start({
      ...point,
      scale: newScale,
      config: config.default,
      onChange() {
        api.start(maybeAdjustImage(maybeAdjustBounds(dx, dy, 1)));
      },
      onResolve() {
        api.start(maybeAdjustImage(maybeAdjustBounds(dx, dy, 1)));
      },
    });
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
        if (!imgRef.current) return;

        if (isDoubleTapRef.current || unmountRef.current) {
          isDoubleTapRef.current = false;
          return;
        }

        if (pinching || canceled) cancel();
        if (!imgRef.current || !containerRef.current) return;

        api.start({
          x:
            style.scale.get() === 1 &&
            !isDesktop &&
            ((isFistImage && mdx > 0) || (isLastImage && mdx < 0))
              ? style.x.get()
              : dx,
          y: dy,
          opacity:
            style.scale.get() === 1 && !isDesktop && mdy > 0
              ? imgRef.current.height / 10 / mdy
              : style.opacity.get(),
          immediate: true,
          config: config.default,
        });
      },

      onDragEnd: ({ cancel, canceled, movement: [mdx, mdy] }) => {
        if (unmountRef.current || !imgRef.current) {
          return;
        }

        if (canceled || isDoubleTapRef.current) {
          isDoubleTapRef.current = false;
          cancel();
        }

        if (style.scale.get() === 1 && !isDesktop) {
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
          config: config.default,
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
        lastOffset: [LScale],
        movement: [mScale],
        memo,
        first,
        canceled,
        event,
        pinching,
        cancel,
      }) => {
        if (
          canceled ||
          event.target === containerRef.current ||
          !imgRef.current
        )
          return memo;

        if (!pinching) cancel();

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

        const ratio = dScale / LScale;

        const point = maybeAdjustBounds(x, y, ratio, dRotate);

        scaleRef.current = dScale;

        api.start({
          ...point,
          scale: dScale,
          rotate: dRotate,
          delay: 0,
          onChange(result) {
            api.start({
              ...maybeAdjustImage({
                x: result.value.x,
                y: result.value.y,
              }),
              delay: 0,
              config: {
                duration: 200,
              },
            });
          },
          config: config.default,
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
          ...newPoint,
          rotate,
          delay: 0,
          onResolve: () => {
            api.start({
              ...maybeAdjustImage({
                x: style.x.get(),
                y: style.y.get(),
              }),
              delay: 0,
              config: {
                ...config.default,
                duration: 200,
              },
            });
          },
          onChange(result) {
            api.start({
              ...maybeAdjustImage({
                x: result.value.x,
                y: result.value.y,
              }),
              delay: 0,
              config: {
                ...config.default,
                duration: 200,
              },
            });
          },
          config: config.default,
        });
      },

      onClick: ({ pinching, event }) => {
        if (!imgRef.current || !containerRef.current || isDesktop || pinching)
          return;

        const time = new Date().getTime();

        if (time - lastTapTimeRef.current < 300) {
          //on Double Tap
          lastTapTimeRef.current = 0;
          isDoubleTapRef.current = true;

          handleDoubleTapOrClick(event);

          clearTimeout(setTimeoutIDTapRef.current);
        } else {
          lastTapTimeRef.current = time;
          setTimeoutIDTapRef.current = setTimeout(() => {
            // onTap
            setBackgroundBlack((state) => !state);
          }, 300);
        }
      },
      onWheel: ({
        first,
        offset: [, yWheel],
        lastOffset: [, lYWheel],
        movement: [, mYWheel],
        pinching,
        memo,
        event,
      }) => {
        if (
          !imgRef.current ||
          pinching ||
          style.scale.isAnimating ||
          event.target !== imgRef.current
        )
          return memo;

        const dScale = (-1 * yWheel) / RatioWheel;
        const lScale = (-1 * lYWheel) / RatioWheel;
        const mScale = (-1 * mYWheel) / RatioWheel;

        if (first || !memo) {
          const {
            width,
            height,
            x,
            y,
          } = imgRef.current.getBoundingClientRect();
          const tx = (event.pageX - (x + width / 2)) / style.scale.get();
          const ty = (event.pageY - (y + height / 2)) / style.scale.get();
          memo = [style.x.get(), style.y.get(), tx, ty];
        }
        const dx = memo[0] - mScale * memo[2];
        const dy = memo[1] - mScale * memo[3];

        const ratio = dScale / lScale;

        const point = maybeAdjustImage(maybeAdjustBounds(dx, dy, ratio));
        toolbarRef.current?.setPercentValue(dScale);
        api.start({
          ...point,
          scale: dScale,
          config: {
            ...config.default,
            duration: 300,
          },
          onResolve() {
            api.start(maybeAdjustImage(maybeAdjustBounds(dx, dy, 1)));
          },
        });

        return memo;
      },
    },
    {
      drag: {
        from: () => [style.x.get(), style.y.get()],
        axis: scale === 1 && !isDesktop ? "lock" : undefined,
        rubberband: isDesktop,
        bounds: () => {
          if (style.scale.get() === 1 && !isDesktop) return {};

          return getBounds() ?? {};
        },
      },
      pinch: {
        scaleBounds: { min: MinScale, max: MaxScale },
        rubberband: false,
        from: () => [style.scale.get(), style.rotate.get()],
        threshold: [0.1, 5],
        pinchOnWheel: false,
      },

      wheel: {
        from: () => [0, -style.scale.get() * RatioWheel],
        bounds: () => ({
          top: -MaxScale * RatioWheel,
          bottom: -MinScale * RatioWheel,
        }),
        axis: "y",
      },

      target: containerRef,
    }
  );

  const handleAction = (action: ToolbarActionType) => {
    resetToolbarVisibleTimer();

    switch (action) {
      case ToolbarActionType.ZoomOut:
        zoomOut();
        break;
      case ToolbarActionType.ZoomIn:
        zoomIn();
        break;

      case ToolbarActionType.RotateLeft:
      case ToolbarActionType.RotateRight:
        const dir = action === ToolbarActionType.RotateRight ? 1 : -1;
        rotateImage(dir);
        break;
      case ToolbarActionType.Reset:
        RestartScaleAndSize();
        break;
      default:
        break;
    }
  };

  function ToolbarEvent(item: ToolbarItemType) {
    if (item.onClick) {
      item.onClick();
    } else {
      handleAction(item.actionType);
    }
  }

  return (
    <>
      {isMobile && !backgroundBlack && mobileDetails}
      <ImageViewerContainer
        ref={containerRef}
        $backgroundBlack={backgroundBlack}
      >
        <ViewerLoader isLoading={isLoading} />
        <ImageWrapper $isLoading={isLoading}>
          <Image
            src={src}
            ref={imgRef}
            style={style}
            onDoubleClick={handleDoubleTapOrClick}
            onLoad={imageLoaded}
          />
        </ImageWrapper>
      </ImageViewerContainer>
      {isDesktop && panelVisible && (
        <ImageViewerToolbar
          ref={toolbarRef}
          toolbar={toolbar}
          generateContextMenu={generateContextMenu}
          setIsOpenContextMenu={setIsOpenContextMenu}
          ToolbarEvent={ToolbarEvent}
        />
      )}
    </>
  );
}

export default ImageViewer;
