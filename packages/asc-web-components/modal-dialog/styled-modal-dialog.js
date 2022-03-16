import styled, { css } from "styled-components";
import Base from "../themes/base";
import Box from "../box";
import CrossSidebarIcon from "../../../public/images/cross.sidebar.react.svg";
import { mobile, isTablet, isDesktop } from "../utils/device";

const StyledModal = styled.div`
  .backdrop {
    background: rgba(6, 22, 38, 0.2);
    backdrop-filter: blur(18px);
  }

  .heading-aside {
    margin-right: -16px;
  }

  .bodybox-aside {
    padding-left: 16px;
  }

  .footer-aside {
    width: 100%;
  }
`;

const Dialog = styled.div`
  position: relative;
  display: flex;
  cursor: default;
  align-items: center;
  justify-content: center;
  width: ${(props) => props.theme.modalDialog.width};
  max-width: ${(props) => props.theme.modalDialog.maxwidth};
  margin: ${(props) => props.theme.modalDialog.margin};
  min-height: ${(props) => props.theme.modalDialog.minHeight};
`;

const Content = styled.div`
  position: relative;
  box-sizing: border-box;
  height: ${(props) => props.contentHeight};
  width: ${(props) => (props.contentWidth ? props.contentWidth : "auto")};
  background-color: ${(props) =>
    props.theme.modalDialog.content.backgroundColor};
  padding: ${(props) =>
    props.displayType === "modal"
      ? props.theme.modalDialog.content.modalPadding
      : props.theme.modalDialog.content.asidePadding};
  border-radius: ${(props) =>
    props.theme.modalDialog.content.modalBorderRadius};
  ${(props) =>
    props.removeScroll &&
    css`
      overflow: hidden;
    `}

  @media ${mobile} {
    position: absolute;
    bottom: 0;
    width: 100%;
  }
`;

const StyledHeader = styled.div`
  display: flex;
  align-items: center;
  border-bottom: ${(props) => props.theme.modalDialog.header.borderBottom};
  margin-bottom: 16px;

  .heading {
    height: 37px;
    display: flex;
    align-items: center;
    max-width: ${(props) => props.theme.modalDialog.content.heading.maxWidth};
    margin: ${(props) => props.theme.modalDialog.content.heading.margin};
    font-family: "Open Sans";

    font-weight: ${(props) =>
      props.theme.modalDialog.content.heading.fontWeight};
    font-size: ${(props) =>
      props.displayType === "modal"
        ? props.theme.modalDialog.content.heading.modalFontSize
        : props.theme.modalDialog.content.heading.asideFontSize};
  }
`;

const CloseButton = styled(CrossSidebarIcon)`
  cursor: pointer;
  position: absolute;

  width: ${(props) => props.theme.modalDialog.closeButton.width};
  height: ${(props) => props.theme.modalDialog.closeButton.height};
  min-width: ${(props) => props.theme.modalDialog.closeButton.minWidth};
  min-height: ${(props) => props.theme.modalDialog.closeButton.minHeight};

  path {
    stroke: #fff;
  }

  right: 0;
  top: 0;
  ${({ displayType }) =>
    displayType === "modal" || (displayType === "auto" && isDesktop())
      ? css`
          margin-right: -23px;
          @media ${mobile} {
            margin-right: 10px;
            margin-top: -23px;
          }
        `
      : css`
          margin-top: 17px;
          margin-right: 325px;
          padding-right: 10px;
        `}
`;

const StyledFooter = styled.div`
  display: flex;
  flex-direction: row;
  gap: 10px;
  padding: 16px;
`;

const BodyBox = styled(Box)`
  position: relative;
  ${(props) => props.removeScroll && `height: 100%;`}
  margin-bottom:8px;
`;

Dialog.defaultProps = { theme: Base };
StyledHeader.defaultProps = { theme: Base };
CloseButton.defaultProps = { theme: Base };
Content.defaultProps = { theme: Base };

export {
  StyledModal,
  CloseButton,
  StyledHeader,
  Content,
  Dialog,
  BodyBox,
  StyledFooter,
};
