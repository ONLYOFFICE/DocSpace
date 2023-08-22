import React, { useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

const StyledModalDialogDelete = styled(ModalDialog)`
  .button-modal {
    width: 50%;
  }
`;

const ModalDialogDelete = (props) => {
  const { visible, onClose, onClickDelete, t } = props;

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    return () => window.removeEventListener("keyup", onKeyPress);
  });

  return (
    <StyledModalDialogDelete
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>
        {t("Settings:DeleteThemeForever")}
      </ModalDialog.Header>
      <ModalDialog.Body>{t("Settings:DeleteThemeNotice")}</ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="delete button-modal"
          label={t("Common:Delete")}
          onClick={onClickDelete}
          primary
          size="normal"
        />
        <Button
          className="cancel-button button-modal"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </StyledModalDialogDelete>
  );
};

export default withTranslation(["Common", "Settings"])(ModalDialogDelete);
