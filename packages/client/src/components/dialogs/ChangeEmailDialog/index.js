import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import EmailInput from "@docspace/components/email-input";
import FieldContainer from "@docspace/components/field-container";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import { sendInstructionsToChangeEmail } from "@docspace/common/api/people";
import toastr from "@docspace/components/toast/toastr";
import { errorKeys } from "@docspace/components/utils/constants";
import { inject, observer } from "mobx-react";
class ChangeEmailDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isEmailValid: true,
      isRequestRunning: false,
      email: "",
      hasError: false,
      errorMessage: "",
      emailErrors: [],
    };
  }

  componentDidMount() {
    window.addEventListener("keyup", this.onKeyPress);
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
    const { user, updateProfileInUsers, updateProfile, fromList } = this.props;
    const { id } = user;
    const newProfile = user;
    newProfile.email = email;

    this.setState({ isRequestRunning: true }, () => {
      sendInstructionsToChangeEmail(id, email)
        .then(async (res) => {
          toastr.success(res);
          const currentProfile = await updateProfile(newProfile);
          fromList && (await updateProfileInUsers(currentProfile));
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

    if (email.trim() === "") {
      return this.setState({
        errorMessage: t("Common:IncorrectEmail"),
        hasError: true,
      });
    }

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

  onClose = () => {
    const { onClose } = this.props;
    const { isRequestRunning } = this.state;

    if (!isRequestRunning) {
      onClose();
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
        onClose={this.onClose}
        displayType="modal"
      >
        <ModalDialog.Header>{t("EmailChangeTitle")}</ModalDialog.Header>
        <ModalDialog.Body className="email-dialog-body">
          <>
            <Text className="text-body">{t("EmailActivationDescription")}</Text>
            <FieldContainer
              isVertical
              style={{ margin: "0" }}
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
                placeholder={t("EnterEmail")}
              />
            </FieldContainer>
          </>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            className="send"
            key="ChangeEmailSendBtn"
            label={t("Common:SendButton")}
            size="normal"
            scale
            primary={true}
            onClick={this.onValidateEmail}
            isLoading={isRequestRunning}
          />
          <Button
            className="cancel-button"
            key="CloseBtn"
            label={t("Common:CancelButton")}
            size="normal"
            scale
            onClick={onClose}
            isDisabled={isRequestRunning}
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

export default inject(({ auth, peopleStore }) => {
  const { updateProfile } = peopleStore.targetUserStore;

  const { updateProfileInUsers } = peopleStore.usersStore;

  return {
    updateProfile,
    updateProfileInUsers,
    isTabletView: auth.settingsStore.isTabletView,
  };
})(observer(ChangeEmailDialog));
