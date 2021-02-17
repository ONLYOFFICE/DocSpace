import React from "react";
import { IconButton } from "asc-web-components";
import { inject, observer } from "mobx-react";

const ShareButton = (props) => {
  //console.log("Share button render");
  const { uploadedFile } = props;
  const isShared = uploadedFile[0].fileInfo
    ? uploadedFile[0].fileInfo.shared
    : false;
  let color = "#A3A9AE";
  if (isShared) color = "#657077";

  const onOpenSharingPanel = () => {
    const {
      setSharingPanelVisible,
      sharingPanelVisible,
      selectUploadedFile,
      uploadedFile,
    } = props;

    const file = uploadedFile[0].fileInfo;
    selectUploadedFile([file]);
    setSharingPanelVisible(!sharingPanelVisible);
  };

  return (
    <IconButton
      iconName="CatalogSharedIcon"
      className="upload_panel-icon"
      color={color}
      isClickable
      onClick={onOpenSharingPanel}
    />
  );
};

export default inject(({ dialogsStore, uploadDataStore }, { uniqueId }) => {
  const { sharingPanelVisible, setSharingPanelVisible } = dialogsStore;
  const { selectUploadedFile, getUploadedFile } = uploadDataStore;

  const uploadedFile = getUploadedFile(uniqueId);

  return {
    sharingPanelVisible,
    uploadedFile,

    setSharingPanelVisible,
    selectUploadedFile,
  };
})(observer(ShareButton));
