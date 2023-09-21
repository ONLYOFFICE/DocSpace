import React from "react";
import { inject, observer } from "mobx-react";
import { SubmitToFormGallery } from "../../../components/dialogs";
import FilesSelector from "@docspace/client/src/components/FilesSelector";
import { FilesSelectorFilterTypes } from "@docspace/common/constants";
// import SelectFileDialog from "src/components/panels/SelectFileDialog";

const Dialogs = ({
  selectFileDialogVisible,
  setSelectFileDialogVisible,
  submitToGalleryDialogVisible,
}) => {
  const onCloseSelectFileDialogVisible = () =>
    setSelectFileDialogVisible(false);

  return [
    selectFileDialogVisible && (
      <FilesSelector
        key="select-file-dialog"
        filterParam={FilesSelectorFilterTypes.OFORM}
        isPanelVisible={selectFileDialogVisible}
        onSelectFile={() => {}}
        onClose={onCloseSelectFileDialogVisible}
      />
    ),
    submitToGalleryDialogVisible && (
      <SubmitToFormGallery key="oform-submit-to-form-gallery-dialog" />
    ),
  ];
};

export default inject(({ dialogsStore }) => ({
  selectFileDialogVisible: dialogsStore.selectFileDialogVisible,
  setSelectFileDialogVisible: dialogsStore.setSelectFileDialogVisible,
  submitToGalleryDialogVisible: dialogsStore.submitToGalleryDialogVisible,
}))(observer(Dialogs));
