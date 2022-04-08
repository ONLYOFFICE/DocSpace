import React from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import FolderTreeBody from "../../FolderTreeBody";
import Button from "@appserver/components/button";
import FilesListBody from "./FilesListBody";
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
import { StyledBody } from "./StyledSelectionPanel";
import Text from "@appserver/components/text";
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
  selectedFile,
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
  folderSelection,
  folderTitle,
}) => {
  return (
    <ModalDialog
      theme={theme}
      visible={isPanelVisible}
      onClose={onClose}
      className="select-file-modal-dialog"
      style={{ maxWidth: "773px" }}
      displayType="modal"
      modalBodyPadding="0px"
      isLoading={isLoading}
      modalLoaderBodyHeight="277px"
    >
      <ModalDialog.Header theme={theme}>{dialogName}</ModalDialog.Header>
      <ModalDialog.Body theme={theme} className="select-file_body-modal-dialog">
        <StyledBody>
          <div className="selection-panel_body">
            <div className="selection-panel_tree-body">
              <FolderTreeBody
                theme={theme}
                folderTree={resultingFolderTree}
                onSelect={onSelectFolder}
                withoutProvider={withoutProvider}
                certainFolders
                isAvailable={isAvailable}
                selectedKeys={[`${folderId}`]}
                displayType="modal"
              />
            </div>
            <div className="selection-panel_files-body">
              <>
                <div className="selection-panel_files-header">
                  {header}
                  {folderSelection ? (
                    <Text color="#A3A9AE">{`The contents of the '${folderTitle}' folder`}</Text>
                  ) : (
                    <Text theme={theme} className="modal-dialog-filter-title">
                      {filesListTitle}
                    </Text>
                  )}
                </div>
                {folderId && (
                  <FilesListBody
                    theme={theme}
                    files={files}
                    onSelectFile={onSelectFile}
                    hasNextPage={hasNextPage}
                    isNextPageLoading={isNextPageLoading}
                    loadNextPage={loadNextPage}
                    folderId={folderId}
                    selectedFile={selectedFile}
                    displayType={"modal"}
                    folderSelection
                  />
                )}
              </>
            </div>

            <div className="selection-panel_footer">
              {footer}
              <div className="selection-panel_buttons">
                <Button
                  theme={theme}
                  className="select-file-modal-dialog-buttons-save"
                  primary
                  size="small"
                  label={primaryButtonName}
                  onClick={onButtonClick}
                  //isDisabled={selectedFile.length === 0}
                />
                <Button
                  theme={theme}
                  className="modal-dialog-button"
                  size="small"
                  label={t("Common:CancelButton")}
                  onClick={onClose}
                />
              </div>
            </div>
          </div>
        </StyledBody>
      </ModalDialog.Body>
    </ModalDialog>
  );
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
      if (foldersType !== "third-party") {
        onSetBaseFolderPath && onSetBaseFolderPath(passedId);
        onSelectFolder && onSelectFolder(passedId);
      }

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
