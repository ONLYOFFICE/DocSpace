import React, { useState } from "react";
import DynamicComponent from "../components/dynamic";
import { getPresignedUri } from "@appserver/common/api/files";

const insertImageAction = "imageFileType";
const mailMergeAction = "mailMergeFileType";
const compareFilesAction = "documentsFileType";

function useSelectFileDialog(docEditor, t) {
  const [filesType, setFilesType] = useState("");
  const [isFileDialogVisible, setIsFileDialogVisible] = useState(false);
  const [typeInsertImageAction, setTypeInsertImageAction] = useState();

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
    setTypeInsertImageAction(event.data);
    setFilesType(insertImageAction);
    setIsFileDialogVisible(true);
  };

  const insertImage = (link) => {
    const token = link.token;

    docEditor.insertImage({
      ...typeInsertImageAction,
      fileType: link.filetype,
      ...(token && { token }),
      url: link.url,
    });
  };

  const mailMerge = (link) => {
    const token = link.token;

    docEditor.setMailMergeRecipients({
      fileType: link.filetype,
      ...(token && { token }),
      url: link.url,
    });
  };

  const compareFiles = (link) => {
    const token = link.token;

    docEditor.setRevisedFile({
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
    if (filesType === insertImageAction) {
      return insertImageActionProps;
    }
    if (filesType === mailMergeAction) {
      return mailMergeActionProps;
    }
    if (filesType === compareFilesAction) {
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

  const headerName = t("SelectFileTitle");

  const selectFileDialog =
    typeof window !== "undefined" && isFileDialogVisible ? (
      <DynamicComponent
        system={{
          scope: "files",
          url: "/products/files/remoteEntry.js",
          module: "./SelectFileDialog",
        }}
        resetTreeFolders
        foldersType="exceptPrivacyTrashFolders"
        isPanelVisible={isFileDialogVisible}
        onSelectFile={onSelectFile}
        onClose={onCloseFileDialog}
        {...fileTypeDetection()}
        titleFilesList={selectFilesListTitle()}
        headerName={headerName}
      />
    ) : null;

  return [
    selectFileDialog,
    onSDKRequestInsertImage,
    onSDKRequestMailMergeRecipients,
    onSDKRequestCompareFile,
  ];
}

export default useSelectFileDialog;
