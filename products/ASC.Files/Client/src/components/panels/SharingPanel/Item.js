import React from "react";

import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import AccessComboBox from "./AccessComboBox";
import { StyledItem } from "./StyledSharingPanel";

const Item = ({
  t,
  item,
  canShareOwnerChange,
  externalAccessOptions,
  onChangeItemAccess,
  label,
  avatarUrl,
  isOwner,
  ownerText,
  onShowChangeOwnerPanel,
  changeOwnerText,
  access,
  onRemoveUserClick,
}) => {
  const onShowChangeOwnerPanelAction = React.useCallback(() => {
    onShowChangeOwnerPanel && onShowChangeOwnerPanel();
  }, [onShowChangeOwnerPanel]);

  return (
    <StyledItem isEndOfBlock={isOwner || item?.isEndOfBlock}>
      <div className="item__info-block">
        <Avatar
          className="info-block__avatar"
          size={"min"}
          role={"user"}
          source={avatarUrl}
          userName={label}
        />
        <Text className="info-block__text">{label}</Text>
      </div>
      {isOwner ? (
        canShareOwnerChange ? (
          <Text
            className="item__change-owner"
            onClick={onShowChangeOwnerPanelAction}
          >
            {changeOwnerText}
          </Text>
        ) : (
          <Text className="item__owner">{ownerText}</Text>
        )
      ) : (
        <AccessComboBox
          t={t}
          access={access}
          directionX="right"
          directionY="bottom"
          accessOptions={externalAccessOptions}
          onAccessChange={onChangeItemAccess}
          itemId={item.sharedTo.id}
          isDisabled={false}
          disableLink={false}
          canDelete={true}
          onRemoveUserClick={onRemoveUserClick}
          fixedDirection={item?.isFixedDirection}
        />
      )}
    </StyledItem>
  );
};

export default Item;
