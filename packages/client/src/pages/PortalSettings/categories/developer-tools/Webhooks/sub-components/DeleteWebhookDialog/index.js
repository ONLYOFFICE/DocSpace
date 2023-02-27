import React, { useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import styled from "styled-components";

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

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    return () => window.removeEventListener("keyup", onKeyPress);
  });

  return (
    <ModalDialog withFooterBorder visible={visible} onClose={onClose} displayType="modal">
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        The webhook will be deleted permanently. <br />
        You will not be able to undo this action.
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer>
          <Button label="Delete forever" size="normal" primary={true} onClick={handleSubmit} />
          <Button label="Cancel" size="normal" onClick={onClose} />
        </Footer>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};
