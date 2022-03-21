import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";
import { StyledInfoRoomBody } from "./styles/styles.js";

const InfoPanelBodyContent = ({
  t,
  selectedItems,
  bufferSelectedItem,
  getFolderInfo,
  getIcon,
  getFolderIcon,
  getShareUsers,
  onSelectItem,
  setSharingPanelVisible,
  isRecycleBinFolder,
  showCurrentFolder,
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
    folder.thumbnailUrl = getIcon(96, fileExst, providerKey, contentLength);
    folder.isFolder = true;

    setCurrentFolder(folder);
    setCurrentFolderLoading(false);
  };

  useEffect(() => {
    if (showCurrentFolder) getCurrentFolderInfo();
  }, [window.location.href, showCurrentFolder]);

  const singleItem = (item) => {
    const dontShowLocation = item.isFolder && item.parentId === 0;
    const dontShowSize = item.isFolder && (isFavoritesFolder || isRecentFolder);
    const dontShowAccess =
      showCurrentFolder || isRecycleBinFolder || isCommonFolder;
    return (
      <SingleItem
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
      {showCurrentFolder ? (
        <>
          {currentFolderLoading ? <>Loading ...</> : singleItem(currentFolder)}
        </>
      ) : (
        <>
          {selectedItems.length === 0 && !bufferSelectedItem ? (
            <div className="no-item">
              <h4>{t("NoItemsSelected")}</h4>
            </div>
          ) : selectedItems.length === 1 || bufferSelectedItem ? (
            singleItem(selectedItems[0] || bufferSelectedItem)
          ) : (
            <SeveralItems selectedItems={selectedItems} getIcon={getIcon} />
          )}
        </>
      )}
    </StyledInfoRoomBody>
  );
};

export default inject(
  ({
    infoPanelStore,
    filesStore,
    settingsStore,
    filesActionsStore,
    dialogsStore,
    treeFoldersStore,
  }) => {
    const selectedItems = JSON.parse(JSON.stringify(filesStore.selection));
    const bufferSelectedItem = JSON.parse(
      JSON.stringify(filesStore.bufferSelection)
    );

    const { showCurrentFolder } = infoPanelStore;
    const { getFolderInfo, getShareUsers } = filesStore;
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
      bufferSelectedItem,
      selectedItems,
      getFolderInfo,
      getShareUsers,
      getIcon,
      getFolderIcon,
      onSelectItem,
      setSharingPanelVisible,
      isRecycleBinFolder,
      isCommonFolder,
      showCurrentFolder,
      isRecentFolder,
      isFavoritesFolder,
    };
  }
)(withRouter(withTranslation(["InfoPanel"])(observer(InfoPanelBodyContent))));
