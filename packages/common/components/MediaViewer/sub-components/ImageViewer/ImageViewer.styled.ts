import styled from "styled-components";
import { animated } from "@react-spring/web";

export const ImageViewerContainer = styled.div<{ $backgroundBlack: boolean }>`
  width: 100%;
  height: 100%;

  position: fixed;
  inset: 0;
  overflow: hidden;

  z-index: 300;

  user-select: none;
  touch-action: none;

  background-color: ${(props) =>
    props.$backgroundBlack ? "#000" : "rgba(55, 55, 55, 0.6)"};
`;

export const ImageWrapper = styled.div<{ $isLoading: boolean }>`
  overflow: hidden;
  width: 100%;
  height: 100%;
  visibility: ${(props) => (props.$isLoading ? "hidden" : "visible")};
`;

export const Image = styled(animated.img)`
  will-change: opacity, transform, scale;
`;
