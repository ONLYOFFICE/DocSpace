import styled, { css } from "styled-components";
import Base from "../themes/base";
import { tablet } from "../utils/device";
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
  padding: 4px 14px 0;
  line-height: 20px;
`;

export const StyledSubmenuItemText = styled.div`
  width: 100%;
  display: flex;

  .item-text {
    color: ${(props) =>
      props.isActive ? props.theme.submenu.textColor : "#657077"};
  }
`;

StyledSubmenuItemText.defaultProps = { theme: Base };

export const StyledSubmenuItemLabel = styled.div`
  z-index: 1;
  width: calc(100% + 28px);
  margin-left: -14px;
  height: 4px;
  bottom: 0px;
  border-radius: 4px 4px 0 0;
  background-color: ${(props) =>
    props.isActive ? props.theme.submenu.bottomLineColor : "none"};
`;

StyledSubmenuItemLabel.defaultProps = { theme: Base };
