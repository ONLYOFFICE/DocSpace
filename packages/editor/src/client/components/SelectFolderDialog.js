import React from "react";
import DynamicComponent from "./DynamicComponent";
import { CLIENT_REMOTE_ENTRY_URL, CLIENT_SCOPE } from "../helpers/constants";
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

  const headerProps = {
    header: (
      <StyledSelectFolder>
        <Text className="editor-select-folder_text">{t("FileName")}</Text>
        <TextInput
          className="editor-select-folder_text-input"
          scale
          onChange={onChangeInput}
          value={titleSelectorFolder}
        />
      </StyledSelectFolder>
    ),
  };

  const footerProps =
    extension !== "fb2"
      ? {
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
        }
      : {};

  return (
    (mfReady && isVisible && successAuth && (
      <DynamicComponent
        system={{
          scope: CLIENT_SCOPE,
          url: CLIENT_REMOTE_ENTRY_URL,
          module: "./SelectFolderDialog",
        }}
        needProxy
        folderId={folderId}
        isPanelVisible={isVisible}
        onClose={onCloseFolderDialog}
        foldersType="exceptSortedByTags"
        onSave={onClickSaveSelectFolder}
        isDisableButton={!titleSelectorFolder.trim()}
        {...headerProps}
        {...footerProps}
      />
    )) ||
    null
  );
};

export default SelectFolderDialog;
