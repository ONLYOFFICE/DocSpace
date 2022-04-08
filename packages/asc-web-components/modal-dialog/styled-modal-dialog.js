import styled, { css } from "styled-components";
import Base from "../themes/base";
import Box from "../box";
import { smallTablet } from "../utils/device";

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
  width: ${(props) => props.theme.modalDialog.width};
  max-width: ${(props) => props.theme.modalDialog.maxwidth};
  margin: ${(props) => props.theme.modalDialog.margin};
  min-height: ${(props) => props.theme.modalDialog.minHeight};
`;

const Content = styled.div.attrs((props) => ({
  style: {
    marginBottom:
      props.modalSwipeOffset < 0 ? `${props.modalSwipeOffset}px` : "0px",
  },
}))`
  position: relative;
  box-sizing: border-box;
  height: auto;
  max-height: ${(props) =>
    props.displayType === "aside" ? "auto" : props.isLarge ? "400px" : "280px"};
  width: ${(props) =>
    props.displayType === "aside" ? "auto" : props.isLarge ? "520px" : "400px"};

  background-color: ${(props) =>
    props.theme.modalDialog.content.backgroundColor};
  padding: ${(props) =>
    props.displayType === "modal"
      ? props.theme.modalDialog.content.modalPadding
      : props.theme.modalDialog.content.asidePadding};
  border-radius: ${(props) =>
    props.theme.modalDialog.content.modalBorderRadius};
  ${(props) =>
    props.withoutBodyScroll &&
    css`
      overflow: hidden;
    `}

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
  border-bottom: 1px solid #eceef1;
  margin-bottom: 16px;
  height: 52px;

  display: flex;
  align-items: center;
  padding: 0 16px 0;

  .heading {
    max-width: ${(props) => props.theme.modalDialog.content.heading.maxWidth};
    font-family: "Open Sans";

    font-weight: ${(props) =>
      props.theme.modalDialog.content.heading.fontWeight};
    font-size: ${(props) =>
      props.displayType === "modal"
        ? props.theme.modalDialog.content.heading.modalFontSize
        : props.theme.modalDialog.content.heading.asideFontSize};
  }
`;

const BodyBox = styled(Box)`
  position: relative;
  ${(props) => props.withoutBodyScroll && `height: 100%;`}
  padding-bottom: 8px;
`;

const StyledFooter = styled.div`
  display: flex;
  flex-direction: row;
  border-top: 1px solid #eceef1;
  gap: 10px;
  padding: 16px;
  ${(props) => props.displayType === "aside" && "margin-bottom: -10px"}
`;

Dialog.defaultProps = { theme: Base };
StyledHeader.defaultProps = { theme: Base };
Content.defaultProps = { theme: Base };

export { StyledModal, StyledHeader, Content, Dialog, BodyBox, StyledFooter };
