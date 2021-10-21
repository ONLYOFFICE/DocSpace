import styled from "styled-components";
import { tablet, desktop } from "../utils/device";
import Base from "../themes/base";

const StyledGroupButtonsMenu = styled.div`
  box-sizing: border-box;
  position: sticky;
  top: ${(props) => props.theme.groupButtonsMenu.top};
  background: ${(props) => props.theme.groupButtonsMenu.background};
  box-shadow: ${(props) => props.theme.groupButtonsMenu.boxShadow};
  height: ${(props) => props.theme.groupButtonsMenu.height};
  list-style: none;
  padding: ${(props) => props.theme.groupButtonsMenu.padding};
  width: ${(props) => props.theme.groupButtonsMenu.width};
  white-space: nowrap;

  display: ${(props) => (props.visible ? "block" : "none")};
  z-index: ${(props) => props.theme.groupButtonsMenu.zIndex};

  @media ${tablet} {
    height: ${(props) => props.theme.groupButtonsMenu.tabletHeight};
  }

  @media ${desktop} {
    margin-top: ${(props) => props.theme.groupButtonsMenu.marginTop};
  }

  -webkit-touch-callout: none;
  -webkit-user-select: none;
`;
StyledGroupButtonsMenu.defaultProps = { theme: Base };

const CloseButton = styled.div`
  position: absolute;
  right: ${(props) => props.theme.groupButtonsMenu.closeButton.right};
  top: ${(props) => props.theme.groupButtonsMenu.closeButton.top};
  width: ${(props) => props.theme.groupButtonsMenu.closeButton.width};
  height: ${(props) => props.theme.groupButtonsMenu.closeButton.height};
  padding: ${(props) => props.theme.groupButtonsMenu.closeButton.padding};

  @media ${tablet} {
    right: 3px;
    height: ${(props) => props.theme.groupButtonsMenu.closeButton.tabletHeight};
    top: ${(props) => props.theme.groupButtonsMenu.closeButton.tabletTop};
  }

  &:hover {
    cursor: pointer;

    &:before,
    &:after {
      background-color: ${(props) =>
        props.theme.groupButtonsMenu.closeButton.hoverBackgroundColor};
    }
  }

  &:before,
  &:after {
    position: absolute;
    left: 15px;
    content: " ";
    height: 20px;
    width: 1px;
    background-color: ${(props) =>
      props.theme.groupButtonsMenu.closeButton.backgroundColor};
  }

  &:before {
    transform: rotate(45deg);
  }

  &:after {
    transform: rotate(-45deg);
  }
`;
CloseButton.defaultProps = { theme: Base };

const GroupMenuWrapper = styled.div`
  display: inline-block;
`;

export { StyledGroupButtonsMenu, CloseButton, GroupMenuWrapper };
