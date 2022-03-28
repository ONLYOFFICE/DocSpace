import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";
import { StyledInfoRoomBody } from "./styles/styles.js";
import Loader from "@appserver/components/loader";

const InfoPanelBodyContent = ({
  t,
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
  const [currentFolderLoading, setCurrentFolderLoading] = useState(false);
  const [currentFolder, setCurrentFolder] = useState({});

  const getCurrentFolderInfo = async () => {
    setCurrentFolderLoading(true);

    const folderId = new URL(window.location.href).searchParams.get("folder");
    const folder = await getFolderInfo(folderId);

    const fileExst = folder.fileExst,
      providerKey = folder.providerKey,
      contentLength = folderId.contentLength;

    folder.iconUrl = getIcon(32, fileExst, providerKey, contentLength);
    //folder.thumbnailUrl = getIcon(96, fileExst, providerKey, contentLength);
    folder.isFolder = true;

    setCurrentFolder(folder);
    setCurrentFolderLoading(false);
  };

  useEffect(() => {
    getCurrentFolderInfo();
  }, [window.location.href]);

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
          <div className="no-item">
            {currentFolderLoading ? (
              <div className="current-folder-loader-wrapper">
                <Loader type="oval" size="48px" />
              </div>
            ) : (
              singleItem(currentFolder)
            )}
          </div>
        ) : selectedItems.length === 1 ? (
          singleItem(selectedItems[0])
        ) : (
          <SeveralItems selectedItems={selectedItems} getIcon={getIcon} />
        )}
      </>
    </StyledInfoRoomBody>
  );
};

export default inject(
  ({
    filesStore,
    settingsStore,
    filesActionsStore,
    dialogsStore,
    treeFoldersStore,
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
      selectedItems: selection,
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
