import styled, { css } from "styled-components";
import Base from "../themes/base";
import { isMobileOnly } from "react-device-detect";

export const StyledSubmenu = styled.div`
  display: flex;
  flex-direction: column;

  .scrollbar {
    width: 100%;
    height: auto;
  }

  .text {
    width: auto;
    display: inline-block;
    position: absolute;
  }

  .sticky {
    position: sticky;
    top: 0;
    background: ${(props) => props.theme.submenu.backgroundColor};
    z-index: 1;
  }

  ${isMobileOnly &&
  css`
    .sticky {
      top: 52px;
    }
  `}

  .sticky-indent {
    height: 15px;
  }
`;

StyledSubmenu.defaultProps = { theme: Base };

export const StyledSubmenuBottomLine = styled.div`
  height: 1px;
  width: 100%;
  margin-top: -1px;
  background: ${(props) => props.theme.submenu.lineColor};
`;

StyledSubmenuBottomLine.defaultProps = { theme: Base };

export const StyledSubmenuContentWrapper = styled.div`
  width: 100%;
  display: flex;
  align-items: center;
`;

export const StyledSubmenuItems = styled.div`
  overflow: scroll;

  display: flex;
  flex-direction: row;
  gap: 4px;

  width: max-content;
  overflow: hidden;
  &::-webkit-scrollbar {
    display: none;
  }
`;

export const StyledSubmenuItem = styled.div.attrs((props) => ({
  id: props.id,
}))`
  scroll-behavior: smooth;
  cursor: pointer;
  display: flex;
  gap: 4px;
  flex-direction: column;
  padding-top: 4px;
  line-height: 20px;
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-left: 17px;
        `
      : css`
          &:not(:last-child) {
            margin-right: 17px;
          }
        `}
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

export const StyledSubmenuItemText = styled.div`
  width: max-content;
  display: flex;

  .item-text {
    color: ${(props) =>
      props.isActive
        ? props.theme.submenu.activeTextColor
        : props.theme.submenu.textColor};
    font-weight: 600;
  }
`;

StyledSubmenuItemText.defaultProps = { theme: Base };

export const StyledSubmenuItemLabel = styled.div`
  z-index: 1;
  width: 100%;
  height: 4px;
  bottom: 0px;
  border-radius: 4px 4px 0 0;
  background-color: ${(props) =>
    props.isActive ? props.theme.submenu.bottomLineColor : ""};
`;

StyledSubmenuItemLabel.defaultProps = { theme: Base };

export const SubmenuScroller = styled.div`
  position: relative;
  display: inline-block;
  flex: 1 1 auto;
  white-space: nowrap;
  scrollbar-width: none; // Firefox
  &::-webkit-scrollbar {
    display: none; // Safari + Chrome
  }
  overflow-x: auto;
  overflow-y: hidden;

  ${(props) =>
    !props.scale &&
    css`
      display: grid;
      flex: 0 1 auto;
    `}
`;

export const SubmenuRoot = styled.div`
  overflow: hidden;
  min-height: 32px;
  // Add iOS momentum scrolling for iOS < 13.0
  -webkit-overflow-scrolling: touch;
  display: flex;
`;

export const SubmenuScrollbarSize = styled.div`
  height: 32;
  position: absolute;
  top: -9999;
  overflow-x: auto;
  overflow-y: hidden;
  // Hide dimensionless scrollbar on macOS
  scrollbar-width: none; // Firefox
  &::-webkit-scrollbar {
    display: none; // Safari + Chrome
  }
`;
