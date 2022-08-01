import React from "react";
import DynamicComponent from "./DynamicComponent";
import { FILES_REMOTE_ENTRY_URL, FILES_SCOPE } from "../helpers/constants";

const SharingDialog = ({
  isVisible,
  fileInfo,
  onCancel,
  loadUsersRightsList,
  filesSettings,
  mfReady,
}) => {
  return (
    (mfReady && isVisible && (
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
        settings={filesSettings}
      />
    )) ||
    null
  );
};

export default SharingDialog;
