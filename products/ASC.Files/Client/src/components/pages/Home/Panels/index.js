import React from "react";
import { inject, observer } from "mobx-react";
// import {
//   getSelection,
//   getSharePanelVisible,
//   getUploadPanelVisible,
//   getUploadSelection,
// } from "../../../../store/files/selectors";
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

// function mapStateToProps(state) {
//   //   const commonSelection = getSelection(state);
//   //   const uploadSelection = getUploadSelection(state);
//   //   const uploadPanelVisible = getUploadPanelVisible(state);
//   //   const selectionItem = uploadPanelVisible ? uploadSelection : commonSelection;
//   // TODO: implement fetching selection data from this component
//   return {
//     uploadPanelVisible: getUploadPanelVisible(state),
//     sharingPanelVisible: getSharePanelVisible(state),
//     selection: selectionItem,
//   };
// }

export default inject(({ dialogsStore, uploadDataStore }) => {
  const { sharingPanelVisible } = dialogsStore;
  const { uploadPanelVisible } = uploadDataStore;

  return {
    sharingPanelVisible,
    uploadPanelVisible,
  };
})(observer(Panels));
