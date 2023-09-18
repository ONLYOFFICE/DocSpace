import React from "react";
import copy from "copy-to-clipboard";
import ModalDialog from "@docspace/components/modal-dialog";
import Textarea from "@docspace/components/textarea";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import styled from "styled-components";

const StyledTextarea = styled(Textarea).attrs(({ theme }) => ({
  color: theme.textInput.placeholderColor,
}))`
  .Toastify {
    display: none;
  }
`;

const GetCodeDialog = (props) => {
  const { t, codeBlock, visible, onClose } = props;

  const onCopyClick = () => {
    copy(codeBlock);
    onClose();
    toastr.success(t("EmbedCodeSuccessfullyCopied"));
  };

  return (
    <ModalDialog visible={visible} isLarge onClose={onClose}>
      <ModalDialog.Header>{t("CopyWindowCode")}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledTextarea isReadOnly heightTextArea={180} value={codeBlock} />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          primary
          scale
          size="normal"
          label={t("Common:Copy")}
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
