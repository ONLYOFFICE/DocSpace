import React from "react";
import DynamicComponent from "./DynamicComponent";
import { STUDIO_REMOTE_ENTRY_URL, STUDIO_SCOPE } from "../helpers/constants";
import Text from "@docspace/components/text";
import TextInput from "@docspace/components/text-input";
import Checkbox from "@docspace/components/checkbox";
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
          scope: STUDIO_SCOPE,
          url: STUDIO_REMOTE_ENTRY_URL,
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
