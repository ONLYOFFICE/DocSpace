import React, { useState, useCallback, useEffect, memo } from "react";
import styled from "styled-components";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import User from "./User";
import { tablet, mobile, isMobile } from "@docspace/components/utils/device";

const StyledMembersList = styled.div`
  height: ${({ withBanner, isPublicRoomType }) =>
    isPublicRoomType
      ? withBanner
        ? "calc(100vh - 442px)"
        : "calc(100vh - 286px)"
      : "calc(100vh - 266px)"};

  @media ${tablet} {
    height: ${({ withBanner, isPublicRoomType }) =>
      isPublicRoomType
        ? withBanner
          ? "calc(100vh - 362px)"
          : "calc(100vh - 206px)"
        : "calc(100vh - 186px)"};
  }

  @media ${mobile} {
    height: ${({ withBanner, isPublicRoomType }) =>
      isPublicRoomType
        ? withBanner
          ? "calc(100vh - 426px)"
          : "calc(100vh - 270px)"
        : "calc(100vh - 250px)"};
  }
`;

const Item = memo(({ data, index, style }) => {
  const {
    t,
    members,
    setMembers,
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
        setMembers={setMembers}
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
    setMembers,
    hasNextPage,
    itemCount,
    onRepeatInvitation,
    loadNextPage,
    isPublicRoomType,
    withBanner,
  } = props;

  const itemsCount = members.length;

  const canInviteUserInRoomAbility = security?.EditAccess;
  const [isNextPageLoading, setIsNextPageLoading] = useState(false);
  const [isMobileView, setIsMobileView] = useState(isMobile());

  const onResize = () => {
    const isMobileView = isMobile();
    setIsMobileView(isMobileView);
  };

  useEffect(() => {
    window.addEventListener("resize", onResize);

    return () => {
      window.removeEventListener("resize", onResize);
    };
  });

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
    <StyledMembersList
      withBanner={withBanner}
      isPublicRoomType={isPublicRoomType}
    >
      <AutoSizer>
        {({ height, width }) => (
          <InfiniteLoader
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => {
              const listWidth = isMobileView ? width + 16 : width + 20; // for scroll

              return (
                <List
                  ref={ref}
                  width={listWidth}
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
                    setMembers,
                    canInviteUserInRoomAbility,
                    onRepeatInvitation,
                  }}
                  outerElementType={CustomScrollbarsVirtualList}
                  onItemsRendered={onItemsRendered}
                >
                  {Item}
                </List>
              );
            }}
          </InfiniteLoader>
        )}
      </AutoSizer>
    </StyledMembersList>
  );
};

export default MembersList;
