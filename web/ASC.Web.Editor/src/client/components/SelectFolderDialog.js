import React from "react";
import DynamicComponent from "./DynamicComponent";
import { FILES_REMOTE_ENTRY_URL, FILES_SCOPE } from "../helpers/constants";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import { StyledSelectFolder } from "../components/StyledEditor";
import { useTranslation } from "react-i18next";
const SelectFolderDialog = ({
  successAuth,
  folderId,
  isVisible,
  onCloseFolderDialog,
  onClickSaveSelectFolder,
  titleSelectorFolder,
  onChangeInput,
  extension,
  onClickCheckbox,
  mfReady,
  openNewTab,
}) => {
  const { t } = useTranslation(["Editor", "Common"]);

  return (
    (mfReady && isVisible && successAuth && (
      <DynamicComponent
        system={{
          scope: FILES_SCOPE,
          url: FILES_REMOTE_ENTRY_URL,
          module: "./SelectFolderDialog",
        }}
        needProxy
        folderId={folderId}
        isPanelVisible={isVisible}
        onClose={onCloseFolderDialog}
        foldersType="exceptSortedByTags"
        onSave={onClickSaveSelectFolder}
        isDisableButton={!titleSelectorFolder.trim()}
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
    )) ||
    null
  );
};

export default SelectFolderDialog;
