import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import EmailInput from "@appserver/components/email-input";
import FieldContainer from "@appserver/components/field-container";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import { sendInstructionsToChangeEmail } from "@appserver/common/api/people";
import toastr from "studio/toastr";
import { errorKeys } from "@appserver/components/utils/constants";
import { inject, observer } from "mobx-react";
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
        this.setState({ errorMessage: t("Common:SameEmail"), hasError: true });
      } else {
        this.setState({ errorMessage: "", hasError: false });
        this.onSendEmailChangeInstructions();
      }
    } else {
      const translatedErrors = emailErrors.map((errorKey) => {
        switch (errorKey) {
          case errorKeys.LocalDomain:
            return t("Common:LocalDomain");
          case errorKeys.IncorrectDomain:
            return t("Common:IncorrectDomain");
          case errorKeys.DomainIpAddress:
            return t("Common:DomainIpAddress");
          case errorKeys.PunycodeDomain:
            return t("Common:PunycodeDomain");
          case errorKeys.PunycodeLocalPart:
            return t("Common:PunycodeLocalPart");
          case errorKeys.IncorrectLocalPart:
            return t("Common:IncorrectLocalPart");
          case errorKeys.SpacesInLocalPart:
            return t("Common:SpacesInLocalPart");
          case errorKeys.MaxLengthExceeded:
            return t("Common:MaxLengthExceeded");
          case errorKeys.IncorrectEmail:
            return t("Common:IncorrectEmail");
          case errorKeys.ManyEmails:
            return t("Common:ManyEmails");
          case errorKeys.EmptyEmail:
            return t("Common:EmptyEmail");
          default:
            throw new Error("Unknown translation key");
        }
      });
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
    const { t, tReady, visible, onClose, isTabletView } = this.props;
    const { isRequestRunning, email, errorMessage, hasError } = this.state;

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
        isTabletView={isTabletView}
      >
        <ModalDialog.Header>{t("EmailChangeTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <>
            <Text className="text-body">{t("EmailActivationDescription")}</Text>
            <FieldContainer
              isVertical
              //labelText={t("EnterEmail")}
              errorMessage={errorMessage}
              hasError={hasError}
              labelVisible={false}
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
          </>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="SendBtn"
            label={t("Common:SendButton")}
            size="small"
            primary={true}
            onClick={this.onValidateEmail}
            isLoading={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const ChangeEmailDialog = withTranslation(["ChangeEmailDialog", "Common"])(
  ChangeEmailDialogComponent
);

ChangeEmailDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
};

export default inject(({ auth }) => ({
  isTabletView: auth.settingsStore.isTabletView,
}))(observer(ChangeEmailDialog));
