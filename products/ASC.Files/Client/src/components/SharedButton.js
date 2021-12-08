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
  isSmallIcon = false,
}) => {
  const color = shared ? "#657077" : "#a3a9ae";

  const onClickShare = () => {
    onSelectItem({ id, isFolder });
    setSharingPanelVisible(true);
  };

  return (
    <IconButton
      className="badge share-button-icon"
      color={color}
      hoverColor="#657077"
      size={isSmallIcon ? 10 : 16}
      iconName={"/static/images/catalog.share.react.svg"}
      onClick={onClickShare}
    />
  );
};

export default inject(({ filesActionsStore, dialogsStore }) => {
  return {
    onSelectItem: filesActionsStore.onSelectItem,
    setSharingPanelVisible: dialogsStore.setSharingPanelVisible,
  };
})(observer(SharedButton));
