import React from "react";
import PropTypes from "prop-types";
import {
  ModalDialog,
  Button,
  Text,
  EmailInput,
  FieldContainer,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import { api, utils, toastr } from "asc-web-common";

import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "ChangeEmailDialog",
  localesPath: "dialogs/ChangeEmailDialog",
});

const { sendInstructionsToChangeEmail } = api.people;
const { changeLanguage } = utils;

class ChangeEmailDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { user } = props;
    const { email } = user;

    this.state = {
      isEmailValid: true,
      isRequestRunning: false,
      email,
      hasError: false,
      errorMessage: "",
      emailErrors: [],
    };

    changeLanguage(i18n);
  }

  componentDidMount() {
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentDidUpdate(prevProps) {
    const { user } = this.props;
    const { email } = user;
    if (prevProps.user.email !== email) {
      this.setState({ email });
    }
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.onKeyPress);
  }

  onValidateEmailInput = (result) =>
    this.setState({ isEmailValid: result.isValid, emailErrors: result.errors });

  onChangeEmailInput = (e) => {
    const { hasError } = this.state;
    const email = e.target.value;
    hasError && this.setState({ hasError: false });
    this.setState({ email });
  };

  onSendEmailChangeInstructions = () => {
    const { email } = this.state;
    const { user } = this.props;
    const { id } = user;
    this.setState({ isRequestRunning: true }, () => {
      sendInstructionsToChangeEmail(id, email)
        .then((res) => {
          toastr.success(res);
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () =>
            this.props.onClose()
          );
        });
    });
  };

  onValidateEmail = () => {
    const { isEmailValid, email, emailErrors } = this.state;
    const { t, user } = this.props;
    if (isEmailValid) {
      const sameEmailError = email.toLowerCase() === user.email.toLowerCase();
      if (sameEmailError) {
        this.setState({ errorMessage: t("SameEmail"), hasError: true });
      } else {
        this.setState({ errorMessage: "", hasError: false });
        this.onSendEmailChangeInstructions();
      }
    } else {
      const translatedErrors = emailErrors.map((errorKey) => t(errorKey));
      const errorMessage = translatedErrors[0];
      this.setState({ errorMessage, hasError: true });
    }
  };

  onKeyPress = (event) => {
    const { isRequestRunning } = this.state;
    if (event.key === "Enter" && !isRequestRunning) {
      this.onValidateEmail();
    }
  };

  render() {
    console.log("ChangeEmailDialog render");
    const { t, visible, onClose } = this.props;
    const { isRequestRunning, email, errorMessage, hasError } = this.state;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("EmailChangeTitle")}</ModalDialog.Header>
          <ModalDialog.Body>
            <FieldContainer
              isVertical
              labelText={t("EnterEmail")}
              errorMessage={errorMessage}
              hasError={hasError}
            >
              <EmailInput
                id="new-email"
                scale={true}
                isAutoFocussed={true}
                value={email}
                onChange={this.onChangeEmailInput}
                onValidateInput={this.onValidateEmailInput}
                onKeyUp={this.onKeyPress}
                hasError={hasError}
              />
            </FieldContainer>
            <Text className="text-dialog">
              {t("EmailActivationDescription")}
            </Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="SendBtn"
              label={t("SendButton")}
              size="medium"
              primary={true}
              onClick={this.onValidateEmail}
              isLoading={isRequestRunning}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const ChangeEmailDialogTranslated = withTranslation()(
  ChangeEmailDialogComponent
);

const ChangeEmailDialog = (props) => (
  <ChangeEmailDialogTranslated i18n={i18n} {...props} />
);

ChangeEmailDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
};

export default ChangeEmailDialog;
