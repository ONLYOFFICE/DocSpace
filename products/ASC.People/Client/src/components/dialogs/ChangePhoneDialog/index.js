import React from "react";
import PropTypes from "prop-types";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils, toastr } from "asc-web-common";

import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "ChangePhoneDialog",
  localesPath: "dialogs/ChangePhoneDialog",
});

const { changeLanguage } = utils;

class ChangePhoneDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isRequestRunning: false,
    };

    changeLanguage(i18n);
  }

  // TODO: add real api request for executing change phone
  onChangePhone = () => {
    const { onClose } = this.props;
    this.setState({ isRequestRunning: true }, () => {
      toastr.success("Context action: Change phone");
      this.setState({ isRequestRunning: false }, () => onClose());
    });
  };

  render() {
    console.log("ChangePhoneDialog render");
    const { t, visible, onClose } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialog visible={visible} onClose={onClose}>
        <ModalDialog.Header>{t("MobilePhoneChangeTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("MobilePhoneEraseDescription")}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="SendBtn"
            label={t("SendButton")}
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

const ChangePhoneDialogTranslated = withTranslation()(
  ChangePhoneDialogComponent
);

const ChangePhoneDialog = (props) => (
  <ChangePhoneDialogTranslated i18n={i18n} {...props} />
);

ChangePhoneDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
};

export default ChangePhoneDialog;
