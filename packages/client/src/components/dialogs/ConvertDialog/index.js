import React, { useState, useEffect } from "react";
import ModalDialogContainer from "../ModalDialogContainer";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import Loaders from "@docspace/common/components/Loaders";
import { FolderType } from "@docspace/common/constants";

const ConvertDialogComponent = (props) => {
  const {
    t,
    tReady,
    visible,
    folderId,
    convertFile,
    convertItem,
    setStoreOriginal,
    hideConfirmConvert,
    storeOriginalFiles,
    convertUploadedFiles,
    setConvertDialogVisible,
    rootFoldersTitles,
    isRecentFolder,
    isFavoritesFolder,
    isShareFolder,
    setIsConvertSingleFile,
  } = props;

  let rootFolderTitle = "";
  const convertSingleFile = !!convertItem;
  const sortedFolder = isRecentFolder || isFavoritesFolder || isShareFolder;

  if (convertSingleFile && sortedFolder) {
    rootFolderTitle = isShareFolder
      ? rootFoldersTitles[FolderType.USER]?.title
      : rootFoldersTitles[convertItem.rootFolderType]?.title;
  }

  const [hideMessage, setHideMessage] = useState(false);

  const onChangeFormat = () =>
    setStoreOriginal(!storeOriginalFiles, "storeOriginalFiles");
  const onChangeMessageVisible = () => setHideMessage(!hideMessage);
  const onClose = () => setConvertDialogVisible(false);

  const onConvert = () => {
    onClose();

    if (convertSingleFile) {
      setIsConvertSingleFile(true);
      const item = {
        fileId: convertItem.id,
        toFolderId: folderId,
        action: "convert",
      };
      item.fileInfo = convertItem;
      convertFile(item, t, convertItem.isOpen);
    } else {
      hideMessage && hideConfirmConvert();
      convertUploadedFiles(t);
    }
  };

  return (
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      withFooterCheckboxes
    >
      <ModalDialog.Header>
        {convertSingleFile
          ? t("DocumentConversionTitle")
          : t("FileUploadTitle")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text>
          {convertSingleFile ? t("OpenFileMessage") : t("ConversionMessage")}
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <div className="convert_dialog_footer">
          <div className="convert_dialog_checkboxes">
            <Checkbox
              className="convert_dialog_checkbox"
              label={t("SaveOriginalFormatMessage")}
              isChecked={storeOriginalFiles}
              onChange={onChangeFormat}
            />
            {convertSingleFile && sortedFolder && (
              <div
                className={`convert_dialog_file-destination ${
                  storeOriginalFiles ? "file-destination_visible" : ""
                }`}
              >
                <Trans
                  t={t}
                  i18nKey="ConvertedFileDestination"
                  ns="ConvertDialog"
                >
                  The file copy will be created in the
                  {{ folderTitle: rootFolderTitle }} folder
                </Trans>
              </div>
            )}
            {!convertSingleFile && (
              <Checkbox
                className="convert_dialog_checkbox"
                label={t("HideMessage")}
                isChecked={hideMessage}
                onChange={onChangeMessageVisible}
              />
            )}
          </div>
          <div className="convert_dialog_buttons">
            <Button
              key="ContinueButton"
              label={t("Common:ContinueButton")}
              size="normal"
              primary
              scale
              onClick={onConvert}
            />
            <Button
              key="CloseButton"
              label={t("Common:CloseButton")}
              size="normal"
              scale
              onClick={onClose}
            />
          </div>
        </div>
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

const ConvertDialog = withTranslation(["ConvertDialog", "Common"])(
  ConvertDialogComponent
);

export default inject(
  ({
    uploadDataStore,
    treeFoldersStore,
    dialogsStore,
    settingsStore,
    selectedFolderStore,
  }) => {
    const {
      rootFoldersTitles,
      isRecentFolder,
      isFavoritesFolder,
      isShareFolder,
    } = treeFoldersStore;
    const { convertUploadedFiles, convertFile, setIsConvertSingleFile } =
      uploadDataStore;
    const { storeOriginalFiles, setStoreOriginal, hideConfirmConvert } =
      settingsStore;
    const { id: folderId } = selectedFolderStore;
    const {
      convertDialogVisible: visible,
      setConvertDialogVisible,
      convertItem,
    } = dialogsStore;

    return {
      visible,
      folderId,
      convertFile,
      convertItem,
      setStoreOriginal,
      hideConfirmConvert,
      storeOriginalFiles,
      convertUploadedFiles,
      setConvertDialogVisible,
      rootFoldersTitles,
      isRecentFolder,
      isFavoritesFolder,
      isShareFolder,
      setIsConvertSingleFile,
    };
  }
)(observer(ConvertDialog));
