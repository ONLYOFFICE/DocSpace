import React from "react";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import AccessComboBox from "./AccessComboBox";
import { StyledItem } from "./StyledSharingPanel";

const Item = ({
  t,
  item,
  canShareOwnerChange,
  externalAccessOptions,
  onChangeItemAccess,
  onShowChangeOwnerPanel,
  onRemoveUserClick,
  isMyId,
  isSeparator,
  style,
}) => {
  const onShowChangeOwnerPanelAction = React.useCallback(() => {
    onShowChangeOwnerPanel && onShowChangeOwnerPanel();
  }, [onShowChangeOwnerPanel]);

  let itemName = "";
  let avatarUrl = "";

  if (!isSeparator) {
    itemName =
      item.sharedTo.id === isMyId
        ? t("Common:MeLabel")
        : !!item.sharedTo.displayName
        ? item.sharedTo.displayName
        : !!item.sharedTo.name
        ? item.sharedTo.name
        : item.sharedTo.label;

    avatarUrl = !!item.avatarSmall ? item.avatarSmall : item.avatarUrl;
  }

  return isSeparator ? (
    <StyledItem style={style} isSeparator={isSeparator} />
  ) : (
    <StyledItem style={style}>
      <div className="item__info-block">
        <Avatar
          className="info-block__avatar"
          size={"min"}
          role={"user"}
          source={avatarUrl}
          userName={itemName}
        />
        <Text className="info-block__text">{itemName}</Text>
      </div>
      {item.isOwner ? (
        canShareOwnerChange ? (
          <Text
            className="item__change-owner"
            onClick={onShowChangeOwnerPanelAction}
          >
            {t("ChangeOwnerPanel:ChangeOwner").replace("()", "")}
          </Text>
        ) : (
          <Text className="item__owner">{t("Common:Owner")}</Text>
        )
      ) : (
        <AccessComboBox
          t={t}
          access={item.access}
          directionX="right"
          directionY="both"
          accessOptions={externalAccessOptions}
          onAccessChange={onChangeItemAccess}
          itemId={item.sharedTo.id}
          isDisabled={false}
          disableLink={false}
          canDelete={true}
          onRemoveUserClick={onRemoveUserClick}
          isDefaultMode={true}
          fixedDirection={false}
        />
      )}
    </StyledItem>
  );
};

export default Item;
