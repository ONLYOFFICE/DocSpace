import React from "react";
import IconButton from "@appserver/components/icon-button";
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
    const { setSharingPanelVisible, selectUploadedFile, uploadedFile } = props;

    const file = uploadedFile[0].fileInfo;
    selectUploadedFile([file]);
    setSharingPanelVisible(true);
  };

  return (
    <IconButton
      iconName="/static/images/catalog.shared.react.svg"
      className="upload_panel-icon"
      color={color}
      isClickable
      onClick={onOpenSharingPanel}
    />
  );
};

export default inject(({ dialogsStore, uploadDataStore }, { uniqueId }) => {
  const { setSharingPanelVisible } = dialogsStore;
  const { selectUploadedFile, getUploadedFile } = uploadDataStore;

  const uploadedFile = getUploadedFile(uniqueId);

  return {
    uploadedFile,

    setSharingPanelVisible,
    selectUploadedFile,
  };
})(observer(ShareButton));
