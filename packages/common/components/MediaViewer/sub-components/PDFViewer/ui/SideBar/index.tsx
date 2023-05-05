import React, { Dispatch, SetStateAction, useState } from "react";
import styled from "styled-components";

import ArticleShowMenuReactSvgUrl from "PUBLIC_DIR/images/article-show-menu.react.svg";
import ViewTilesIcon from "PUBLIC_DIR/images/view-tiles.react.svg";
import ViewRowsIcon from "PUBLIC_DIR/images/view-rows.react.svg";

type PanelProps = {
  isPanelOpen: boolean;
  setIsPDFSidebarOpen: Dispatch<SetStateAction<boolean>>;
};

const SidebarContainer = styled.aside<{ isPanelOpen: boolean }>`
  display: flex;
  flex-direction: column;

  height: 100vh;
  width: 100%;

  background: #333333;

  max-width: ${(props) => (props.isPanelOpen ? "306px" : "0px")};
  visibility: ${(props) => (props.isPanelOpen ? "visible" : "hidden")};
`;

const SidebarHeader = styled.div`
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

const Thumbnails = styled.section`
  height: 100%;
  width: 100%;

  position: relative;
`;

const HideSidebarIcon = styled(ArticleShowMenuReactSvgUrl)`
  transform: rotate(180deg);
`;

// import ArticleShowMenuReactSvgUrl from "PUBLIC_DIR/images/article-show-menu.react.svg?url";

function Sidebar({ isPanelOpen, setIsPDFSidebarOpen }: PanelProps) {
  const [toggle, setToggle] = useState<boolean>(false);

  const handleToggle = () => setToggle((prev) => !prev);

  const closeSidebar = () => setIsPDFSidebarOpen(false);

  return (
    <SidebarContainer isPanelOpen={isPanelOpen}>
      <SidebarHeader>
        {React.createElement(toggle ? ViewTilesIcon : ViewRowsIcon, {
          onClick: handleToggle,
        })}
        <HideSidebarIcon onClick={closeSidebar} />
      </SidebarHeader>
      <Thumbnails id="viewer-thumbnail" />
    </SidebarContainer>
  );
}

export default Sidebar;
