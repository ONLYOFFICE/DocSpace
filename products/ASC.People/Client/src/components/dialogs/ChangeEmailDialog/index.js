import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  toastr,
  ModalDialog,
  Button,
  Text,
  Label,
  EmailInput
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import ModalDialogContainer from '../ModalDialogContainer';
import { api } from "asc-web-common";
const { sendInstructionsToChangeEmail } = api.people;

class ChangeEmailDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { email, language } = props;

    this.state = {
      isEmailValid: true,
      isRequestRunning: false,
      email,
    };

    i18n.changeLanguage(language);
  }

  onValidateEmailInput = value => this.setState({ isEmailValid: value });

  onChangeEmailInput = e => this.setState({ email: e.target.value });

  onSendEmailChangeInstructions = () => {
    this.setState({ isRequestRunning: true }, function () {
      sendInstructionsToChangeEmail(this.props.id, this.state.email)
        .then((res) => {
          toastr.success(res);
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.props.onClose();
          this.setState({ isRequestRunning: false });
        });
    })

  }


  render() {
    console.log("ChangeEmailDialog render");
    const { t, visible, onClose } = this.props;
    const { isEmailValid, isRequestRunning, email } = this.state;
    const isSendButtonDisabled = !email || !email.length || !isEmailValid || email.toLowerCase() === this.props.email.toLowerCase();

    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={visible}
          onClose={onClose}
          headerContent={t('EmailChangeTitle')}
          bodyContent={
            <>
              <Label htmlFor="new-email" text={t('EnterEmail')} />
              <EmailInput
                className='input-dialog'
                id="new-email"
                scale={true}
                isAutoFocussed={true}
                value={email}
                onChange={this.onChangeEmailInput}
                onValidateInput={this.onValidateEmailInput}

              />
              <Text
                className='text-dialog'
              >
                {t('EmailActivationDescription')}
              </Text>
            </>
          }
          footerContent={
            <Button
              key="SendBtn"
              label={t('SendButton')}
              size="medium"
              primary={true}
              onClick={this.onSendEmailChangeInstructions}
              isDisabled={isSendButtonDisabled}
              isLoading={isRequestRunning}
            />
          }
        />
      </ModalDialogContainer>
    );
  }
}

const ChangeEmailDialogTranslated = withTranslation()(ChangeEmailDialogComponent);

const ChangeEmailDialog = props => (
  <ChangeEmailDialogTranslated i18n={i18n} {...props} />
);

ChangeEmailDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  email: PropTypes.string.isRequired,
  id: PropTypes.string.isRequired,
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName,
  };
}

export default connect(mapStateToProps, {})(withRouter(ChangeEmailDialog));
