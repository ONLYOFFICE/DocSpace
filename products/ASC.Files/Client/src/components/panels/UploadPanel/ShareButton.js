import React from "react";
import { IconButton } from "asc-web-components";
import { connect } from "react-redux";
import {
  setSharingPanelVisible,
  selectUploadedFile,
} from "../../../store/files/actions";
import { getSharePanelVisible } from "../../../store/files/selectors";

const ShareButton = (props) => {
  //console.log("Share button render");
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
      color={props.color}
      isClickable
      onClick={onOpenSharingPanel}
    />
  );
};

const mapStateToProps = (state, ownProps) => {
  const isShared = ownProps.uploadedFile[0].fileInfo
    ? ownProps.uploadedFile[0].fileInfo.shared
    : false;
  let color = "#A3A9AE";
  if (isShared) color = "#657077";
  return {
    sharingPanelVisible: getSharePanelVisible(state),
    color,
  };
};

export default connect(mapStateToProps, {
  setSharingPanelVisible,
  selectUploadedFile,
})(ShareButton);
