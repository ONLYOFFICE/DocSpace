import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";

import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Text from "@appserver/components/text";
import ModalDialog from "@appserver/components/modal-dialog";
import FieldContainer from "@appserver/components/field-container";
import toastr from "@appserver/components/toast/toastr";
import ModalDialogContainer from "./modal-dialog-container";
import { sendInstructionsToChangePassword } from "@appserver/common/api/people";
import { useTranslation } from "react-i18next";

const ForgotPasswordModalDialog = (props) => {
  const [email, setEmail] = useState(props.email);
  const [emailError, setEmailError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const { t } = useTranslation("Login");

  const { visible, onDialogClose } = props;

  useEffect(() => {}, []);

  const onChangeEmail = (event) => {
    //console.log("onChangeEmail", event.target.value);
    setEmail(event.target.value);
    setEmailError(false);
  };

  const onSendPasswordInstructions = () => {
    if (!email.trim()) {
      setEmailError(true);
    } else {
      setIsLoading(true);
      sendInstructionsToChangePassword(email)
        .then(
          (res) => toastr.success(res),
          (message) => toastr.error(message)
        )
        .finally(onDialogClose());
    }
  };

  const onKeyDown = (e) => {
    console.log("onKeyDown", e.key);
    if (e.key === "Enter") {
      onSendPasswordInstructions();
      e.preventDefault();
    }
  };

  return (
    <ModalDialogContainer
      visible={visible}
      modalBodyPadding="12px 0 0 0"
      asideBodyPadding="16px 0 0 0"
      onClose={onDialogClose}
    >
      <ModalDialog.Header>
        <Text isBold={true} fontSize="21px">
          {t("PasswordRecoveryTitle")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text
          key="text-body"
          className="text-body"
          isBold={false}
          fontSize="13px"
        >
          {t("MessageSendPasswordRecoveryInstructionsOnEmail")}
        </Text>

        <FieldContainer
          key="e-mail"
          isVertical={true}
          hasError={emailError}
          labelVisible={false}
          errorMessage={t("Common:RequiredField")}
        >
          <TextInput
            hasError={emailError}
            placeholder={t("RegistrationEmail")}
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
            onKeyDown={onKeyDown}
          />
        </FieldContainer>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="modal-dialog-button"
          key="SendBtn"
          label={
            isLoading ? t("Common:LoadingProcessing") : t("Common:SendButton")
          }
          size="big"
          scale={false}
          primary={true}
          onClick={onSendPasswordInstructions}
          isLoading={isLoading}
          isDisabled={isLoading}
          tabIndex={2}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

ForgotPasswordModalDialog.propTypes = {
  email: PropTypes.string,
  visible: PropTypes.bool.isRequired,
  onDialogClose: PropTypes.func.isRequired,
};

export default ForgotPasswordModalDialog;
