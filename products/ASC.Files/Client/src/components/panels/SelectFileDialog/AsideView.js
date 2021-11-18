import React, { useState } from "react";
import {
  StyledAsidePanel,
  StyledSelectFilePanel,
  StyledHeaderContent,
} from "../StyledPanels";
import Text from "@appserver/components/text";
import SelectFolderInput from "../SelectFolderInput";
import FilesListBody from "./FilesListBody";
import Aside from "@appserver/components/aside";
import Heading from "@appserver/components/heading";
import Backdrop from "@appserver/components/backdrop";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";
import Loader from "@appserver/components/loader";
import EmptyContainer from "../../EmptyContainer/EmptyContainer";
import ModalDialog from "@appserver/components/modal-dialog";
const DISPLAY_TYPE = "aside";
const SelectFileDialogAsideView = ({
  t,
  isPanelVisible,
  zIndex,
  onClose,
  isVisible,
  withoutProvider,
  foldersType,
  onSelectFile,
  onClickInput,
  onCloseSelectFolderDialog,
  onSelectFolder,
  filesList,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  selectedFolder,
  header,
  loadingText,
  selectedFile,
  onClickSave,
  onSetFileName,
  fileName,
  displayType,
  isTranslationsReady,
  passedId,
  headerName,
  isAvailableFolderList,
  primaryButtonName,
}) => {
  const [isLoadingData, setIsLoadingData] = useState(false);
  const onSetLoadingData = (loading) => {
    setIsLoadingData(loading);
  };
  const isHeaderChildren = !!header;

  return (
    <StyledAsidePanel visible={isPanelVisible}>
      <ModalDialog
        visible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        //className="select-file-modal-dialog"
        //style={{ maxWidth: "890px" }}
        contentHeight="100%"
        displayType="aside"
        //bodyPadding="0"
        removeScroll
      >
        <ModalDialog.Header>
          {headerName ? headerName : t("SelectFile")}
        </ModalDialog.Header>
        <ModalDialog.Body className="select-file_body-modal-dialog">
          <StyledSelectFilePanel isHeaderChildren={isHeaderChildren}>
            <div className="select-file-dialog_aside-body_wrapper">
              <div className="select-file-dialog_aside-children">{header}</div>
              {/* <Text fontWeight="600" fontSize="14px">
                {t("Translations:SelectFolder")}
              </Text> */}
              <div className="select-file-dialog_aside_body">
                <SelectFolderInput
                  onClickInput={onClickInput}
                  onClose={onCloseSelectFolderDialog}
                  onSelectFolder={onSelectFolder}
                  onSetLoadingData={onSetLoadingData}
                  isPanelVisible={isVisible}
                  foldersType={foldersType}
                  isNeedArrowIcon
                  withoutProvider={withoutProvider}
                  isSetFolderImmediately
                  selectedFolderId={selectedFolder}
                  id={passedId}
                  onSetFileName={onSetFileName}
                  fileName={fileName}
                  displayType={displayType}
                  dialogWithFiles
                  showButtons
                  selectionButtonPrimary
                />

                {selectedFolder && !isLoadingData ? (
                  <FilesListBody
                    filesList={filesList}
                    onSelectFile={onSelectFile}
                    hasNextPage={hasNextPage}
                    isNextPageLoading={isNextPageLoading}
                    loadNextPage={loadNextPage}
                    selectedFolder={selectedFolder}
                    displayType={DISPLAY_TYPE}
                    loadingText={loadingText}
                    selectedFile={selectedFile}
                  />
                ) : isAvailableFolderList ? (
                  <div key="loader">
                    <Loader type="oval" size="16px" className="panel-loader" />
                    <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
                      "Common:LoadingDescription"
                    )}`}</Text>
                  </div>
                ) : (
                  <div className="select-file-dialog_empty-container">
                    <EmptyContainer
                      headerText={t("Home:EmptyFolderHeader")}
                      imageSrc="/static/images/empty_screen.png"
                    />
                  </div>
                )}
              </div>
            </div>
          </StyledSelectFilePanel>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <StyledSelectFilePanel isHeaderChildren={isHeaderChildren}>
            <div className="select-file-dialog-aside_buttons">
              <Button
                className="select-file-dialog-buttons-save"
                primary
                size="big"
                label={primaryButtonName}
                onClick={onClickSave}
                isDisabled={selectedFile.length === 0}
              />
              <Button
                size="big"
                label={t("Common:CancelButton")}
                onClick={onClose}
              />
            </div>
          </StyledSelectFilePanel>
        </ModalDialog.Footer>
      </ModalDialog>
    </StyledAsidePanel>
  );
};
export default SelectFileDialogAsideView;
