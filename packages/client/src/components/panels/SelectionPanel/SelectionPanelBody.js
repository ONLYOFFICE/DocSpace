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
  exceptPrivacyTrashArchiveFolders,
  roomsOnly,
  userFolderOnly,
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
  isLoadingData,
  expandedKeys,
  isDisableTree,
  page,
  newFilter,
  isDisableButton,
  parentId,
  selectionFiles,
  displayType,
}) => {
  const isLoaded = folderId && resultingFolderTree;
  return (
    <StyledModalDialog
      visible={isPanelVisible}
      onClose={onClose}
      displayType="modal"
      isLoading={isLoading}
      withFooterBorder
      isDoubleFooterLine
      autoMaxWidth
    >
      {displayType !== "embedded" && (
        <ModalDialog.Header className={"select-panel-modal-header"}>
          {dialogName}
        </ModalDialog.Header>
      )}
      <ModalDialog.Body className="select-file_body-modal-dialog">
        <StyledBody header={!!header} footer={!!footer}>
          <div className="selection-panel_body">
            <div className="selection-panel_tree-body">
              {isLoaded ? (
                <FolderTreeBody
                  selectionFiles={selectionFiles}
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
                    className="selection-panel_title"
                    fontSize="12px"
                    fontWeight={600}
                    noSelect
                  >
                    {folderSelection
                      ? t("FolderContents", { folderTitle })
                      : filesListTitle}
                  </Text>
                </div>

                <FilesListWrapper
                  onSelectFile={onSelectFile}
                  folderId={folderId}
                  displayType={"modal"}
                  folderSelection={folderSelection}
                  newFilter={newFilter}
                  fileId={fileId}
                  maxHeight={!header ? 384 : 269}
                />
              </>
            </div>
          </div>
        </StyledBody>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        {footer}

        <div>
          <Button
            id="select-file-modal-submit"
            className="select-file-modal-dialog-buttons-save"
            primary
            size="normal"
            label={primaryButtonName}
            onClick={onButtonClick}
            isDisabled={
              isDisableButton ||
              (!fileId && !folderSelection) ||
              !(folderId && resultingFolderTree)
            }
            isLoading={isDisableTree}
          />
          <Button
            id="select-file-modal-cancel"
            className="modal-dialog-button"
            size="normal"
            label={t("Common:CancelButton")}
            onClick={onClose}
            isDisabled={isLoadingData}
          />
        </div>
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
    filteredType,
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
      switch (filteredType) {
        case "exceptSortedByTags":
          return filterFoldersTree(treeFolders, exceptSortedByTagsFolders);
        case "exceptPrivacyTrashArchiveFolders":
          return filterFoldersTree(
            treeFolders,
            exceptPrivacyTrashArchiveFolders
          );
        case "roomsOnly":
          return filterFoldersTree(treeFolders, roomsOnly);
        case "userFolderOnly":
          return filterFoldersTree(treeFolders, userFolderOnly);
      }
    };

    let filteredTreeFolders;

    const foldersTree =
      passedFoldersTree.length > 0 ? passedFoldersTree : treeFolders;

    if (
      filteredType === "exceptSortedByTags" ||
      filteredType === "exceptPrivacyTrashArchiveFolders" ||
      filteredType === "roomsOnly" ||
      filteredType === "userFolderOnly"
    ) {
      filteredTreeFolders = getExceptionsFolders(foldersTree);
    }

    const passedId = id
      ? id
      : filteredTreeFolders
      ? filteredTreeFolders[0].id
      : foldersTree[0].id;

    return [filteredTreeFolders || foldersTree, passedId];
  };
  render() {
    return <SelectionPanelBody {...this.props} />;
  }
}

export default SelectionPanel;
