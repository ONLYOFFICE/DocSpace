import React from "react";
import DynamicComponent from "./DynamicComponent";
import { STUDIO_REMOTE_ENTRY_URL, STUDIO_SCOPE } from "../helpers/constants";

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
          scope: STUDIO_SCOPE,
          url: STUDIO_REMOTE_ENTRY_URL,
          module: "./SelectFileDialog",
        }}
        resetTreeFolders
        foldersType="exceptPrivacyTrashFolders"
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
