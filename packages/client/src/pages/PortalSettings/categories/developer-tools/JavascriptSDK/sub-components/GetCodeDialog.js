import React from "react";
import copy from "copy-to-clipboard";
import ModalDialog from "@docspace/components/modal-dialog";
import Textarea from "@docspace/components/textarea";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import { useTheme } from "styled-components";

const GetCodeDialog = (props) => {
  const { t, codeBlock, visible, onClose } = props;

  const theme = useTheme();

  const onCopyClick = () => {
    copy(codeBlock);
    onClose();
    toastr.success(t("EmbedCodeSuccessfullyCopied"));
  };

  return (
    <ModalDialog visible={visible} isLarge onClose={onClose}>
      <ModalDialog.Header>{t("CopyWindowCode")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Textarea
          color={theme.textInput.placeholderColor}
          isReadOnly
          heightTextArea={180}
          value={codeBlock}
        />
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
