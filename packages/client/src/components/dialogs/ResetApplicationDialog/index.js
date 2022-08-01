import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import toastr from "client/toastr";

class ResetApplicationDialogComponent extends React.Component {
  constructor(props) {
    super(props);
  }

  resetApp = async () => {
    const { resetTfaApp, id, onClose, history } = this.props;
    onClose && onClose();
    try {
      const res = await resetTfaApp(id);
      if (res) history.push(res.replace(window.location.origin, ""));
    } catch (e) {
      toastr.error(e);
    }
  };

  render() {
    //console.log("Render ResetApplicationDialog");
    const { t, tReady, visible, onClose } = this.props;

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
            key="SendBtn"
            label={t("Common:ResetApplication")}
            size="normal"
            scale
            primary={true}
            onClick={this.resetApp}
          />
          <Button
            key="CloseBtn"
            label={t("Common:CloseButton")}
            size="normal"
            scale
            primary={false}
            onClick={onClose}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const ResetApplicationDialog = withRouter(
  withTranslation(["ResetApplicationDialog", "Common"])(
    ResetApplicationDialogComponent
  )
);

ResetApplicationDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  resetTfaApp: PropTypes.func.isRequired,
  id: PropTypes.string.isRequired,
};

export default ResetApplicationDialog;
