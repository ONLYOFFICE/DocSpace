import React, { useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

const StyledBodyText = styled.div`
  line-height: 20px;
`;

const Footer = styled.div`
  width: 100%;
  display: flex;

  button {
    width: 100%;
  }
  button:first-of-type {
    margin-right: 10px;
  }
`;

export const DeleteWebhookDialog = ({ visible, onClose, header, handleSubmit }) => {
  const onKeyPress = (e) => (e.key === "Esc" || e.key === "Escape") && onClose();

  const { t } = useTranslation(["Webhooks", "Common", "EmptyTrashDialog"]);

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    return () => window.removeEventListener("keyup", onKeyPress);
  });

  return (
    <ModalDialog visible={visible} onClose={onClose} displayType="modal">
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledBodyText>{t("DeleteHint")}</StyledBodyText>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer>
          <Button
            label={t("EmptyTrashDialog:DeleteForeverButton")}
            size="normal"
            primary={true}
            onClick={handleSubmit}
          />
          <Button label={t("Common:CancelButton")} size="normal" onClick={onClose} />
        </Footer>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};
