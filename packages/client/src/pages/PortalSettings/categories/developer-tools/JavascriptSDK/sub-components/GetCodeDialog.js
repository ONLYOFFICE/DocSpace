import React from "react";
import copy from "copy-to-clipboard";
import ModalDialog from "@docspace/components/modal-dialog";
import Textarea from "@docspace/components/textarea";
import Button from "@docspace/components/button";

const GetCodeDialog = (props) => {
  const { t, codeBlock, visible, onClose } = props;

  const onCopyClick = () => {
    copy(codeBlock);
    onClose();
  };

  return (
    <ModalDialog visible={visible} isLarge onClose={onClose}>
      <ModalDialog.Header>{t("CopyWindowCode")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Textarea isReadOnly heightTextArea={180} placeholder={codeBlock} />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          primary
          scale
          size="normal"
          label={t("Common:CopyToClipboard")}
          onClick={onCopyClick}
        />
        <Button
          scale
          size="normal"
          label={t("Common:CloseButton")}
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default GetCodeDialog;
