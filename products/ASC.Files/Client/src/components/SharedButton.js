import React from "react";
import Text from "@appserver/components/text";
import IconButton from "@appserver/components/icon-button";
import { inject, observer } from "mobx-react";

const SharedButton = ({
  t,
  id,
  isFolder,
  shared,
  onSelectItem,
  setSharingPanelVisible,
}) => {
  const color = shared ? "#657077" : "#a3a9ae";

  const onClickShare = () => {
    onSelectItem({ id, isFolder });
    setSharingPanelVisible(true);
  };

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
        hoverColor="#657077"
        size={18}
        iconName="/static/images/catalog.shared.react.svg"
      />
      {t("Share")}
    </Text>
  );
};

export default inject(({ filesActionsStore, dialogsStore }) => {
  return {
    onSelectItem: filesActionsStore.onSelectItem,
    setSharingPanelVisible: dialogsStore.setSharingPanelVisible,
  };
})(observer(SharedButton));
