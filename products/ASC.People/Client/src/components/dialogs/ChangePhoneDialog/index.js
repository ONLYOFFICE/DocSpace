import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  toastr,
  ModalDialog,
  Button,
  Text
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api } from "asc-web-common";

class ChangePhoneDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { language } = props;

    this.state = {
      isRequestRunning: false
    };

    i18n.changeLanguage(language);
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

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName,
  };
}

export default connect(mapStateToProps, {})(withRouter(ChangePhoneDialog));
