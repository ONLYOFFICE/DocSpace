import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import EmailInput from "@docspace/components/email-input";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import FieldContainer from "@docspace/components/field-container";
import toastr from "@docspace/components/toast/toastr";
import ModalDialogContainer from "./modal-dialog-container";
import { sendInstructionsToChangePassword } from "@docspace/common/api/people";
import { useTranslation } from "react-i18next";

const ForgotPasswordModalDialog = (props) => {
  const [email, setEmail] = useState(props.email);
  const [emailError, setEmailError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errorText, setErrorText] = useState("");
  const [isShowError, setIsShowError] = useState(false);

  const { t } = useTranslation("Login");

  const { visible, onDialogClose } = props;

  useEffect(() => {}, []);

  const onChangeEmail = (event) => {
    //console.log("onChangeEmail", event.target.value);
    setEmail(event.target.value);
    setEmailError(false);
    setIsShowError(false);
  };

  const onSendPasswordInstructions = () => {
    if (!email.trim() || emailError) {
      setEmailError(true);
      setIsShowError(true);
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
    //console.log("onKeyDown", e.key);
    if (e.key === "Enter") {
      onSendPasswordInstructions();
      e.preventDefault();
    }
  };

  const onValidateEmail = (res) => {
    setEmailError(!res.isValid);
    setErrorText(res.errors[0]);
  };

  const onBlurEmail = () => {
    setIsShowError(true);
  };

  return (
    <ModalDialogContainer
      displayType="modal"
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
          className="email-reg-field"
          key="e-mail"
          isVertical={true}
          hasError={isShowError && emailError}
          labelVisible={false}
          errorMessage={
            errorText ? t(`Common:${errorText}`) : t("Common:RequiredField")
          }
        >
          <EmailInput
            hasError={isShowError && emailError}
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
            onValidateInput={onValidateEmail}
            onBlur={onBlurEmail}
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
          size="normal"
          scale
          primary={true}
          onClick={onSendPasswordInstructions}
          isLoading={isLoading}
          isDisabled={isLoading}
          tabIndex={2}
        />
        <Button
          className="modal-dialog-button"
          key="CancelBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          primary={false}
          onClick={onDialogClose}
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
