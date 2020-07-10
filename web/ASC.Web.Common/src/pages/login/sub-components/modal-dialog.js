import React from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog } from "asc-web-components";
import styled from "styled-components";

const ModalDialogContainer = styled.div`
  .modal-dialog-aside-footer {
    @media(max-width: 1024px) {
      width: 90%;
    }
  }
  .login-button-dialog {
    @media(max-width: 1024px) {
      width: 100%;
    }
}`;

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
      <ModalDialogContainer>
        <ModalDialog
          visible={openDialog}
          headerContent={
            <Text isBold={true} fontSize='21px'>
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
              placeholder={t("PasswordRecoveryPlaceholder")}
              key="e-mail"
              id="e-mail"
              name="e-mail"
              type="text"
              size="base"
              scale={true}
              tabIndex={1}
              style={{ marginTop: "16px" }}
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
            />
          ]}
          onClose={onDialogClose}
        />
      </ModalDialogContainer>
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
