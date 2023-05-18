import React, { useEffect } from "react";
import styled from "styled-components";

import ModalDialogContainer from "../ModalDialogContainer";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const StyledModal = styled(ModalDialogContainer)`
  max-width: 400px;

  .cancel-btn {
    display: inline-block;
    margin-left: 8px;
  }
`;

const UnsavedChangesDialogComponent = (props) => {
  const {
    t,
    tReady,
    visible,
    setUnsavedChangesDialog,
    setEditLinkPanelIsVisible,
  } = props;

  const onKeyPress = (e) => {
    if (e.keyCode === 13) {
      onCloseMenu();
    }
  };

  useEffect(() => {
    window.addEventListener("keydown", onKeyPress);

    return () => window.removeEventListener("keydown", onKeyPress);
  }, []);

  const onCloseMenu = () => {
    setEditLinkPanelIsVisible(false);
    onClose();
  };

  const onClose = () => {
    setUnsavedChangesDialog(false);
  };

  return (
    <StyledModal
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
      zIndex={401}
    >
      <ModalDialog.Header>
        {t("Settings:YouHaveUnsavedChanges")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text noSelect>{t("Settings:UnsavedChangesBody")}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("Settings:CloseMenu")}
          size="normal"
          primary
          onClick={onCloseMenu}
          scale
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          scale
        />
      </ModalDialog.Footer>
    </StyledModal>
  );
};

const UnsavedChangesDialog = withTranslation(["Files", "Settings"])(
  UnsavedChangesDialogComponent
);

export default inject(({ dialogsStore }) => {
  const {
    unsavedChangesDialogVisible: visible,
    setUnsavedChangesDialog,
    setEditLinkPanelIsVisible,
  } = dialogsStore;

  return {
    visible,
    setUnsavedChangesDialog,
    setEditLinkPanelIsVisible,
  };
})(observer(UnsavedChangesDialog));
