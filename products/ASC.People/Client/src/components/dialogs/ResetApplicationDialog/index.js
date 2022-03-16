import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import toastr from "studio/toastr";

class ResetApplicationDialogComponent extends React.Component {
  constructor(props) {
    super(props);
  }

  resetApp = async () => {
    const { resetTfaApp, history } = this.props;
    try {
      const res = await resetTfaApp();
      if (res) history.push(res);
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
          <Text className="text-dialog">
            {t("ResetApplicationDescription")}
          </Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="SendBtn"
            label={t("Common:ResetApplication")}
            size="small"
            primary={true}
            onClick={this.resetApp}
          />
          <Button
            key="CloseBtn"
            className="button-dialog"
            label={t("Common:CloseButton")}
            size="small"
            primary={false}
            onClick={onClose}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const ResetApplicationDialog = withTranslation([
  "ResetApplicationDialog",
  "Common",
])(ResetApplicationDialogComponent);

ResetApplicationDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  resetTfaApp: PropTypes.func.isRequired,
};

export default ResetApplicationDialog;
