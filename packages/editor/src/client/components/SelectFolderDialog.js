import React from "react";
import DynamicComponent from "./DynamicComponent";
import { CLIENT_REMOTE_ENTRY_URL, CLIENT_SCOPE } from "../helpers/constants";

import { useTranslation } from "react-i18next";
const SelectFolderDialog = ({
  successAuth,
  folderId,
  isVisible,
  onCloseFolderDialog,
  onClickSaveSelectFolder,
  titleSelectorFolder,

  extension,

  mfReady,
}) => {
  const { t } = useTranslation(["Editor", "Common"]);

  return (
    (mfReady && isVisible && successAuth && (
      <DynamicComponent
        system={{
          scope: CLIENT_SCOPE,
          url: CLIENT_REMOTE_ENTRY_URL,
          module: "./SelectFolderDialog",
        }}
        needProxy
        id={folderId}
        isPanelVisible={isVisible}
        onClose={onCloseFolderDialog}
        onSave={onClickSaveSelectFolder}
        isCopy={true}
        isEditorDialog={true}
        withFooterInput={true}
        withFooterCheckbox={extension !== "fb2"}
        footerInputHeader={t("FileName")}
        currentFooterInputValue={titleSelectorFolder}
        footerCheckboxLabel={t("OpenSavedDocument")}
      />
    )) ||
    null
  );
};

export default SelectFolderDialog;
