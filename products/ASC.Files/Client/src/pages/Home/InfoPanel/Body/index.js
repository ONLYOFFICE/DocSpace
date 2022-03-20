import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { useParams } from "react-router-dom";
import CurrentFolder from "./CurrentFolder";
import api from "@appserver/common/api";
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
  ...props
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

    console.log(folder);
    setCurrentFolder(folder);
    setCurrentFolderLoading(false);
  };

  useEffect(() => {
    getCurrentFolderInfo();
  }, [window.location.href]);

  return (
    <StyledInfoRoomBody>
      {showCurrentFolder ? (
        <>
          {currentFolderLoading ? (
            <>Loading ...</>
          ) : (
            <SingleItem
              selectedItem={currentFolder}
              isRecycleBinFolder={isRecycleBinFolder}
              onSelectItem={onSelectItem}
              setSharingPanelVisible={setSharingPanelVisible}
              getFolderInfo={getFolderInfo}
              getIcon={getIcon}
              getFolderIcon={getFolderIcon}
              getShareUsers={getShareUsers}
            />
          )}
        </>
      ) : (
        <>
          {selectedItems.length === 0 && !bufferSelectedItem ? (
            <div className="no-item">
              <h4>{t("NoItemsSelected")}</h4>
            </div>
          ) : selectedItems.length === 1 || bufferSelectedItem ? (
            <SingleItem
              selectedItem={selectedItems[0] || bufferSelectedItem}
              isRecycleBinFolder={isRecycleBinFolder}
              onSelectItem={onSelectItem}
              setSharingPanelVisible={setSharingPanelVisible}
              getFolderInfo={getFolderInfo}
              getIcon={getIcon}
              getFolderIcon={getFolderIcon}
              getShareUsers={getShareUsers}
            />
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
    const {
      getFolderInfo,
      getShareUsers,
      getFileInfo,
      filesList,
      folders,
      viewAs,
    } = filesStore;
    const { getIcon, getFolderIcon } = settingsStore;
    const { onSelectItem } = filesActionsStore;
    const { setSharingPanelVisible } = dialogsStore;
    const { isRecycleBinFolder } = treeFoldersStore;

    console.log(filesList);
    console.log(folders);

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
      showCurrentFolder,
      getFileInfo,
      filesList,
      folders,
      viewAs,
    };
  }
)(withRouter(withTranslation(["InfoPanel"])(observer(InfoPanelBodyContent))));
