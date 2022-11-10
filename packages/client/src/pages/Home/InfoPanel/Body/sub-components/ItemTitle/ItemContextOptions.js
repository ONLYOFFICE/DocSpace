import React, { useRef, useEffect } from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";

import { ContextMenu, ContextMenuButton } from "@docspace/components";

import ContextHelper from "../../helpers/ContextHelper";

const StyledItemContextOptions = styled.div`
  margin-left: auto;
`;

const ItemContextOptions = ({
  t,
  selection,
  getContextOptions,
  getContextOptionActions,
  getUserContextOptions,
  setBufferSelection,

  isUser = false,
  itemTitleRef,
}) => {
  if (!selection) return null;

  const contextMenuRef = useRef();

  const contextHelper = new ContextHelper({
    t,
    isUser,
    selection,
    getContextOptions,
    getContextOptionActions,
    getUserContextOptions,
  });

  if (!selection) return null;

  const onContextMenu = (e) => {
    e.button === 2;
    if (!contextMenuRef.current.menuRef.current) itemTitleRef.current.click(e);
    contextMenuRef.current.show(e);
  };

  useEffect(() => {
    contextMenuRef.current.hide();
  }, [selection]);

  const options = contextHelper.getItemContextOptions();

  const getData = () => options;

  return (
    <StyledItemContextOptions>
      <ContextMenu
        ref={contextMenuRef}
        getContextModel={getData}
        withBackdrop={false}
      />
      {options.length > 0 && (
        <ContextMenuButton
          className="expandButton"
          title={"Show item actions"}
          onClick={onContextMenu}
          getData={getData}
          directionX="right"
          isNew={true}
        />
      )}
    </StyledItemContextOptions>
  );
};

export default inject(({ filesStore, peopleStore, contextOptionsStore }) => {
  const { getUserContextOptions } = peopleStore.contextOptionsStore;
  const {
    setBufferSelection,
    getFilesContextOptions: getContextOptions,
  } = filesStore;
  const {
    getFilesContextOptions: getContextOptionActions,
  } = contextOptionsStore;

  return {
    setBufferSelection,
    getContextOptions,
    getContextOptionActions,
    getUserContextOptions,
  };
})(observer(ItemContextOptions));
