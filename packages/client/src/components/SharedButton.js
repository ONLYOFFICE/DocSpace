﻿import CatalogShareSmallReactSvgUrl from "PUBLIC_DIR/images/catalog.share.small.react.svg?url";
import CatalogSharedReactSvgUrl from "PUBLIC_DIR/images/catalog.shared.react.svg?url";
import React from "react";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import { inject, observer } from "mobx-react";

const SharedButton = ({
  t,
  id,
  isFolder,
  shared,
  onSelectItem,
  setSharingPanelVisible,
  isSmallIcon = false,
  theme,
}) => {
  const color = shared
    ? theme.filesSharedButton.sharedColor
    : theme.filesSharedButton.color;

  const onClickShare = () => {
    onSelectItem({ id, isFolder });
    setSharingPanelVisible(true);
  };

  const icon = isSmallIcon
    ? CatalogShareSmallReactSvgUrl
    : CatalogSharedReactSvgUrl;

  return (
    <Text
      className="share-button"
      as="span"
      title={t("Share")}
      fontSize="12px"
      fontWeight={600}
      color={color}
      display="inline-flex"
      onClick={onClickShare}
    >
      <IconButton
        className="share-button-icon"
        color={color}
        hoverColor={theme.filesSharedButton.sharedColor}
        size={isSmallIcon ? 12 : 16}
        iconName={icon}
      />
      {t("Share")}
    </Text>
  );
};

export default inject(({ auth, filesActionsStore, dialogsStore }) => {
  return {
    theme: auth.settingsStore.theme,
    onSelectItem: filesActionsStore.onSelectItem,
    setSharingPanelVisible: dialogsStore.setSharingPanelVisible,
  };
})(observer(SharedButton));
