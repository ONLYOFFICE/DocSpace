import styled, { css } from "styled-components";
import Base from "../themes/base";
import Box from "../box";
import { mobile, smallTablet, tablet } from "../utils/device";
import { isMobile } from "react-device-detect";

const StyledModal = styled.div`
  pointer-events: none;
  &.modal-active {
    pointer-events: all;
  }
  .loader-wrapper {
    padding: 0 16px 16px;
  }
`;

const Dialog = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  width: auto;
  margin: 0 auto;
  min-height: 100%;
`;

const Content = styled.div.attrs((props) => ({
  style: {
    marginBottom:
      props.modalSwipeOffset < 0 ? `${props.modalSwipeOffset * 1.1}px` : "0px",
  },
}))`
  box-sizing: border-box;
  position: relative;
  background-color: ${(props) => props.theme.modalDialog.backgroundColor};
  color: ${(props) => props.theme.modalDialog.textColor};
  padding: ${(props) =>
    props.currentDisplayType === "modal" ? "0" : "0 0 -16px"};

  ${(props) =>
    props.currentDisplayType === "modal"
      ? css`
          height: auto;
          max-height: ${(props) =>
            props.autoMaxHeight ? "auto" : props.isLarge ? "400px" : "280px"};
          width: ${(props) =>
            props.autoMaxWidth ? "auto" : props.isLarge ? "520px" : "400px"};

          border-radius: 6px;
          @media ${smallTablet} {
            transform: translateY(${(props) => (props.visible ? "0" : "100%")});
            transition: transform 0.3s ease-in-out;
            position: absolute;
            bottom: 0;
            width: 100%;
            height: auto;
            border-radius: 6px 6px 0 0;
          }
        `
      : css`
          width: 480px;
          display: flex;
          flex-direction: column;
          position: absolute;
          top: 0;
          right: 0;
          bottom: 0;
          transform: translateX(${(props) => (props.visible ? "0" : "100%")});
          transition: transform 0.3s ease-in-out;
          @media ${smallTablet} {
            transform: translateY(${(props) => (props.visible ? "0" : "100%")});
            height: calc(100% - 64px);
            width: 100%;
            left: 0;
            top: auto;
            bottom: 0;
          }
        `}
`;

const StyledHeader = styled.div`
  display: flex;
  align-items: center;
  border-bottom: ${(props) =>
    `1px solid ${props.theme.modalDialog.headerBorderColor}`};
  margin-bottom: 16px;
  height: 52px;

  display: flex;
  align-items: center;
  padding: 0 16px 0;

  .heading {
    font-family: "Open Sans";
    color: ${(props) => props.theme.modalDialog.textColor};
    font-weight: 700;
    font-size: ${(props) =>
      props.currentDisplayType === "modal" ? "18px" : "21px"};
  }
`;

const StyledBody = styled(Box)`
  position: relative;
  padding: 0 16px;
  padding-bottom: ${(props) =>
    props.currentDisplayType === "aside" || props.hasFooter ? "8px" : "16px"};

  ${(props) =>
    props.currentDisplayType === "aside" &&
    css`
      .body_content-with_scroll {
        ${isMobile && "margin-right: 8px;"}
      }
      margin-right: ${props.withBodyScroll ? "-16px" : "0"};
      padding-bottom: 8px;
      height: 100%;
      min-height: auto;
    `}
`;

const StyledFooter = styled.div`
  display: flex;
  flex-direction: row;
  ${(props) =>
    props.withFooterBorder &&
    `border-top: 1px solid ${props.theme.modalDialog.headerBorderColor}`};
  padding: 16px;

  gap: 8px;
  @media ${tablet} {
    gap: 10px;
  }
`;

Dialog.defaultProps = { theme: Base };
StyledHeader.defaultProps = { theme: Base };
Content.defaultProps = { theme: Base };

export { StyledModal, StyledHeader, Content, Dialog, StyledBody, StyledFooter };
