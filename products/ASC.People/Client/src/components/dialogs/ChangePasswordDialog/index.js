import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { withTranslation, Trans } from "react-i18next";
import { sendInstructionsToChangePassword } from "@appserver/common/api/people";
import toastr from "studio/toastr";

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

  render() {
    console.log("ChangePasswordDialog render");
    const { t, tReady, visible, email, onClose, theme } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialog isLoading={!tReady} visible={visible} onClose={onClose}>
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
                type="page"
                href={`mailto:${email}`}
                noHover
                color={theme.peopleDialogs.changePassword.linkColor}
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
            key="SendBtn"
            label={t("Common:SendButton")}
            size="small"
            primary={true}
            onClick={this.onSendPasswordChangeInstructions}
            isLoading={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

const ChangePasswordDialog = inject(({ auth }) => ({
  theme: auth.settingsStore.theme,
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
