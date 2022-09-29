import React from "react";
import DynamicComponent from "./DynamicComponent";
import { CLIENT_REMOTE_ENTRY_URL, CLIENT_SCOPE } from "../helpers/constants";

const SelectFileDialog = ({
  isVisible,
  mfReady,
  onCloseFileDialog,
  onSelectFile,
  filesListTitle,
  settings,
  successAuth,
  ...rest
}) => {
  return (
    (mfReady && isVisible && successAuth && (
      <DynamicComponent
        {...rest}
        system={{
          scope: CLIENT_SCOPE,
          url: CLIENT_REMOTE_ENTRY_URL,
          module: "./SelectFileDialog",
        }}
        resetTreeFolders
        filteredType="exceptPrivacyTrashArchiveFolders"
        isPanelVisible={isVisible}
        onClose={onCloseFileDialog}
        onSelectFile={onSelectFile}
        filesListTitle={filesListTitle}
        settings={settings}
      />
    )) ||
    null
  );
};

export default SelectFileDialog;
