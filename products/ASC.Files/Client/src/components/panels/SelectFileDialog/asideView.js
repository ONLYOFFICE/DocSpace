import React, { useState } from "react";
import {
  StyledAsidePanel,
  StyledSelectFilePanel,
  StyledHeaderContent,
} from "../StyledPanels";
import Text from "@appserver/components/text";
import SelectFolderInput from "../SelectFolderInput";
import FilesListBody from "./filesListBody";
import Aside from "@appserver/components/aside";
import Heading from "@appserver/components/heading";
import Backdrop from "@appserver/components/backdrop";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";
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
}) => {
  const [isLoadingData, setIsLoadingData] = useState(false);
  const onSetLoadingData = (loading) => {
    setIsLoadingData(loading);
  };

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
          <StyledSelectFilePanel displayType={DISPLAY_TYPE}>
            <StyledHeaderContent className="select-file-dialog_aside-header">
              <Heading
                size="medium"
                className="select-file-dialog_aside-header_title"
              >
                {header ? header : t("SelectFile")}
              </Heading>
            </StyledHeaderContent>

            <div className="select-file-dialog_aside-body_wrapper">
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
                />

                {selectedFolder && !isLoadingData && (
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
                )}
              </div>
            </div>
            <div className="select-file-dialog-aside_buttons">
              <Button
                className="select-file-dialog-buttons-save"
                primary
                size="medium"
                label={t("Common:SaveButton")}
                onClick={onClickSave}
                isDisabled={selectedFile.length === 0}
              />
              <Button
                primary
                size="medium"
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
