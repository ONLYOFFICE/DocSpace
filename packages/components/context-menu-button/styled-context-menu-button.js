import styled from "styled-components";
import Base from "../themes/base";

const StyledOuter = styled.div`
  display: inline-block;
  position: relative;
  cursor: pointer;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;
StyledOuter.defaultProps = { theme: Base };

const StyledContent = styled.div`
  box-sizing: border-box;
  position: relative;
  width: ${(props) => props.theme.contextMenuButton.content.width};
  background-color: ${(props) =>
    props.theme.contextMenuButton.content.backgroundColor};
  padding: ${(props) => props.theme.contextMenuButton.content.padding};

  .header {
    max-width: ${(props) =>
      props.theme.contextMenuButton.headerContent.maxWidth};
    margin: ${(props) => props.theme.contextMenuButton.headerContent.margin};
    line-height: ${(props) =>
      props.theme.contextMenuButton.headerContent.lineHeight};
    font-weight: ${(props) =>
      props.theme.contextMenuButton.headerContent.fontWeight} !important;
  }
`;
StyledContent.defaultProps = { theme: Base };

const StyledHeaderContent = styled.div`
  display: flex;
  align-items: center;
  border-bottom: ${(props) =>
    props.theme.contextMenuButton.headerContent.borderBottom};
`;
StyledHeaderContent.defaultProps = { theme: Base };

const StyledBodyContent = styled.div`
  position: relative;
  padding: ${(props) => props.theme.contextMenuButton.bodyContent.padding};
  display: flex;
  flex-direction: column;

  .context-menu-button_link {
    margin-top: 17px;
  }

  .context-menu-button_link-header {
    text-transform: uppercase;
  }

  .context-menu-button_link-header:not(:first-child) {
    margin-top: 50px;
  }
`;
StyledBodyContent.defaultProps = { theme: Base };

export { StyledBodyContent, StyledHeaderContent, StyledContent, StyledOuter };
