import styled from "styled-components";
import { isMobile } from "react-device-detect";

export const PageCountWrapper = styled.div<{ isPanelOpen: boolean }>`
  display: flex;
  gap: 10px;
  align-items: center;

  position: fixed;
  bottom: ${isMobile ? "12px" : "108px"};
  left: ${({ isPanelOpen }) => `calc(50% + ${isPanelOpen ? 306 / 2 : 0}px)`};
  z-index: 307;

  transform: translateX(-50%);

  padding: ${isMobile ? "12px 16px" : "16px 32px"};
  border-radius: 22px;
  background: rgba(0, 0, 0, 0.4);

  color: white;
  font-size: 12px;
  line-height: 16px;

  box-sizing: border-box;

  svg {
    path {
      fill: white;
    }
  }

  user-select: none;

  transition: background 0.26s ease-out 0s;

  @media (hover: hover) {
    &:hover {
      background: rgba(0, 0, 0, 0.8);
    }
  }
`;
