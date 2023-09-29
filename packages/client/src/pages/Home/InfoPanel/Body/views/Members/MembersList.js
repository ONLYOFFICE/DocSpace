import React, { useState, useCallback, useEffect, memo } from "react";
import styled from "styled-components";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import User from "./User";
import { isMobile } from "@docspace/components/utils/device";
import throttle from "lodash/throttle";
import Loaders from "@docspace/common/components/Loaders";

const StyledMembersList = styled.div`
  height: ${({ offsetTop }) => `calc(100vh - ${offsetTop})`};
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

  if (!user) {
    return (
      <div style={{ ...style, width: "calc(100% - 8px)", margin: "0 -16px" }}>
        <Loaders.SelectorRowLoader
          isMultiSelect={false}
          isContainer={true}
          isUser={true}
        />
      </div>
    );
  }

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
  } = props;

  const itemsCount = hasNextPage ? members.length + 1 : members.length;

  const canInviteUserInRoomAbility = security?.EditAccess;
  const [isNextPageLoading, setIsNextPageLoading] = useState(false);
  const [isMobileView, setIsMobileView] = useState(isMobile());

  const [offsetTop, setOffsetTop] = useState(0);

  const onResize = throttle(() => {
    const isMobileView = isMobile();
    setIsMobileView(isMobileView);
    setOffset();
  }, 300);

  const setOffset = () => {
    const rect = document
      .getElementById("infoPanelMembersList")
      ?.getBoundingClientRect();

    setOffsetTop(Math.ceil(rect?.top) + 2 + "px");
  };

  useEffect(() => {
    setOffset();
  }, [members]);

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
    <StyledMembersList id="infoPanelMembersList" offsetTop={offsetTop}>
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
