import React from "react";

import { VariableSizeList as List } from "react-window";
import { isMobileOnly } from "react-device-detect";

import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import { ShareAccessRights } from "@docspace/common/constants";

import ExternalLink from "./ExternalLink";
import InternalLink from "./InternalLink";
import Item from "./Item";

import { StyledBodyContent } from "./StyledSharingPanel";

const Row = React.memo(({ data, index, style }) => {
  const {
    isMyId,
    externalAccessOptions,
    onChangeItemAccess,
    canShareOwnerChange,
    onShowChangeOwnerPanel,
    onRemoveUserClick,
    t,
    items,
    selection,
    onShowEmbeddingPanel,
    externalLinkOpen,
    onToggleExternalLinkOpen,
  } = data;

  if (items === undefined) return;

  if (!!items[index]?.sharedTo?.shareLink) {
    return (
      <ExternalLink
        t={t}
        isPersonal={false}
        selection={selection}
        externalItem={items[index]}
        externalAccessOptions={externalAccessOptions}
        onChangeItemAccess={onChangeItemAccess}
        onShowEmbeddingPanel={onShowEmbeddingPanel}
        isOpen={externalLinkOpen}
        onToggleLink={onToggleExternalLinkOpen}
        style={style}
      />
    );
  }

  if (!!items[index]?.internalLink) {
    return (
      <InternalLink
        t={t}
        internalLink={items[index]?.internalLink}
        style={style}
      />
    );
  }

  return (
    <Item
      t={t}
      item={items[index]}
      isMyId={isMyId}
      externalAccessOptions={externalAccessOptions}
      onChangeItemAccess={onChangeItemAccess}
      canShareOwnerChange={canShareOwnerChange(items[index])}
      onShowChangeOwnerPanel={onShowChangeOwnerPanel}
      onRemoveUserClick={onRemoveUserClick}
      isSeparator={items[index].isSeparator}
      style={style}
    />
  );
});

const Body = ({
  t,
  selection,
  externalItem,
  externalAccessOptions,
  onChangeItemAccess,
  onShowEmbeddingPanel,
  onToggleLink,
  internalLink,
  owner = {},
  isMyId,
  onShowChangeOwnerPanel,
  onRemoveUserClick,
  canShareOwnerChange,
  shareGroups = [],
  shareUsers = [],
  isPersonal,
  isShared,
}) => {
  const [externalLinkVisible, setExternalLinkVisible] = React.useState(false);
  const [externalLinkOpen, setExternalLinkOpen] = React.useState(isShared);
  const [itemList, setItemList] = React.useState([]);
  const [listData, setListData] = React.useState({});

  const bodyRef = React.useRef();
  const listRef = React.useRef();

  const onToggleExternalLinkOpen = React.useCallback(() => {
    onToggleLink && onToggleLink(externalItem);
  }, [externalItem, onToggleLink]);

  React.useEffect(() => {
    setExternalLinkVisible(
      selection?.length === 1 && !!externalItem?.sharedTo?.shareLink
    );

    setExternalLinkOpen(externalItem?.access !== ShareAccessRights.DenyAccess);
  }, [externalItem, selection]);

  const getItemSize = React.useCallback(
    (index) => {
      if (itemList.length === 0) return;
      if (itemList[index].isSeparator) return 16;
      if (itemList[index]?.internalLink) return 62;
      if (!!itemList[index]?.sharedTo.shareLink) {
        return externalLinkOpen ? 145 : 63;
      }
      return 48;
    },
    [itemList, externalLinkOpen]
  );

  React.useEffect(() => {
    if (!isPersonal) {
      const items = [];

      if (!!owner) {
        items.push(owner);
        items.push({ key: "separator-1", isSeparator: true });
      }

      if (shareGroups.length > 0) {
        items.push(...shareGroups);
        items.push({ key: "separator-2", isSeparator: true });
      }

      if (shareUsers.length > 0) {
        items.push(...shareUsers);
      }

      const newListData = {
        height: bodyRef?.current?.offsetHeight,
        width: "auto",
        data: {
          items: items,
          isMyId: isMyId,
          externalAccessOptions: externalAccessOptions,
          onChangeItemAccess: onChangeItemAccess,
          canShareOwnerChange: canShareOwnerChange,
          onShowChangeOwnerPanel: onShowChangeOwnerPanel,
          onRemoveUserClick: onRemoveUserClick,
          t: t,
        },
      };

      if (isMobileOnly) {
        if (!!internalLink) {
          items.unshift({ internalLink: internalLink });
        }
        if (selection?.length === 1 && !!externalItem?.sharedTo?.shareLink) {
          items.unshift(externalItem);
        }

        newListData.data.items = items;
        newListData.data.selection = selection;
        newListData.data.onShowEmbeddingPanel = onShowEmbeddingPanel;
        newListData.data.externalLinkOpen = externalLinkOpen;
        newListData.data.onToggleExternalLinkOpen = onToggleExternalLinkOpen;
      }

      setItemList(items);
      setListData(newListData);
    }
  }, [
    isPersonal,
    bodyRef.current,
    owner,
    shareGroups,
    shareUsers,
    isMyId,
    externalAccessOptions,
    onChangeItemAccess,
    canShareOwnerChange,
    onShowChangeOwnerPanel,
    onRemoveUserClick,
    t,
    externalItem,
    internalLink,
    selection,
    onShowEmbeddingPanel,
    externalLinkOpen,
    onToggleExternalLinkOpen,
  ]);

  React.useEffect(() => {
    if (!isPersonal) {
      listRef?.current?.resetAfterIndex(0);
    }
  }, [externalLinkOpen, isPersonal]);

  return (
    <>
      {isPersonal ? (
        <ExternalLink
          t={t}
          isPersonal={isPersonal}
          selection={selection}
          externalItem={externalItem}
          externalAccessOptions={externalAccessOptions}
          onChangeItemAccess={onChangeItemAccess}
          onShowEmbeddingPanel={onShowEmbeddingPanel}
          isOpen={externalLinkOpen}
          onToggleLink={onToggleExternalLinkOpen}
        />
      ) : (
        <>
          {!isMobileOnly ? (
            <StyledBodyContent
              externalLinkOpen={externalLinkOpen}
              externalLinkVisible={externalLinkVisible}
            >
              {externalLinkVisible && (
                <ExternalLink
                  t={t}
                  isPersonal={isPersonal}
                  selection={selection}
                  externalItem={externalItem}
                  externalAccessOptions={externalAccessOptions}
                  onChangeItemAccess={onChangeItemAccess}
                  onShowEmbeddingPanel={onShowEmbeddingPanel}
                  isOpen={externalLinkOpen}
                  onToggleLink={onToggleExternalLinkOpen}
                />
              )}

              {!!internalLink && (
                <InternalLink t={t} internalLink={internalLink} />
              )}

              <div className="body-scroll-content-sharing-panel" ref={bodyRef}>
                {listData?.height && listData?.data?.items?.length > 0 && (
                  <List
                    height={listData.height}
                    width={listData.width}
                    itemCount={itemList.length}
                    itemSize={getItemSize}
                    itemData={listData.data}
                    outerElementType={CustomScrollbarsVirtualList}
                  >
                    {Row}
                  </List>
                )}
              </div>
            </StyledBodyContent>
          ) : (
            <>
              <StyledBodyContent>
                <div
                  className="body-scroll-content-sharing-panel"
                  ref={bodyRef}
                >
                  {listData?.height && listData?.data?.items?.length > 0 && (
                    <List
                      height={listData.height}
                      width={listData.width}
                      itemCount={itemList.length}
                      itemSize={getItemSize}
                      itemData={listData.data}
                      outerElementType={CustomScrollbarsVirtualList}
                      ref={listRef}
                    >
                      {Row}
                    </List>
                  )}
                </div>
              </StyledBodyContent>
            </>
          )}
        </>
      )}
    </>
  );
};

export default React.memo(Body);
