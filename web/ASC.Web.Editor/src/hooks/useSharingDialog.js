import React, { useState } from "react";
import DynamicComponent from "../components/dynamic";
import { FILES_REMOTE_ENTRY_URL, FILES_SCOPE } from "../helpers/constants";
function useSharingDialog(fileInfo, fileId, docEditor) {
  const [isVisible, setIsVisible] = useState(false);

  const onSDKRequestSharingSettings = () => {
    setIsVisible(true);
  };

  const onCancel = () => {
    setIsVisible(false);
  };

  const loadUsersRightsList = () => {
    window.SharingDialog.getSharingSettings(fileId).then((sharingSettings) => {
      docEditor.setSharingSettings({
        sharingSettings,
      });
    });
  };

  const sharingDialog = (
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
    />
  );

  return [
    sharingDialog,
    onSDKRequestSharingSettings,
    loadUsersRightsList,
    isVisible,
  ];
}

export default useSharingDialog;
