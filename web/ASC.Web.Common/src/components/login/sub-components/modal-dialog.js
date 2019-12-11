import React from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog } from "asc-web-components";

class SubModalDialog extends React.Component {
  render() {
    const {
      openDialog,
      isLoading,
      email,
      onChangeEmail,
      onSendPasswordInstructions,
      onDialogClose,
      t
    } = this.props;
    return (
      <ModalDialog
        visible={openDialog}
        headerContent={
          <Text isBold={false} fontSize='21px'>
            {t("PasswordRecoveryTitle")}
          </Text>
        }
        bodyContent={[
          <Text
            key="text-body"
            className="text-body"
            isBold={false}
            fontSize='13px'
          >
            {t("MessageSendPasswordRecoveryInstructionsOnEmail")}
          </Text>,
          <TextInput
            key="e-mail"
            id="e-mail"
            name="e-mail"
            type="text"
            size="base"
            scale={true}
            tabIndex={1}
            isDisabled={isLoading}
            value={email}
            onChange={onChangeEmail}
          />
        ]}
        footerContent={[
          <Button
            className="login-button-dialog"
            key="SendBtn"
            label={isLoading ? t("LoadingProcessing") : t("SendButton")}
            size="big"
            scale={false}
            primary={true}
            onClick={onSendPasswordInstructions}
            isLoading={isLoading}
            isDisabled={isLoading}
            tabIndex={2}
          />,
          <Button
            key="CancelBtn"
            label={t("CancelButton")}
            size="big"
            scale={false}
            primary={false}
            onClick={onDialogClose}
            isDisabled={isLoading}
            tabIndex={3}
          />
        ]}
        onClose={onDialogClose}
      />
    );
  }
}

SubModalDialog.propTypes = {
  openDialog: PropTypes.bool.isRequired,
  isLoading: PropTypes.bool.isRequired,
  email: PropTypes.string.isRequired,
  onChangeEmail: PropTypes.func.isRequired,
  onSendPasswordInstructions: PropTypes.func.isRequired,
  onDialogClose: PropTypes.func.isRequired,
  t: PropTypes.func.isRequired
};

export default SubModalDialog;
