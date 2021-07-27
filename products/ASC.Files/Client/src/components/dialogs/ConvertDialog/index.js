import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Loaders from "@appserver/common/components/Loaders";

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
  } = props;

  const convertSingleFile = !!convertItem;
  const [hideMessage, setHideMessage] = useState(false);

  const onChangeFormat = () =>
    setStoreOriginal(!storeOriginalFiles, "storeOriginalFiles");
  const onChangeMessageVisible = () => setHideMessage(!hideMessage);
  const onClose = () => setConvertDialogVisible(false);

  const onConvert = () => {
    onClose();

    if (convertSingleFile) {
      const item = {
        fileId: convertItem.id,
        toFolderId: folderId,
        action: "convert",
      };
      item.fileInfo = convertItem;
      convertFile(item);
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
    >
      <ModalDialog.Header>
        {convertSingleFile ? t("ConvertAndOpenTitle") : t("ConversionTitle")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <div className="convert_dialog_content">
          <img
            className="convert_dialog_image"
            src="images/convert_alert.png"
            alt="convert alert"
          />
          <div className="convert_dialog-content">
            <Text>
              {convertSingleFile
                ? t("ConversionFileMessage")
                : t("ConversionMessage")}
            </Text>
            <Checkbox
              className="convert_dialog_checkbox"
              label={t("SaveOriginalFormatMessage")}
              isChecked={storeOriginalFiles}
              onChange={onChangeFormat}
            />
            {!convertSingleFile && (
              <Checkbox
                className="convert_dialog_checkbox"
                label={t("HideMessage")}
                isChecked={hideMessage}
                onChange={onChangeMessageVisible}
              />
            )}
          </div>
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <div className="convert_dialog_footer">
          <Button
            className="convert_dialog_button-accept"
            key="ContinueButton"
            label={t("Common:ContinueButton")}
            size="medium"
            primary
            onClick={onConvert}
          />
          <Button
            className="convert_dialog_button"
            key="CloseButton"
            label={t("Common:CloseButton")}
            size="medium"
            onClick={onClose}
          />
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
    const { setTreeFolders } = treeFoldersStore;
    const { convertUploadedFiles, convertFile } = uploadDataStore;
    const {
      storeOriginalFiles,
      setStoreOriginal,
      hideConfirmConvert,
    } = settingsStore;
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
      setTreeFolders,
      setStoreOriginal,
      hideConfirmConvert,
      storeOriginalFiles,
      convertUploadedFiles,
      setConvertDialogVisible,
    };
  }
)(withRouter(observer(ConvertDialog)));
