import styled, { css } from "styled-components";
import Base from "../themes/base";
import Box from "../box";
import { smallTablet } from "../utils/device";

const StyledModal = styled.div`
  pointer-events: none;

  &.modal-active {
    pointer-events: all;
  }

  .heading-aside {
    margin-right: -16px;
  }

  .bodybox-aside {
    padding-left: 16px;
  }

  .footer-aside {
    width: 100%;
    margin-bottom: -6px;
  }

  .aside-dialog {
    padding: 0;
    margin-bottom: -6px;
  }
`;

const Dialog = styled.div`
  position: relative;
  display: flex;
  cursor: default;
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
  position: relative;
  box-sizing: border-box;
  background-color: ${(props) => props.theme.modalDialog.backgroundColor};
  color: ${(props) => props.theme.modalDialog.textColor};
  height: auto;
  max-height: ${(props) =>
    props.displayType === "aside" ? "auto" : props.isLarge ? "400px" : "280px"};
  width: ${(props) =>
    props.displayType === "aside" ? "auto" : props.isLarge ? "520px" : "400px"};
  border-radius: 6px;
  padding: ${(props) => (props.displayType === "modal" ? "0" : "0 0 -16px")};

  ${({ displayType }) =>
    displayType === "modal" &&
    css`
      @media ${smallTablet} {
        position: absolute;
        bottom: 0;
        width: 100%;
        border-radius: 6px 6px 0 0;
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
    font-size: ${(props) => (props.displayType === "modal" ? "18px" : "21px")};
  }
`;

const BodyBox = styled(Box)`
  position: relative;
  ${(props) => props.withoutBodyScroll && `height: 100%;`}
  padding-bottom: 8px;
`;

const StyledFooter = styled.div`
  //width: 100%;
  display: flex;
  flex-direction: row;
  border-top: ${(props) =>
    `1px solid ${props.theme.modalDialog.footerBorderColor}`};
  gap: 10px;
  padding: 16px;
  ${(props) => props.displayType === "aside" && "margin-bottom: -10px"}
`;

Dialog.defaultProps = { theme: Base };
StyledHeader.defaultProps = { theme: Base };
Content.defaultProps = { theme: Base };

export { StyledModal, StyledHeader, Content, Dialog, BodyBox, StyledFooter };
