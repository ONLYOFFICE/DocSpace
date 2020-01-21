import React from "react";
import PropTypes from "prop-types";
import {
  toastr,
  ModalDialog,
  Button,
  Text
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";
const { changeLanguage } = utils;

class ChangePhoneDialogComponent extends React.Component {
  constructor() {
    super();

    this.state = {
      isRequestRunning: false
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
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t('MobilePhoneChangeTitle')}
        bodyContent={
          <Text>
            {t('MessageChangePhone')}
          </Text>
        }
        footerContent={
          <Button
            key="SendBtn"
            label={t('SendButton')}
            size="medium"
            primary={true}
            onClick={this.onChangePhone}
            isLoading={isRequestRunning}
          />
        }
      />
    );
  }
}

const ChangePhoneDialogTranslated = withTranslation()(ChangePhoneDialogComponent);

const ChangePhoneDialog = props => (
  <ChangePhoneDialogTranslated i18n={i18n} {...props} />
);

ChangePhoneDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
};

export default ChangePhoneDialog;
