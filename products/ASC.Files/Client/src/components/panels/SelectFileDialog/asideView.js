import React from "react";
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
const DISPLAY_TYPE = "aside";
const SelectFileDialogAsideView = ({
  t,
  isPanelVisible,
  zIndex,
  onClose,
  isVisible,
  withoutProvider,
  foldersType,
  isLoadingData,
  onSelectFile,
  onClickInput,
  onCloseSelectFolderDialog,
  onSelectFolder,
  onSetLoadingData,
  filesList,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  selectedFolder,
  header,
  loadingText,
}) => {
  console.log("isLoadingData", isLoadingData, "selectedFolder", selectedFolder);
  return (
    <StyledAsidePanel visible={isPanelVisible}>
      <Backdrop
        onClick={onClose}
        visible={isPanelVisible}
        zIndex={zIndex}
        isAside={true}
      />
      <Aside visible={isPanelVisible} zIndex={zIndex}>
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
              {t("ChooseFolderByUser")}
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
              />

              {selectedFolder && (
                <FilesListBody
                  filesList={filesList}
                  onSelectFile={onSelectFile}
                  hasNextPage={hasNextPage}
                  isNextPageLoading={isNextPageLoading}
                  loadNextPage={loadNextPage}
                  selectedFolder={selectedFolder}
                  displayType={DISPLAY_TYPE}
                  loadingText={loadingText}
                />
              )}
            </div>
          </div>
        </StyledSelectFilePanel>
      </Aside>
    </StyledAsidePanel>
  );
};
export default SelectFileDialogAsideView;
