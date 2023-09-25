import React, { useState, useCallback, memo } from "react";
import styled from "styled-components";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import User from "./User";

const StyledMembersList = styled.div`
  height: ${({ withBanner }) =>
    withBanner ? "calc(100vh - 442px)" : "calc(100vh - 266px)"};
`;

const Item = memo(({ data, index, style }) => {
  const {
    t,
    members,
    security,
    membersHelper,
    currentMember,
    updateRoomMemberRole,
    selectionParentRoom,
    setSelectionParentRoom,
    changeUserType,
    setIsScrollLocked,
    canInviteUserInRoomAbility,
    onRepeatInvitation,
  } = data;

  const user = members[index];

  return (
    <div key={user.id} style={{ ...style, width: "calc(100% - 8px)" }}>
      <User
        t={t}
        user={user}
        key={user.id}
        security={security}
        membersHelper={membersHelper}
        currentMember={currentMember}
        updateRoomMemberRole={updateRoomMemberRole}
        roomId={selectionParentRoom.id}
        roomType={selectionParentRoom.roomType}
        selectionParentRoom={selectionParentRoom}
        setSelectionParentRoom={setSelectionParentRoom}
        changeUserType={changeUserType}
        setIsScrollLocked={setIsScrollLocked}
        isTitle={user.isTitle}
        isExpect={user.isExpect}
        showInviteIcon={canInviteUserInRoomAbility && user.isExpect}
        onRepeatInvitation={onRepeatInvitation}
      />
    </div>
  );
}, areEqual);

const MembersList = (props) => {
  const {
    t,
    security,
    membersHelper,
    currentMember,
    updateRoomMemberRole,
    selectionParentRoom,
    setSelectionParentRoom,
    changeUserType,
    setIsScrollLocked,
    members,
    hasNextPage,
    itemCount,
    onRepeatInvitation,
    loadNextPage,
    withBanner,
  } = props;

  const itemsCount = members.length;
  const canInviteUserInRoomAbility = security?.EditAccess;
  const [isNextPageLoading, setIsNextPageLoading] = useState(false);

  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < itemsCount;
    },
    [hasNextPage, itemsCount]
  );

  const loadMoreItems = useCallback(
    async (startIndex) => {
      setIsNextPageLoading(true);
      if (!isNextPageLoading) {
        await loadNextPage(startIndex - 1);
      }
      setIsNextPageLoading(false);
    },
    [isNextPageLoading, loadNextPage]
  );

  return (
    <StyledMembersList withBanner={withBanner}>
      <AutoSizer>
        {({ height, width }) => (
          <InfiniteLoader
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <List
                ref={ref}
                width={width + 20}
                height={height}
                itemCount={itemsCount}
                itemSize={48}
                itemData={{
                  t,
                  security,
                  membersHelper,
                  currentMember,
                  updateRoomMemberRole,
                  selectionParentRoom,
                  setSelectionParentRoom,
                  changeUserType,
                  setIsScrollLocked,
                  members,
                  canInviteUserInRoomAbility,
                  onRepeatInvitation,
                }}
                outerElementType={CustomScrollbarsVirtualList}
                onItemsRendered={onItemsRendered}
              >
                {Item}
              </List>
            )}
          </InfiniteLoader>
        )}
      </AutoSizer>
    </StyledMembersList>
  );
};

export default MembersList;
