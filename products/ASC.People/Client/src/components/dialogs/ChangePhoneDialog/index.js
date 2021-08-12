import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import toastr from "studio/toastr";

class ChangePhoneDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isRequestRunning: false,
    };
  }

  // TODO: add real api request for executing change phone
  onChangePhone = () => {
    const { onClose, t } = this.props;
    this.setState({ isRequestRunning: true }, () => {
      toastr.success(t("ChangePhoneInstructionSent"));
      this.setState({ isRequestRunning: false }, () => onClose());
    });
  };

  render() {
    console.log("ChangePhoneDialog render");
    const { t, tReady, visible, onClose } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialog isLoading={!tReady} visible={visible} onClose={onClose}>
        <ModalDialog.Header>{t("MobilePhoneChangeTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("MobilePhoneEraseDescription")}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="SendBtn"
            label={t("Common:SendButton")}
            size="medium"
            primary={true}
            onClick={this.onChangePhone}
            isLoading={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

const ChangePhoneDialog = withTranslation(["ChangePhoneDialog", "Common"])(
  ChangePhoneDialogComponent
);

ChangePhoneDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
};

export default ChangePhoneDialog;
