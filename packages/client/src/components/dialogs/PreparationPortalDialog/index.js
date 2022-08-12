import React from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import { inject, observer } from "mobx-react";
import PreparationPortal from "SRC_DIR/pages/PreparationPortal";
import StyledPreparationPortalDialog from "./StyledPreparationPortalDialog";
const PreparationPortalDialog = (props) => {
  const { t, tReady, preparationPortalVisible, setVisible } = props;

  const onClose = () => setVisible(false);

  return (
    <ModalDialog
      isLoading={!tReady}
      visible={preparationPortalVisible}
      onClose={onClose}
      contentHeight="388px"
      contentWidth="520px"
      displayType="modal"
      withoutCloseButton
    >
      <ModalDialog.Header>{t("PortalRestoring")}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledPreparationPortalDialog>
          <PreparationPortal withoutHeader style={{ padding: "0" }} />
        </StyledPreparationPortalDialog>
      </ModalDialog.Body>
    </ModalDialog>
  );
};

const PreparationPortalDialogWrapper = inject(({ backup }, { visible }) => {
  const {
    preparationPortalDialogVisible,
    setPreparationPortalDialogVisible: setVisible,
  } = backup;

  const preparationPortalVisible = visible
    ? visible
    : preparationPortalDialogVisible;
  return {
    preparationPortalVisible,
    setVisible,
  };
})(withTranslation("PreparationPortal")(observer(PreparationPortalDialog)));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <PreparationPortalDialogWrapper {...props} />
  </I18nextProvider>
);
