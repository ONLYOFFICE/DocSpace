import React from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog, FieldContainer } from "asc-web-components";
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
  }
  .field-body {
    margin-top: 16px;
  }
`;

class SubModalDialog extends React.Component {
  render() {
    const {
      openDialog,
      isLoading,
      email,
      emailError,
      onChangeEmail,
      onSendPasswordInstructions,
      onDialogClose,
      t
    } = this.props;
    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={openDialog}
          bodyPadding="16px 0 0 0"
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
            <FieldContainer
              key="e-mail"
              isVertical={true}
              hasError={emailError}
              labelVisible={false}
              errorMessage={t("RequiredFieldMessage")}>
              <TextInput
                hasError={emailError}
                placeholder={t("PasswordRecoveryPlaceholder")}
                isAutoFocussed={true}
                id="e-mail"
                name="e-mail"
                type="text"
                size="base"
                scale={true}
                tabIndex={2}
                isDisabled={isLoading}
                value={email}
                onChange={onChangeEmail}
              />
            </FieldContainer>
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
  emailError: PropTypes.bool.isRequired,
  onChangeEmail: PropTypes.func.isRequired,
  onSendPasswordInstructions: PropTypes.func.isRequired,
  onDialogClose: PropTypes.func.isRequired,
  t: PropTypes.func.isRequired
};

export default SubModalDialog;
