import React, { useState } from "react";
import Bookmarks from "../Bookmarks";

import SidebarProps from "./Sidebar.props";
import {
  HideSidebarIcon,
  SidebarContainer,
  SidebarHeader,
  Thumbnails,
} from "./Sidebar.styled";

import ViewTilesIcon from "PUBLIC_DIR/images/view-tiles.react.svg";
import ViewRowsIcon from "PUBLIC_DIR/images/view-rows.react.svg";

function Sidebar({
  bookmarks,
  isPanelOpen,
  setIsPDFSidebarOpen,
  navigate,
}: SidebarProps) {
  const [toggle, setToggle] = useState<boolean>(false);

  const handleToggle = () => {
    setToggle((prev) => !prev);
  };

  const closeSidebar = () => setIsPDFSidebarOpen(false);

  return (
    <SidebarContainer isPanelOpen={isPanelOpen}>
      <SidebarHeader>
        {bookmarks.length > 0 &&
          React.createElement(toggle ? ViewTilesIcon : ViewRowsIcon, {
            onClick: handleToggle,
          })}
        <HideSidebarIcon onClick={closeSidebar} />
      </SidebarHeader>
      {toggle && <Bookmarks bookmarks={bookmarks} navigate={navigate} />}
      <Thumbnails id="viewer-thumbnail" visible={!toggle} />
    </SidebarContainer>
  );
}

export default Sidebar;
