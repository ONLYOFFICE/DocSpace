import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";
import { StyledInfoRoomBody } from "./styles/styles.js";
import Loader from "@appserver/components/loader";
import { Base } from "@appserver/components/themes";

const InfoPanelBodyContent = ({
  t,
  selectedFolder,
  selectedItems,
  getFolderInfo,
  getIcon,
  getFolderIcon,
  getShareUsers,
  onSelectItem,
  setSharingPanelVisible,
  isRecycleBinFolder,
  isCommonFolder,
  isRecentFolder,
  isFavoritesFolder,
}) => {
  console.log("--- ||| RENDER INFO PANEL ||| ---");

  const singleItem = (item) => {
    const dontShowLocation = item.isFolder && item.parentId === 0;
    const dontShowSize = item.isFolder && (isFavoritesFolder || isRecentFolder);
    const dontShowAccess =
      isRecycleBinFolder || isCommonFolder || selectedItems.length === 0;
    return (
      <SingleItem
        t={t}
        selectedItem={item}
        onSelectItem={onSelectItem}
        setSharingPanelVisible={setSharingPanelVisible}
        getFolderInfo={getFolderInfo}
        getIcon={getIcon}
        getFolderIcon={getFolderIcon}
        getShareUsers={getShareUsers}
        dontShowLocation={dontShowLocation}
        dontShowSize={dontShowSize}
        dontShowAccess={dontShowAccess}
      />
    );
  };

  return (
    <StyledInfoRoomBody>
      <>
        {selectedItems.length === 0 ? (
          singleItem({
            ...selectedFolder,
            isFolder: true,
          })
        ) : selectedItems.length === 1 ? (
          singleItem(selectedItems[0])
        ) : (
          <SeveralItems selectedItems={selectedItems} getIcon={getIcon} />
        )}
      </>
    </StyledInfoRoomBody>
  );
};

InfoPanelBodyContent.defaultProps = { theme: Base };

export default inject(
  ({
    filesStore,
    settingsStore,
    filesActionsStore,
    dialogsStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const { selection, getFolderInfo, getShareUsers } = filesStore;
    const { getIcon, getFolderIcon } = settingsStore;
    const { onSelectItem } = filesActionsStore;
    const { setSharingPanelVisible } = dialogsStore;
    const {
      isRecycleBinFolder,
      isCommonFolder,
      isRecentFolder,
      isFavoritesFolder,
    } = treeFoldersStore;

    return {
      selectedFolder: { ...selectedFolderStore },
      selectedItems: [...selection],
      getFolderInfo,
      getShareUsers,
      getIcon,
      getFolderIcon,
      onSelectItem,
      setSharingPanelVisible,
      isRecycleBinFolder,
      isCommonFolder,
      isRecentFolder,
      isFavoritesFolder,
    };
  }
)(
  withRouter(
    withTranslation(["InfoPanel", "Home", "Common", "Translations"])(
      observer(InfoPanelBodyContent)
    )
  )
);
