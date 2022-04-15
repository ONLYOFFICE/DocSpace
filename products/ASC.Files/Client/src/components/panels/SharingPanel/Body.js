import React from "react";

import { VariableSizeList as List } from "react-window";

import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import { ShareAccessRights } from "@appserver/common/constants";

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
  } = data;

  if (items === undefined) return;

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
}) => {
  const [externalLinkVisible, setExternalLinkVisible] = React.useState(false);
  const [externalLinkOpen, setExternalLinkOpen] = React.useState(false);
  const [hideDropDown, setHideDropDown] = React.useState(false);
  const [itemList, setItemList] = React.useState([]);
  const [listData, setListData] = React.useState({});

  const bodyRef = React.useRef();

  const onToggleExternalLinkOpen = React.useCallback(() => {
    setExternalLinkOpen((oldState) => !oldState);
    onToggleLink && onToggleLink(externalItem);
  }, [externalItem, onToggleLink]);

  React.useEffect(() => {
    setExternalLinkVisible(
      selection?.length === 1 && !!externalItem?.sharedTo?.shareLink
    );
    setExternalLinkOpen(externalItem?.access !== ShareAccessRights.DenyAccess);
  }, [externalItem, selection]);

  const onScrollStart = React.useCallback(() => {
    setHideDropDown(true);
  }, []);

  const onScrollStop = React.useCallback(() => {
    setHideDropDown(false);
  }, []);

  const getItemSize = React.useCallback(
    (index) => {
      if (itemList.length === 0) return;
      if (itemList[index].isSeparator) return 16;
      return 48;
    },
    [itemList]
  );

  React.useEffect(() => {
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

    setItemList(items);

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

    setListData(newListData);
  }, [
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
  ]);

  return (
    <StyledBodyContent
      externalLinkOpen={externalLinkOpen}
      externalLinkVisible={externalLinkVisible}
    >
      {externalLinkVisible && (
        <ExternalLink
          t={t}
          selection={selection}
          externalItem={externalItem}
          externalAccessOptions={externalAccessOptions}
          onChangeItemAccess={onChangeItemAccess}
          onShowEmbeddingPanel={onShowEmbeddingPanel}
          isOpen={externalLinkOpen}
          onToggleLink={onToggleExternalLinkOpen}
        />
      )}

      {!!internalLink && <InternalLink t={t} internalLink={internalLink} />}

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
  );
};

export default React.memo(Body);
