import React, { useState } from "react";
import DynamicComponent from "../components/dynamic";
import { getPresignedUri } from "@appserver/common/api/files";
import { FILES_REMOTE_ENTRY_URL, FILES_SCOPE } from "./constants";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import { StyledSelectFolder } from "../StyledEditor";
import { useTranslation } from "react-i18next";

const insertImageAction = "imageFileType";
const mailMergeAction = "mailMergeFileType";
const compareFilesAction = "documentsFileType";

const withDialogs = (WrappedComponent) => {
  return (props) => {
    const [isVisible, setIsVisible] = useState(false);
    const [filesType, setFilesType] = useState("");
    const [isFileDialogVisible, setIsFileDialogVisible] = useState(false);
    const [typeInsertImageAction, setTypeInsertImageAction] = useState();
    const [isFolderDialogVisible, setIsFolderDialogVisible] = useState(false);
    const [titleSelectorFolder, setTitleSelectorFolder] = useState("");
    const [urlSelectorFolder, setUrlSelectorFolder] = useState("");
    const [extension, setExtension] = useState();
    const [openNewTab, setNewOpenTab] = useState(false);

    const { t } = useTranslation(["Editor", "Common"]);

    const { fileInfo, fileId, mfReady } = props;

    const onSDKRequestSharingSettings = () => {
      setIsVisible(true);
    };

    const onCancel = () => {
      setIsVisible(false);
    };

    const loadUsersRightsList = () => {
      window.SharingDialog.getSharingSettings(fileId).then(
        (sharingSettings) => {
          window.docEditor.setSharingSettings({
            sharingSettings,
          });
        }
      );
    };

    const onCloseFileDialog = () => {
      setIsFileDialogVisible(false);
    };

    const onSDKRequestCompareFile = () => {
      setFilesType(compareFilesAction);
      setIsFileDialogVisible(true);
    };

    const onSDKRequestMailMergeRecipients = () => {
      setFilesType(mailMergeAction);
      setIsFileDialogVisible(true);
    };

    const onSDKRequestInsertImage = (event) => {
      console.log("onSDKRequestInsertImage", event.data, insertImageAction);
      setTypeInsertImageAction(event.data);
      setFilesType(insertImageAction);
      setIsFileDialogVisible(true);
    };

    const insertImage = (link) => {
      const token = link.token;

      window.docEditor.insertImage({
        ...typeInsertImageAction,
        fileType: link.filetype,
        ...(token && { token }),
        url: link.url,
      });
    };

    const mailMerge = (link) => {
      const token = link.token;

      window.docEditor.setMailMergeRecipients({
        fileType: link.filetype,
        ...(token && { token }),
        url: link.url,
      });
    };

    const compareFiles = (link) => {
      const token = link.token;

      window.docEditor.setRevisedFile({
        fileType: link.filetype,
        ...(token && { token }),
        url: link.url,
      });
    };

    const insertImageActionProps = {
      isImageOnly: true,
    };

    const mailMergeActionProps = {
      isTablesOnly: true,
      searchParam: ".xlsx",
    };
    const compareFilesActionProps = {
      isDocumentsOnly: true,
    };

    const fileTypeDetection = () => {
      // console.log(
      //   "------",
      //   insertImageAction,
      //   filesType,
      //   insertImageActionProps,
      //   "--------"
      // );
      if (filesType === insertImageAction) {
        // console.log(
        //   "filesType === insertImageAction",
        //   filesType === insertImageAction
        // );
        return insertImageActionProps;
      }
      if (filesType === mailMergeAction) {
        // console.log(
        //   "filesType === mailMergeAction",
        //   filesType === mailMergeAction
        // );
        return mailMergeActionProps;
      }
      if (filesType === compareFilesAction) {
        // console.log(
        //   "filesType === compareFilesAction",
        //   filesType === compareFilesAction
        // );
        return compareFilesActionProps;
      }
    };

    const onSelectFile = async (file) => {
      try {
        const link = await getPresignedUri(file.id);

        if (filesType === insertImageAction) insertImage(link);
        if (filesType === mailMergeAction) mailMerge(link);
        if (filesType === compareFilesAction) compareFiles(link);
      } catch (e) {
        console.error(e);
      }
    };

    const getFileTypeTranslation = () => {
      switch (filesType) {
        case mailMergeAction:
          return t("MailMergeFileType");
        case insertImageAction:
          return t("ImageFileType");
        case compareFilesAction:
          return t("DocumentsFileType");
      }
    };

    const selectFilesListTitle = () => {
      const type = getFileTypeTranslation();
      return filesType === mailMergeAction
        ? type
        : t("SelectFilesType", { fileType: type });
    };

    const onSDKRequestSaveAs = (event) => {
      setTitleSelectorFolder(event.data.title);
      setUrlSelectorFolder(event.data.url);
      setExtension(event.data.title.split(".").pop());

      setIsFolderDialogVisible(true);
    };

    const onCloseFolderDialog = () => {
      setIsFolderDialogVisible(false);
      setNewOpenTab(false);
    };

    const getSavingInfo = async (title, folderId) => {
      const savingInfo = await window.filesUtils.SaveAs(
        title,
        urlSelectorFolder,
        folderId,
        openNewTab
      );

      if (savingInfo) {
        const convertedInfo = savingInfo.split(": ").pop();
        docEditor.showMessage(convertedInfo);
      }
    };

    const onClickSaveSelectFolder = (e, folderId) => {
      const currentExst = titleSelectorFolder.split(".").pop();

      const title =
        currentExst !== extension
          ? titleSelectorFolder.concat(`.${extension}`)
          : titleSelectorFolder;

      if (openNewTab) {
        window.filesUtils.SaveAs(
          title,
          urlSelectorFolder,
          folderId,
          openNewTab
        );
      } else {
        getSavingInfo(title, folderId);
      }
    };

    const onClickCheckbox = () => {
      setNewOpenTab(!openNewTab);
    };

    const onChangeInput = (e) => {
      setTitleSelectorFolder(e.target.value);
    };

    const sharingDialog = mfReady && (
      <DynamicComponent
        className="dynamic-sharing-dialog"
        system={{
          scope: FILES_SCOPE,
          url: FILES_REMOTE_ENTRY_URL,
          module: "./SharingDialog",
          name: "SharingDialog",
        }}
        isVisible={isVisible}
        sharingObject={fileInfo}
        onCancel={onCancel}
        onSuccess={loadUsersRightsList}
      />
    );

    const typeFilter = fileTypeDetection();
    // console.log(typeFilter, "-----------------");

    const selectFileDialog = mfReady && (
      <DynamicComponent
        system={{
          scope: FILES_SCOPE,
          url: FILES_REMOTE_ENTRY_URL,
          module: "./SelectFileDialog",
        }}
        resetTreeFolders
        foldersType="exceptPrivacyTrashFolders"
        isPanelVisible={isFileDialogVisible}
        onSelectFile={onSelectFile}
        onClose={onCloseFileDialog}
        {...typeFilter}
        titleFilesList={selectFilesListTitle()}
        headerName={t("SelectFileTitle")}
      />
    );

    const selectFolderDialog = mfReady && (
      <DynamicComponent
        resetTreeFolders
        showButtons
        isSetFolderImmediately
        asideHeightContent="calc(100% - 50px)"
        foldersType="exceptSortedByTags"
        system={{
          scope: FILES_SCOPE,
          url: FILES_REMOTE_ENTRY_URL,
          module: "./SelectFolderDialog",
        }}
        isPanelVisible={isFolderDialogVisible}
        onClose={onCloseFolderDialog}
        onSave={onClickSaveSelectFolder}
        headerName={t("FolderForSave")}
        header={
          <StyledSelectFolder>
            <Text className="editor-select-folder_text">{t("FileName")}</Text>
            <TextInput
              className="editor-select-folder_text-input"
              scale
              onChange={onChangeInput}
              value={titleSelectorFolder}
            />
          </StyledSelectFolder>
        }
        {...(extension !== "fb2" && {
          footer: (
            <StyledSelectFolder>
              <Checkbox
                className="editor-select-folder_checkbox"
                label={t("OpenSavedDocument")}
                onChange={onClickCheckbox}
                isChecked={openNewTab}
              />
            </StyledSelectFolder>
          ),
        })}
      />
    );

    return (
      <WrappedComponent
        {...props}
        sharingDialog={sharingDialog}
        onSDKRequestSharingSettings={onSDKRequestSharingSettings}
        loadUsersRightsList={loadUsersRightsList}
        isVisible={isVisible}
        selectFileDialog={selectFileDialog}
        onSDKRequestInsertImage={onSDKRequestInsertImage}
        onSDKRequestMailMergeRecipients={onSDKRequestMailMergeRecipients}
        onSDKRequestCompareFile={onSDKRequestCompareFile}
        selectFolderDialog={selectFolderDialog}
        onSDKRequestSaveAs={onSDKRequestSaveAs}
      />
    );
  };
};

export default withDialogs;
