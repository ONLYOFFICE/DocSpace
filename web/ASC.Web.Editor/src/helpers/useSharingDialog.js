import React, { useState } from "react";
import DynamicComponent from "../components/dynamic";
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
        scope: "files",
        url: "/products/files/remoteEntry.js",
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
