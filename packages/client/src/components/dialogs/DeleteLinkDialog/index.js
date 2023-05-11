import React, { useState, useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import { StyledDeleteLinkDialog } from "./StyledDeleteLinkDialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";

const DeleteLinkDialogComponent = (props) => {
  const {
    t,
    link,
    visible,
    setIsVisible,
    tReady,
    roomId,
    setExternalLinks,
    editExternalLink,
  } = props;
  const { id: linkId, title } = link.sharedTo;

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    document.addEventListener("keyup", onKeyUp, false);

    return () => {
      document.removeEventListener("keyup", onKeyUp, false);
    };
  }, []);

  const onKeyUp = (e) => {
    if (e.keyCode === 27) onClose();
    if (e.keyCode === 13 || e.which === 13) onDelete();
  };

  const onClose = () => {
    setIsVisible(false);
  };

  const onDelete = () => {
    setIsLoading(true);
    editExternalLink({ roomId, linkId, title, access: 0 })
      .then((res) => {
        setExternalLinks(res);
      })
      .catch((err) => toastr.error(err?.message))
      .finally(() => {
        setIsLoading(false);
        onClose();
      });
  };

  return (
    <StyledDeleteLinkDialog
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("Files:DeleteLink")}</ModalDialog.Header>
      <ModalDialog.Body>
        <div className="modal-dialog-content-body">
          <Text noSelect>{t("Files:DeleteLinkNote")}</Text>
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="delete-file-modal_submit"
          key="OkButton"
          label={t("Common:Delete")}
          size="normal"
          primary
          scale
          onClick={onDelete}
          isDisabled={isLoading}
        />
        <Button
          id="delete-file-modal_cancel"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isDisabled={isLoading}
        />
      </ModalDialog.Footer>
    </StyledDeleteLinkDialog>
  );
};

const DeleteLinkDialog = withTranslation(["Common", "Files"])(
  DeleteLinkDialogComponent
);

export default inject(({ auth, dialogsStore, publicRoomStore }) => {
  const { selectionParentRoom } = auth.infoPanelStore;
  const {
    deleteLinkDialogVisible: visible,
    setDeleteLinkDialogVisible: setIsVisible,
    linkParams,
  } = dialogsStore;
  const { editExternalLink, setExternalLinks } = publicRoomStore;

  return {
    visible,
    setIsVisible,
    roomId: selectionParentRoom.id,
    link: linkParams.link,
    editExternalLink,
    setExternalLinks,
  };
})(observer(DeleteLinkDialog));
