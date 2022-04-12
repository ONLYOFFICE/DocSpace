import React from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import FolderTreeBody from "../../FolderTreeBody";
import Button from "@appserver/components/button";
import FilesListBody from "./FilesListBody";
import {
  getCommonFoldersTree,
  getFolder,
  getFoldersTree,
  getThirdPartyCommonFolderTree,
} from "@appserver/common/api/files";
import toastr from "studio/toastr";
import {
  exceptSortedByTagsFolders,
  exceptPrivacyTrashFolders,
} from "./ExceptionFoldersConstants";
import { StyledBody } from "./StyledSelectionPanel";
import Text from "@appserver/components/text";
import Loaders from "@appserver/common/components/Loaders";
const SelectionPanelBody = ({
  t,
  isPanelVisible,
  onClose,
  withoutProvider,
  onSelectFile,
  files,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
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
}) => {
  const onMouseEvent = (event) => {
    event.stopPropagation();
  };

  return (
    <div onMouseUp={onMouseEvent} onMouseDown={onMouseEvent}>
      <ModalDialog
        theme={theme}
        visible={isPanelVisible}
        onClose={onClose}
        className="select-file-modal-dialog"
        style={{ maxWidth: "773px" }}
        displayType="modal"
        modalBodyPadding="0px"
        isLoading={isLoading}
      >
        <ModalDialog.Header theme={theme}>{dialogName}</ModalDialog.Header>
        <ModalDialog.Body
          theme={theme}
          className="select-file_body-modal-dialog"
        >
          <StyledBody header={!!header}>
            <div className="selection-panel_body">
              <div className="selection-panel_tree-body">
                <Text
                  fontWeight="700"
                  fontSize="18px"
                  className="selection-panel_folder-title"
                >
                  {"Documents"}
                </Text>

                {folderId && resultingFolderTree ? (
                  <FolderTreeBody
                    theme={theme}
                    folderTree={resultingFolderTree}
                    onSelect={onSelectFolder}
                    withoutProvider={withoutProvider}
                    certainFolders
                    isAvailable={isAvailable}
                    selectedKeys={[`${folderId}`]}
                    expandedKeys={expandedKeys}
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
                        ? `The contents of the '${folderTitle}' folder`
                        : filesListTitle}
                    </Text>
                  </div>

                  <FilesListBody
                    theme={theme}
                    files={files}
                    onSelectFile={onSelectFile}
                    hasNextPage={hasNextPage}
                    isNextPageLoading={isNextPageLoading}
                    loadNextPage={loadNextPage}
                    folderId={folderId}
                    displayType={"modal"}
                    folderSelection={folderSelection}
                    fileId={fileId}
                  />
                </>
              </div>

              <div className="selection-panel_footer">
                <div>{footer}</div>

                <div className="selection-panel_buttons">
                  <Button
                    theme={theme}
                    className="select-file-modal-dialog-buttons-save"
                    primary
                    size="normalTouchscreen"
                    label={primaryButtonName}
                    onClick={onButtonClick}
                    isDisabled={
                      isLoadingData ||
                      (!fileId && !folderSelection) ||
                      !canCreate
                    }
                  />
                  <Button
                    theme={theme}
                    className="modal-dialog-button"
                    size="normalTouchscreen"
                    label={t("Common:CancelButton")}
                    onClick={onClose}
                    isDisabled={isLoadingData}
                  />
                </div>
              </div>
            </div>
          </StyledBody>
        </ModalDialog.Body>
      </ModalDialog>
    </div>
  );
};

class SelectionPanel extends React.Component {
  static setFolderObjectToTree = async (id) => {
    try {
      const data = await getFolder(id);
      const newPathParts = data.pathParts.map((item) => item.toString());
      return newPathParts;
    } catch (e) {
      toastr.error(e);
    }
  };
  static getBasicFolderInfo = async (
    treeFolders,
    foldersType,
    id,
    onSetBaseFolderPath,
    onSelectFolder,
    foldersList,
    isSetFolderImmediately,
    isFilesPanel = false
  ) => {
    //debugger;
    const isFilesModule =
      window.location.href.indexOf("products/files") !== -1 &&
      window.location.href.indexOf("doceditor") === -1;

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

      if (foldersType === "third-party") {
        isFilesPanel && onSetBaseFolderPath && onSetBaseFolderPath(passedId);
      } else {
        onSetBaseFolderPath && onSetBaseFolderPath(passedId);
      }

      onSelectFolder && onSelectFolder(passedId);
    }

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
