import { animated } from "@react-spring/web";
import styled from "styled-components";

export const MobileDrawerContainer = styled.section`
  position: fixed;
  z-index: 308;
  width: 100%;
`;

export const MobileDrawerWrapper = styled(animated.div)`
  position: fixed;
  bottom: 0;

  display: flex;
  flex-direction: column;

  width: 100%;

  background: #333333;
  touch-action: none;

  overflow: hidden;

  .block_elem {
    position: absolute;
    padding: 0;
    margin: 0;
  }
`;

export const MobileDrawerHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;

  padding: 24px 30px;

  touch-action: none;

  svg path {
    fill: rgba(255, 255, 255, 0.6);
  }

  .mobile-drawer_cross-icon {
    margin-left: auto;
  }
`;
