import React, { useState } from "react";
import PropTypes from "prop-types";
import { Button, TextInput, Text, ModalDialog, FieldContainer } from "asc-web-components";
import ModalDialogContainer from "./modal-dialog-container";

const domains = ['mail.ru', 'gmail.com', 'yandex.ru'];

const domainList = domains
  .map((domain, i) =>
    <span key={i}>
      <b>
        {domain}{i === domains.length - 1 ? "." : ", "}
      </b>
    </span>
  );

const RegisterModalDialog = ({ t, visible, onRegisterModalClose }) => {

  const [email, setEmail] = useState("");
  const [emailErr, setEmailErr] = useState(false);

  const onSendRegisterRequest = () => {
    if (!email.trim()) {
      setEmailErr(true);
    }
    else {
      alert(`Registration request sent. E-mail: ${email}`);
    }
  }

  const onChangeEmail = (e) => {
    setEmail(e.currentTarget.value);
    setEmailErr(false);
  }

  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        bodyPadding="16px 0 0 0"
        headerContent={
          <Text isBold={true} fontSize='21px'>
            {t("RegisterTitle")}
          </Text>
        }
        bodyContent={[
          <Text
            key="text-body"
            isBold={false}
            fontSize='13px'
          >
            {t("RegisterTextBodyBeforeDomainsList")} {domainList} {t("RegisterTextBodyAfterDomainsList")}
          </Text>,
          <FieldContainer
            key="e-mail"
            isVertical={true}
            hasError={emailErr}
            labelVisible={false}
            errorMessage={t("RequiredFieldMessage")}>
            <TextInput
              hasError={emailErr}
              placeholder={t("RegisterPlaceholder")}
              isAutoFocussed={true}
              id="e-mail"
              name="e-mail"
              type="text"
              size="base"
              scale={true}
              tabIndex={3}
              //isDisabled={isLoading}
              value={email}
              onChange={onChangeEmail}
            />
          </FieldContainer>
        ]}
        footerContent={[
          <Button
            className="modal-dialog-button"
            key="SendBtn"
            label={t("RegisterSendButton")}
            size="big"
            scale={false}
            primary={true}
            onClick={onSendRegisterRequest}
            //isLoading={isLoading}
            //isDisabled={isLoading}
            tabIndex={3}
          />
        ]}
        onClose={onRegisterModalClose}
      />
    </ModalDialogContainer>
  );
}


RegisterModalDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onChangeEmail: PropTypes.func.isRequired,
  onSendRegisterRequest: PropTypes.func.isRequired,
  onRegisterModalClose: PropTypes.func.isRequired,
  t: PropTypes.func.isRequired
};

export default RegisterModalDialog;
