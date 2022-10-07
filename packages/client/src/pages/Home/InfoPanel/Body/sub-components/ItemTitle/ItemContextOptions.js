import React, { useRef, useEffect } from "react";
import styled from "styled-components";

import {
  ContextMenu,
  ContextMenuButton,
  IconButton,
} from "@docspace/components";

import ContextHelper from "../../helpers/ContextHelper";

const StyledItemContextOptions = styled.div`
  margin-left: auto;
`;

const ItemContextOptions = ({
  selection,
  setBufferSelection,
  itemTitleRef,
  ...props
}) => {
  if (!selection) return null;

  const contextMenuRef = useRef();

  const contextHelper = new ContextHelper({
    selection,
    ...props,
  });

  const setItemAsBufferSelection = () => setBufferSelection(selection);

  const onContextMenu = (e) => {
    e.button === 2;
    if (!contextMenuRef.current.menuRef.current) itemTitleRef.current.click(e);
    contextMenuRef.current.show(e);
  };

  return (
    <StyledItemContextOptions onClick={setItemAsBufferSelection}>
      <ContextMenu
        ref={contextMenuRef}
        getContextModel={contextHelper.getItemContextOptions}
        withBackdrop={false}
      />
      <ContextMenuButton
        className="expandButton"
        title={"Show item actions"}
        onClick={onContextMenu}
        getData={contextHelper.getItemContextOptions}
        directionX="right"
        isNew={true}
      />
    </StyledItemContextOptions>
  );
};

export default ItemContextOptions;
