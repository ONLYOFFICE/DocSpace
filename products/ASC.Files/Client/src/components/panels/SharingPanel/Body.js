import React from "react";

import Scrollbar from "@appserver/components/scrollbar";

import ExternalLink from "./ExternalLink";
import InternalLink from "./InternalLink";
import Item from "./Item";

import { StyledBodyContent } from "./StyledSharingPanel";

const Body = ({
  t,
  selection,
  externalItem,
  externalAccessOptions,
  onChangeItemAccess,
  onShowEmbeddingPanel,
  onToggleLink,
  internalLink,
  owner,
  isMyId,
  onShowChangeOwnerPanel,
  onRemoveUserClick,
  canShareOwnerChange,
  shareGroups,
  shareUsers,
}) => {
  const [externalLinkVisible, setExternalLinkVisible] = React.useState(false);

  React.useEffect(() => {
    setExternalLinkVisible(
      selection?.length === 1 && externalItem?.sharedTo?.shareLink
    );
  }, [externalItem, selection]);

  return (
    <StyledBodyContent>
      {externalLinkVisible && (
        <ExternalLink
          t={t}
          selection={selection}
          externalItem={externalItem}
          externalAccessOptions={externalAccessOptions}
          onChangeItemAccess={onChangeItemAccess}
          onShowEmbeddingPanel={onShowEmbeddingPanel}
          onToggleLink={onToggleLink}
        />
      )}
      {!!internalLink && <InternalLink t={t} internalLink={internalLink} />}

      <Scrollbar>
        {owner && (
          <>
            <Item
              t={t}
              item={owner}
              isMyId={isMyId}
              externalAccessOptions={externalAccessOptions}
              onChangeItemAccess={onChangeItemAccess}
              canShareOwnerChange={canShareOwnerChange(owner)}
              onShowChangeOwnerPanel={onShowChangeOwnerPanel}
              onRemoveUserClick={onRemoveUserClick}
            />
            <Item isSeparator={true} />
          </>
        )}

        {shareGroups && (
          <>
            {shareGroups.map((group) => (
              <Item
                key={group.sharedTo.id}
                t={t}
                item={group}
                isMyId={isMyId}
                externalAccessOptions={externalAccessOptions}
                onChangeItemAccess={onChangeItemAccess}
                canShareOwnerChange={canShareOwnerChange(group)}
                onShowChangeOwnerPanel={onShowChangeOwnerPanel}
                onRemoveUserClick={onRemoveUserClick}
              />
            ))}
            <Item isSeparator={true} />
          </>
        )}

        {shareUsers && (
          <>
            {shareUsers.map((user) => (
              <Item
                key={user.sharedTo.id}
                t={t}
                item={user}
                isMyId={isMyId}
                externalAccessOptions={externalAccessOptions}
                onChangeItemAccess={onChangeItemAccess}
                canShareOwnerChange={canShareOwnerChange(user)}
                onShowChangeOwnerPanel={onShowChangeOwnerPanel}
                onRemoveUserClick={onRemoveUserClick}
              />
            ))}
            <Item isSeparator={true} />
          </>
        )}
      </Scrollbar>
    </StyledBodyContent>
  );
};

export default React.memo(Body);
