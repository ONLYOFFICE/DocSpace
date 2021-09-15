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
}) => {
  const [isLoadingData, setIsLoadingData] = useState(false);
  const onSetLoadingData = (loading) => {
    setIsLoadingData(loading);
  };
  const isHeaderChildren = !!header;

  return (
    <StyledAsidePanel visible={isPanelVisible}>
      <Backdrop
        onClick={onClose}
        visible={isPanelVisible}
        zIndex={zIndex}
        isAside={true}
      />
      <Aside visible={isPanelVisible} zIndex={zIndex}>
        {isTranslationsReady ? (
          <StyledSelectFilePanel
            displayType={DISPLAY_TYPE}
            isHeaderChildren={isHeaderChildren}
          >
            <StyledHeaderContent className="select-file-dialog_aside-header">
              <Heading
                size="medium"
                className="select-file-dialog_aside-header_title"
              >
                {headerName ? headerName : t("SelectFile")}
              </Heading>
            </StyledHeaderContent>

            <div className="select-file-dialog_aside-body_wrapper">
              <div className="select-file-dialog_aside-children">{header}</div>
              <Text fontWeight="600" fontSize="14px">
                {t("Translations:SelectFolder")}
              </Text>
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
            <div className="select-file-dialog-aside_buttons">
              <Button
                className="select-file-dialog-buttons-save"
                primary
                size="big"
                label={t("Common:SaveButton")}
                onClick={onClickSave}
                isDisabled={selectedFile.length === 0}
              />
              <Button
                primary
                size="big"
                label={t("Common:CloseButton")}
                onClick={onClose}
              />
            </div>
          </StyledSelectFilePanel>
        ) : (
          <Loaders.DialogAsideLoader withoutAside isPanel />
        )}
      </Aside>
    </StyledAsidePanel>
  );
};
export default SelectFileDialogAsideView;
