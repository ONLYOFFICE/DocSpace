import React from "react";
import { inject, observer } from "mobx-react";
import { SharingPanel, UploadPanel } from "../../../panels";

const Panels = (props) => {
  const { uploadPanelVisible, sharingPanelVisible /*selection*/ } = props;
  return [
    uploadPanelVisible && <UploadPanel key="upload-panel" />,
    sharingPanelVisible && (
      <SharingPanel
        key="sharing-panel"
        // selection={selection}
        uploadPanelVisible={uploadPanelVisible}
      />
    ),
  ];
};

export default inject(({ dialogsStore, uploadDataStore }) => {
  const { sharingPanelVisible } = dialogsStore;
  const { uploadPanelVisible } = uploadDataStore;

  return {
    sharingPanelVisible,
    uploadPanelVisible,
  };
})(observer(Panels));
