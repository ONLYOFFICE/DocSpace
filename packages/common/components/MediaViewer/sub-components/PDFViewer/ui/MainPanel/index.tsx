import React, { ForwardedRef, forwardRef, useEffect, useRef } from "react";
import { useSpring, animated } from "@react-spring/web";
import { useGesture } from "@use-gesture/react";
import { isDesktop } from "react-device-detect";
import styled from "styled-components";
import MainPanelProps from "./MainPanel.props";

const Wrapper = styled(animated.section)`
  width: 100%;
  height: ${`calc(100vh - ${isDesktop ? "85" : "66"}px)`};
  margin-top: ${isDesktop ? "85px" : "66px"};
  touch-action: none;
`;

const Content = styled.div<{ isLoading: boolean }>`
  visibility: ${(props) => (props.isLoading ? "hidden" : "visible")};
`;

function MainPanel(
  {
    isLoading,
    isFistImage,
    isLastImage,
    src,
    onNext,
    onPrev,
    setZoom,
  }: MainPanelProps,
  ref: ForwardedRef<HTMLDivElement>
) {
  const wrapperRef = useRef<HTMLDivElement>(null);

  const [style, api] = useSpring(() => ({
    x: 0,
    scale: 1,
  }));

  useEffect(() => {
    resetState();
  }, [src]);

  const resetState = () => {
    api.set({ x: 0 });
  };

  useGesture(
    {
      onDrag: ({ offset: [dx], movement: [mdx] }) => {
        if (isDesktop) return;

        api.start({
          x:
            (isFistImage && mdx > 0) || (isLastImage && mdx < 0)
              ? style.x.get()
              : dx,
          immediate: true,
        });
      },
      onDragEnd: ({ movement: [mdx] }) => {
        if (isDesktop) return;

        const width = window.innerWidth;

        if (mdx < -width / 4) {
          return onNext();
        } else if (mdx > width / 4) {
          return onPrev();
        }

        api.start({ x: 0 });
      },
    },
    {
      drag: {
        from: () => [style.x.get(), 0],
        axis: "x",
      },
      pinch: {
        scaleBounds: { min: 0.5, max: 5 },
        from: () => [style.scale.get(), 0],
        threshold: [0.1, 5],
        rubberband: false,
        pinchOnWheel: false,
      },
      target: wrapperRef,
    }
  );

  return (
    <Wrapper ref={wrapperRef} style={style}>
      <Content id="mainPanel" ref={ref} isLoading={isLoading} />
    </Wrapper>
  );
}

export default forwardRef<HTMLDivElement, MainPanelProps>(MainPanel);
