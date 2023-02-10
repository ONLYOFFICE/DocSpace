import React, { useState, useEffect, useRef, memo, useCallback } from "react";
import { inject, observer } from "mobx-react";
import { FixedSizeList as List } from "react-window";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import Item from "./Item";

import { StyledRow, ScrollList } from "../StyledInvitePanel";

const FOOTER_HEIGHT = 70;

const Row = memo(({ data, index, style }) => {
  const {
    inviteItems,
    setInviteItems,
    changeInviteItem,
    t,
    setHasErrors,
    roomType,
    isOwner,
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
}) => {
  const [bodyHeight, setBodyHeight] = useState(0);
  const [offsetTop, setOffsetTop] = useState(0);
  const bodyRef = useRef();

  const onBodyResize = useCallback(() => {
    setBodyHeight(bodyRef.current.offsetHeight - FOOTER_HEIGHT);
    setOffsetTop(bodyRef.current.offsetTop);
  }, [bodyRef?.current?.offsetHeight]);

  useEffect(() => {
    onBodyResize();
  }, [bodyRef.current]);

  useEffect(() => {
    window.addEventListener("resize", onBodyResize);
    return () => {
      window.removeEventListener("resize", onBodyResize);
    };
  }, []);

  return (
    <ScrollList offsetTop={offsetTop} ref={bodyRef}>
      <List
        height={bodyHeight}
        width="auto"
        itemCount={inviteItems.length}
        itemSize={48}
        itemData={{
          inviteItems,
          setInviteItems,
          changeInviteItem,
          setHasErrors,
          roomType,
          isOwner,
          t,
        }}
        outerElementType={CustomScrollbarsVirtualList}
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
