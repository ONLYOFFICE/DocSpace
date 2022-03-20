import React, { useState } from "react";
import DynamicComponent from "../components/dynamic";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import { StyledSelectFolder } from "../StyledEditor";
import { FILES_REMOTE_ENTRY_URL, FILES_SCOPE } from "../helpers/constants";

function useSelectFolderDialog(t, docEditor) {
  const [isFolderDialogVisible, setIsFolderDialogVisible] = useState(false);
  const [titleSelectorFolder, setTitleSelectorFolder] = useState("");
  const [urlSelectorFolder, setUrlSelectorFolder] = useState("");
  const [extension, setExtension] = useState();
  const [openNewTab, setNewOpenTab] = useState(false);

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
      window.filesUtils.SaveAs(title, urlSelectorFolder, folderId, openNewTab);
    } else {
      getSavingInfo(title, folderId);
    }
  };

  const onClickCheckbox = () => {
    setNewOpenTab(!openNewTab);
  };

  const onChangeInput = (e) => {
    console.log(e.target.value);
    setTitleSelectorFolder(e.target.value);
  };

  const selectFolderDialog =
    typeof window !== "undefined" && isFolderDialogVisible ? (
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
    ) : null;
  return [selectFolderDialog, onSDKRequestSaveAs];
}

export default useSelectFolderDialog;
