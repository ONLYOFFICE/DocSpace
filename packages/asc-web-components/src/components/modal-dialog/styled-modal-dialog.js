import styled from "styled-components";
import { Base } from "../../themes";
import { Icons } from "../icons";
import Box from "../box";

const Dialog = styled.div`
  position: relative;
  display: flex;
  cursor: default;
  align-items: center;

  width: ${(props) => props.theme.modalDialog.width};
  max-width: ${(props) => props.theme.modalDialog.maxwidth};
  margin: ${(props) => props.theme.modalDialog.margin};
  min-height: ${(props) => props.theme.modalDialog.minHeight};
`;
Dialog.defaultProps = { theme: Base };

const Content = styled.div`
  position: relative;
  box-sizing: border-box;

  height: ${(props) => props.contentHeight};
  width: ${(props) => props.contentWidth};
  background-color: ${(props) =>
    props.theme.modalDialog.content.backgroundColor};
  padding: ${(props) => props.theme.modalDialog.content.padding};

  .heading {
    max-width: ${(props) => props.theme.modalDialog.content.heading.maxWidth};
    margin: ${(props) => props.theme.modalDialog.content.heading.margin};
    line-height: ${(props) =>
      props.theme.modalDialog.content.heading.lineHeight};
    font-weight: ${(props) =>
      props.theme.modalDialog.content.heading.fontWeight};
  }
`;
Content.defaultProps = { theme: Base };

const StyledHeader = styled.div`
  display: flex;
  align-items: center;
  border-bottom: ${(props) => props.theme.modalDialog.header.borderBottom};
`;
StyledHeader.defaultProps = { theme: Base };

const CloseButton = styled(Icons.CrossSidebarIcon)`
  cursor: pointer;
  position: absolute;

  width: ${(props) => props.theme.modalDialog.closeButton.width};
  height: ${(props) => props.theme.modalDialog.closeButton.height};
  min-width: ${(props) => props.theme.modalDialog.closeButton.minWidth};
  min-height: ${(props) => props.theme.modalDialog.closeButton.minHeight};

  right: ${(props) => props.theme.modalDialog.closeButton.right};
  top: ${(props) => props.theme.modalDialog.closeButton.top};

  &:hover {
    path {
      fill: ${(props) => props.theme.modalDialog.closeButton.hoverColor};
    }
  }
`;

const BodyBox = styled(Box)`
  position: relative;
`;

export { CloseButton, StyledHeader, Content, Dialog, BodyBox };
