import styled from "styled-components";
import ArticleShowMenuReactSvgUrl from "PUBLIC_DIR/images/article-show-menu.react.svg";

export const SidebarContainer = styled.aside<{ isPanelOpen: boolean }>`
  display: flex;
  flex-direction: column;

  height: 100vh;
  width: 100%;

  background: #333333;

  max-width: ${(props) => (props.isPanelOpen ? "306px" : "0px")};
  visibility: ${(props) => (props.isPanelOpen ? "visible" : "hidden")};
  overflow: ${(props) => (props.isPanelOpen ? "visible" : "hidden")};
`;

export const SidebarHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30px;

  svg {
    cursor: pointer;
    path {
      fill: rgba(255, 255, 255, 0.6);
    }
  }
`;

export const Thumbnails = styled.section<{ visible: boolean }>`
  height: 100%;
  width: 100%;

  position: relative;

  display: ${(props) => (props.visible ? "block" : "none")};

  visibility: ${(props) => (props.visible ? "visible" : "hidden")};
  opacity: ${(props) => (props.visible ? 1 : 0)};
`;

export const HideSidebarIcon = styled(ArticleShowMenuReactSvgUrl)`
  transform: rotate(180deg);
  margin-left: auto;
`;
