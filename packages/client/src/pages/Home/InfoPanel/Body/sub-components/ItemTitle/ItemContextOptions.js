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
  setSelection,
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
    setSelection,
    getContextOptions,
    getContextOptionActions,
    getUserContextOptions,
  });

  if (!selection) return null;

  const setItemAsBufferSelection = () => setBufferSelection(selection);

  const onContextMenu = (e) => {
    e.button === 2;
    if (!contextMenuRef.current.menuRef.current) itemTitleRef.current.click(e);
    contextMenuRef.current.show(e);
  };

  useEffect(() => {
    contextMenuRef.current.hide();
  }, [selection]);

  console.log(selection);

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

export default inject(
  ({ auth, filesStore, peopleStore, contextOptionsStore }) => {
    const { setSelection } = auth.infoPanelStore;
    const { getUserContextOptions } = peopleStore.contextOptionsStore;
    const {
      setBufferSelection,
      getFilesContextOptions: getContextOptions,
    } = filesStore;
    const {
      getFilesContextOptions: getContextOptionActions,
    } = contextOptionsStore;

    return {
      setSelection,
      setBufferSelection,
      getContextOptions,
      getContextOptionActions,
      getUserContextOptions,
    };
  }
)(observer(ItemContextOptions));
