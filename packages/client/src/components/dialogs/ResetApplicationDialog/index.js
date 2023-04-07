import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import ModalDialogContainer from "../ModalDialogContainer";
import toastr from "@docspace/components/toast/toastr";

const ResetApplicationDialogComponent = (props) => {
  const { t, resetTfaApp, id, onClose, tReady, visible } = props;

  const navigate = useNavigate();

  const resetApp = async () => {
    onClose && onClose();
    try {
      const res = await resetTfaApp(id);
      toastr.success(t("SuccessResetApplication"));
      if (res) navigate(res.replace(window.location.origin, ""));
    } catch (e) {
      toastr.error(e);
    }
  };

  return (
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("ResetApplicationTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("ResetApplicationDescription")}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="ResetSendBtn"
          label={t("Common:ResetApplication")}
          size="normal"
          scale
          primary={true}
          onClick={resetApp}
        />
        <Button
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          primary={false}
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

const ResetApplicationDialog = withTranslation([
  "ResetApplicationDialog",
  "Common",
])(ResetApplicationDialogComponent);

ResetApplicationDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  resetTfaApp: PropTypes.func.isRequired,
  id: PropTypes.string.isRequired,
};

export default ResetApplicationDialog;
