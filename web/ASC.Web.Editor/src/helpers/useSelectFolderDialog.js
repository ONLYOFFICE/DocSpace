import React, { useState } from "react";
import DynamicComponent from "../components/dynamic";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import { StyledSelectFolder } from "../StyledEditor";

function useSelectFolderDialog(t) {
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
    const savingInfo = await SaveAs(
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
      SaveAs(title, urlSelectorFolder, folderId, openNewTab);
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

  const selectFolderDialog =
    typeof window !== "undefined" && isFolderDialogVisible ? (
      <DynamicComponent
        resetTreeFolders
        showButtons
        isSetFolderImmediately
        asideHeightContent="calc(100% - 50px)"
        foldersType="exceptSortedByTags"
        system={{
          scope: "files",
          url: "/products/files/remoteEntry.js",
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
