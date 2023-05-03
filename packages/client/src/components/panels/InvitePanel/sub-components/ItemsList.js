import React, { useState, useEffect, useRef, memo, useCallback } from "react";
import { inject, observer } from "mobx-react";
import { FixedSizeList as List } from "react-window";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import useResizeObserver from "use-resize-observer";
import Item from "./Item";

import { StyledRow, ScrollList } from "../StyledInvitePanel";

const FOOTER_HEIGHT = 70;
const USER_ITEM_HEIGHT = 48;

const Row = memo(({ data, index, style }) => {
  const {
    inviteItems,
    setInviteItems,
    changeInviteItem,
    t,
    setHasErrors,
    roomType,
    isOwner,
    inputsRef,
    setIsOpenItemAccess,
  } = data;

  if (inviteItems === undefined) return;

  const item = inviteItems[index];

  return (
    <StyledRow key={item.id} style={style}>
      <Item
        t={t}
        item={item}
        setInviteItems={setInviteItems}
        changeInviteItem={changeInviteItem}
        inviteItems={inviteItems}
        setHasErrors={setHasErrors}
        roomType={roomType}
        isOwner={isOwner}
        inputsRef={inputsRef}
        setIsOpenItemAccess={setIsOpenItemAccess}
      />
    </StyledRow>
  );
});

const ItemsList = ({
  t,
  setInviteItems,
  inviteItems,
  changeInviteItem,
  setHasErrors,
  roomType,
  isOwner,
  externalLinksVisible,
  scrollAllPanelContent,
  inputsRef,
}) => {
  const [bodyHeight, setBodyHeight] = useState(0);
  const [offsetTop, setOffsetTop] = useState(0);
  const [isTotalListHeight, setIsTotalListHeight] = useState(false);
  const [isOpenItemAccess, setIsOpenItemAccess] = useState(false);
  const bodyRef = useRef();
  const { height } = useResizeObserver({ ref: bodyRef });

  const onBodyResize = useCallback(() => {
    const scrollHeight = bodyRef?.current?.firstChild.scrollHeight;
    const heightList = height ? height : bodyRef.current.offsetHeight;
    const totalHeightItems = inviteItems.length * USER_ITEM_HEIGHT;
    const listAreaHeight = heightList;

    const calculatedHeight = scrollAllPanelContent
      ? Math.max(
          totalHeightItems,
          listAreaHeight,
          isOpenItemAccess ? scrollHeight : 0
        )
      : heightList - FOOTER_HEIGHT;

    setBodyHeight(calculatedHeight);
    setOffsetTop(bodyRef.current.offsetTop);

    if (scrollAllPanelContent && totalHeightItems && listAreaHeight)
      setIsTotalListHeight(
        totalHeightItems >= listAreaHeight && totalHeightItems >= scrollHeight
      );
  }, [
    height,
    bodyRef?.current?.offsetHeight,
    inviteItems.length,
    scrollAllPanelContent,
    isOpenItemAccess,
  ]);

  useEffect(() => {
    onBodyResize();
  }, [
    bodyRef.current,
    externalLinksVisible,
    height,
    inviteItems.length,
    scrollAllPanelContent,
    isOpenItemAccess,
  ]);

  const overflowStyle = scrollAllPanelContent ? "hidden" : "scroll";

  return (
    <ScrollList
      offsetTop={offsetTop}
      ref={bodyRef}
      scrollAllPanelContent={scrollAllPanelContent}
      isTotalListHeight={isTotalListHeight}
    >
      <List
        style={{ overflow: overflowStyle }}
        height={bodyHeight}
        width="auto"
        itemCount={inviteItems.length}
        itemSize={USER_ITEM_HEIGHT}
        itemData={{
          inviteItems,
          setInviteItems,
          changeInviteItem,
          setHasErrors,
          roomType,
          isOwner,
          inputsRef,
          setIsOpenItemAccess,
          t,
        }}
        outerElementType={!scrollAllPanelContent && CustomScrollbarsVirtualList}
      >
        {Row}
      </List>
    </ScrollList>
  );
};

export default inject(({ auth, dialogsStore }) => {
  const { setInviteItems, inviteItems, changeInviteItem } = dialogsStore;
  const { isOwner } = auth.userStore.user;

  return {
    setInviteItems,
    inviteItems,
    changeInviteItem,
    isOwner,
  };
})(observer(ItemsList));
