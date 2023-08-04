import { isMobile, isMobileOnly } from "react-device-detect";
import styled, { css } from "styled-components";
import { animated } from "@react-spring/web";

export const ContainerPlayer = styled.div<{ $isFullScreen: boolean }>`
  position: fixed;
  inset: 0;
  z-index: 305;
  background-color: ${(props) =>
    props.$isFullScreen ? "#000" : "rgba(55, 55, 55, 0.6)"};
  touch-action: none;
`;

export const VideoWrapper = styled(animated.div)<{ $visible: boolean }>`
  inset: 0;
  visibility: ${(props) => (props.$visible ? "visible" : "hidden")};
  opacity: ${(props) => (props.$visible ? 1 : 0)};
  height: 100%;
  width: 100%;
  touch-action: none;

  .audio-container {
    width: 190px;
    height: 190px;
    display: flex;
    justify-content: center;
    align-items: center;
    background: rgba(0, 0, 0, 0.7);
    border-radius: 20px;
  }
`;

const StyledMobilePlayerControls = css`
  background-color: rgba(0, 0, 0, 0.8);
  height: 80px;
`;

export const StyledPlayerControls = styled.div<{ $isShow: boolean }>`
  position: fixed;
  right: 0px;
  bottom: 0px;
  left: 0px;
  z-index: 307;
  display: flex;

  width: 100%;
  height: 188px;

  visibility: ${(props) => (props.$isShow ? "visible" : "hidden")};
  opacity: ${(props) => (props.$isShow ? "1" : "0")};

  background: linear-gradient(
    rgba(0, 0, 0, 0) 0%,
    rgba(0, 0, 0, 0.64) 48.44%,
    rgba(0, 0, 0, 0.89) 100%
  );

  ${isMobile && StyledMobilePlayerControls}
`;

export const ControlContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;

  margin-top: 30px;

  & > div {
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  ${isMobile &&
  css`
    margin-top: 8px;
    .player_right-control {
      margin-right: -8px;
    }
  `}
`;

export const PlayerControlsWrapper = styled.div`
  padding: 0 30px;
  width: 100%;
  margin-top: 80px;

  ${isMobile &&
  css`
    margin-top: 0px;
  `}

  ${isMobileOnly &&
  css`
    padding: 0 15px;
  `}
`;
