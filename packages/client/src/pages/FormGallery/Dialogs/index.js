import React from "react";
import { inject, observer } from "mobx-react";
import { SubmitToFormGallery } from "../../../components/dialogs";

const Dialogs = ({ submitToGalleryDialogVisible }) => {
  return [
    submitToGalleryDialogVisible && (
      <SubmitToFormGallery key="submit-to-form-gallery-dialog" />
    ),
  ];
};

export default inject(({ dialogsStore }) => {
  const { submitToGalleryDialogVisible } = dialogsStore;
  return {
    submitToGalleryDialogVisible,
  };
})(observer(Dialogs));
