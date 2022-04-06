import React from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import IconButton from "@appserver/components/icon-button";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";

import {
  getCommonFoldersTree,
  getFolder,
  getFoldersTree,
  getThirdPartyFoldersTree,
} from "@appserver/common/api/files";
import toastr from "studio/toastr";
import {
  exceptSortedByTagsFolders,
  exceptPrivacyTrashFolders,
} from "./ExceptionFoldersConstants";

const SelectionPanelBody = ({
  isPanelVisible,
  isDataLoading,
  hasNextPage,
  resultingFolderTree,
  footerChild,
  headerChild,
  loadNextPage,
  loadingText,
  onButtonClick,
  onClose,
  onArrowClickAction,
  onSelectFile,
  onFolderClick,
  items,
  folderId,
  isNextPageLoading,
  isInitialLoader,
  page,
  title,
  t,
  selectedFileInfo,
  buttonText,
}) => {
  console.log("isRootPage", isRootPage);
  return <></>;
};

class SelectionPanel extends React.Component {
  static convertPathParts = (pathParts) => {
    let newPathParts = [];
    for (let i = 0; i < pathParts.length - 1; i++) {
      if (typeof pathParts[i] === "number") {
        newPathParts.push(String(pathParts[i]));
      } else {
        newPathParts.push(pathParts[i]);
      }
    }
    return newPathParts;
  };

  static setFolderObjectToTree = async (
    id,
    setSelectedNode,
    setExpandedPanelKeys,
    setSelectedFolder
  ) => {
    const data = await getFolder(id);

    setSelectedNode([id + ""]);
    const newPathParts = this.convertPathParts(data.pathParts);

    setExpandedPanelKeys(newPathParts);

    setSelectedFolder({
      folders: data.folders,
      ...data.current,
      pathParts: newPathParts,
      ...{ new: data.new },
    });
  };
  static getBasicFolderInfo = async (
    treeFolders,
    foldersType,
    id,
    onSetBaseFolderPath,
    onSelectFolder,
    foldersList,
    isSetFolderImmediately,
    setSelectedNode,
    setSelectedFolder,
    setExpandedPanelKeys
  ) => {
    //console.log("getBasicFolderInfo", setSelectedNode);
    //debugger;
    const getRequestFolderTree = () => {
      switch (foldersType) {
        case "exceptSortedByTags":
        case "exceptPrivacyTrashFolders":
          try {
            return getFoldersTree();
          } catch (err) {
            console.error(err);
          }
          break;
        case "common":
          try {
            return getCommonFoldersTree();
          } catch (err) {
            console.error(err);
          }
          break;

        case "third-party":
          try {
            return getThirdPartyFoldersTree();
          } catch (err) {
            console.error(err);
          }
          break;
      }
    };

    const filterFoldersTree = (folders, arrayOfExceptions) => {
      let newArray = [];

      for (let i = 0; i < folders.length; i++) {
        if (!arrayOfExceptions.includes(folders[i].rootFolderType)) {
          newArray.push(folders[i]);
        }
      }

      return newArray;
    };

    const getExceptionsFolders = (treeFolders) => {
      switch (foldersType) {
        case "exceptSortedByTags":
          return filterFoldersTree(treeFolders, exceptSortedByTagsFolders);
        case "exceptPrivacyTrashFolders":
          return filterFoldersTree(treeFolders, exceptPrivacyTrashFolders);
      }
    };

    let requestedTreeFolders, filteredTreeFolders, passedId;

    const treeFoldersLength = treeFolders.length;

    if (treeFoldersLength === 0) {
      try {
        requestedTreeFolders = foldersList
          ? foldersList
          : await getRequestFolderTree();
      } catch (e) {
        toastr.error(e);
        return;
      }
    }

    const foldersTree =
      treeFoldersLength > 0 ? treeFolders : requestedTreeFolders;

    if (id || isSetFolderImmediately || foldersType === "common") {
      passedId = id ? id : foldersTree[0].id;
      console.log("passedId", passedId, id, foldersTree[0].id);
      onSetBaseFolderPath && onSetBaseFolderPath(passedId);
      onSelectFolder && onSelectFolder(passedId);

      await SelectionPanel.setFolderObjectToTree(
        passedId,
        setSelectedNode,
        setExpandedPanelKeys,
        setSelectedFolder
      );
    }

    if (
      foldersType === "exceptSortedByTags" ||
      foldersType === "exceptPrivacyTrashFolders"
    ) {
      filteredTreeFolders = getExceptionsFolders(foldersTree);
    }
    console.log("base info requestedTreeFolders", requestedTreeFolders);
    return [filteredTreeFolders || requestedTreeFolders, passedId];
  };
  render() {
    return <SelectionPanelBody {...this.props} />;
  }
}

export default SelectionPanel;
