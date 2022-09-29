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
import toastr from "@docspace/components/toast/toastr";
import {
  exceptSortedByTagsFolders,
  exceptPrivacyTrashFolders,
} from "./ExceptionFoldersConstants";
import { StyledBody, StyledModalDialog } from "./StyledSelectionPanel";
import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";
import { FolderType } from "@docspace/common/constants";

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
  folderSelectionDisabled,
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
                {t("Common:Rooms")}
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
                  maxHeight={!header ? 384 : 310}
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
            folderSelectionDisabled ||
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
    passedFoldersTree = [],
    hasSharedFolder
  ) => {
    const filterFoldersTree = (folders, arrayOfExceptions) => {
      const arr = !hasSharedFolder
        ? [...arrayOfExceptions, FolderType.Rooms]
        : arrayOfExceptions;

      let newArray = [];

      for (let i = 0; i < folders.length; i++) {
        if (!arr.includes(folders[i].rootFolderType)) {
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

    let filteredTreeFolders;

    const foldersTree =
      passedFoldersTree.length > 0 ? passedFoldersTree : treeFolders;

    const passedId = id ? id : foldersTree[0].id;

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
