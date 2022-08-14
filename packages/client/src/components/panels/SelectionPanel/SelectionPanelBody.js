import React from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import FolderTreeBody from "../../FolderTreeBody";
import Button from "@docspace/components/button";
import FilesListWrapper from "./FilesListWrapper";
import {
  getCommonFoldersTree,
  getFolder,
  getFoldersTree,
  getSharedRoomsTree,
  getThirdPartyCommonFolderTree,
} from "@docspace/common/api/files";
import toastr from "client/toastr";
import {
  exceptSortedByTagsFolders,
  exceptPrivacyTrashFolders,
} from "./ExceptionFoldersConstants";
import { StyledBody, StyledModalDialog } from "./StyledSelectionPanel";
import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";
const SelectionPanelBody = ({
  t,
  isPanelVisible,
  onClose,
  withoutProvider,
  onSelectFile,
  filesListTitle,
  dialogName,
  primaryButtonName,
  theme,
  isLoading,
  onButtonClick,
  folderId,
  onSelectFolder,
  resultingFolderTree,
  isAvailable,
  footer,
  header,
  folderSelection = false,
  folderTitle,
  fileId,
  canCreate = true,
  isLoadingData,
  expandedKeys,
  isDisableTree,
  page,
  newFilter,
  isDisableButton,
  parentId,
  selectionFiles,
}) => {
  return (
    <StyledModalDialog
      theme={theme}
      visible={isPanelVisible}
      onClose={onClose}
      displayType="modal"
      isLoading={isLoading}
      withFooterBorder
      autoMaxWidth
    >
      <ModalDialog.Header theme={theme} className={"select-panel-modal-header"}>
        {dialogName}
      </ModalDialog.Header>
      <ModalDialog.Body theme={theme} className="select-file_body-modal-dialog">
        <StyledBody header={!!header} footer={!!footer}>
          <div className="selection-panel_body">
            <div className="selection-panel_tree-body">
              <Text
                fontWeight="700"
                fontSize="18px"
                className="selection-panel_folder-title"
              >
                {t("Common:Documents")}
              </Text>

              {folderId && resultingFolderTree ? (
                <FolderTreeBody
                  selectionFiles={selectionFiles}
                  theme={theme}
                  folderTree={resultingFolderTree}
                  onSelect={onSelectFolder}
                  withoutProvider={withoutProvider}
                  certainFolders
                  isAvailable={isAvailable}
                  selectedKeys={[`${folderId}`]}
                  parentId={parentId}
                  expandedKeys={expandedKeys}
                  isDisableTree={isDisableTree}
                  displayType="modal"
                />
              ) : (
                <Loaders.NewTreeFolders />
              )}
            </div>
            <div className="selection-panel_files-body">
              <>
                <div className="selection-panel_files-header">
                  {header}

                  <Text
                    color="#A3A9AE"
                    theme={theme}
                    className="selection-panel_title"
                  >
                    {folderSelection
                      ? t("FolderContents", { folderTitle })
                      : filesListTitle}
                  </Text>
                </div>

                <FilesListWrapper
                  theme={theme}
                  onSelectFile={onSelectFile}
                  folderId={folderId}
                  displayType={"modal"}
                  folderSelection={folderSelection}
                  newFilter={newFilter}
                  fileId={fileId}
                />
              </>
            </div>
          </div>
        </StyledBody>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          theme={theme}
          className="select-file-modal-dialog-buttons-save"
          primary
          size="normalTouchscreen"
          label={primaryButtonName}
          onClick={onButtonClick}
          isDisabled={
            isDisableButton ||
            isDisableTree ||
            isLoadingData ||
            (!fileId && !folderSelection) ||
            !canCreate
          }
          isLoading={isDisableTree}
        />
        <Button
          theme={theme}
          className="modal-dialog-button"
          size="normalTouchscreen"
          label={t("Common:CancelButton")}
          onClick={onClose}
          isDisabled={isLoadingData}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

class SelectionPanel extends React.Component {
  static getFolderPath = async (id) => {
    try {
      const data = await getFolder(id);
      const newPathParts = data.pathParts.map((item) => item.toString());

      +newPathParts[newPathParts.length - 1] === +id && newPathParts.pop();

      return newPathParts;
    } catch (e) {
      toastr.error(e);
      return null;
    }
  };
  static getBasicFolderInfo = async (
    treeFolders,
    foldersType,
    id,
    onSetBaseFolderPath,
    onSelectFolder,
    foldersList,
    withoutBasicSelection = false
  ) => {
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
            return getThirdPartyCommonFolderTree();
          } catch (err) {
            console.error(err);
          }
          break;
        case "rooms":
          try {
            return getSharedRoomsTree();
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

    let requestedTreeFolders, filteredTreeFolders;

    const treeFoldersLength = treeFolders?.length;

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

    const passedId = id ? id : foldersTree[0]?.id;

    !withoutBasicSelection &&
      onSetBaseFolderPath &&
      onSetBaseFolderPath(passedId);

    !withoutBasicSelection && onSelectFolder && onSelectFolder(passedId);

    if (
      foldersType === "exceptSortedByTags" ||
      foldersType === "exceptPrivacyTrashFolders"
    ) {
      filteredTreeFolders = getExceptionsFolders(foldersTree);
    }

    return [filteredTreeFolders || foldersTree, passedId];
  };
  render() {
    return <SelectionPanelBody {...this.props} />;
  }
}

export default SelectionPanel;
