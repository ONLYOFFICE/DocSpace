import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import { withTranslation, Trans } from "react-i18next";
import { sendInstructionsToChangePassword } from "@docspace/common/api/people";
import toastr from "@docspace/components/toast/toastr";

class ChangePasswordDialogComponent extends React.Component {
  constructor() {
    super();

    this.state = {
      isRequestRunning: false,
    };
  }
  onSendPasswordChangeInstructions = () => {
    const { email, onClose } = this.props;

    this.setState({ isRequestRunning: true }, () => {
      sendInstructionsToChangePassword(email)
        .then((res) => {
          toastr.success(res);
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => onClose());
        });
    });
  };

  keyPress = (e) => {
    if (e.keyCode === 13) {
      this.onSendPasswordChangeInstructions();
    }
  };

  componentDidMount() {
    addEventListener("keydown", this.keyPress, false);
  }

  componentWillUnmount() {
    removeEventListener("keydown", this.keyPress, false);
  }

  onClose = () => {
    const { onClose } = this.props;
    const { isRequestRunning } = this.state;

    if (!isRequestRunning) {
      onClose();
    }
  };

  render() {
    // console.log("ChangePasswordDialog render");
    const { t, tReady, visible, email, onClose, currentColorScheme } =
      this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialog
        isLoading={!tReady}
        visible={visible}
        onClose={this.onClose}
        displayType="modal"
      >
        <ModalDialog.Header>{t("PasswordChangeTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text fontSize="13px">
            <Trans
              i18nKey="MessageSendPasswordChangeInstructionsOnEmail"
              ns="ChangePasswordDialog"
              t={t}
            >
              Send the password change instructions to the
              <Link
                className="email-link"
                type="page"
                href={`mailto:${email}`}
                noHover
                color={currentColorScheme.main.accent}
                title={email}
              >
                {{ email }}
              </Link>
              email address
            </Trans>
          </Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            className="send"
            key="ChangePasswordSendBtn"
            label={t("Common:SendButton")}
            size="normal"
            scale
            primary={true}
            onClick={this.onSendPasswordChangeInstructions}
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
      </ModalDialog>
    );
  }
}

const ChangePasswordDialog = inject(({ auth }) => ({
  currentColorScheme: auth.settingsStore.currentColorScheme,
}))(
  observer(
    withTranslation(["ChangePasswordDialog", "Common"])(
      ChangePasswordDialogComponent
    )
  )
);

ChangePasswordDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  email: PropTypes.string.isRequired,
};

export default ChangePasswordDialog;
