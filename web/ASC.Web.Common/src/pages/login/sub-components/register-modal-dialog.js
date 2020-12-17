import React from "react";
import PropTypes from "prop-types";
import {
  Button,
  TextInput,
  Text,
  ModalDialog,
  FieldContainer,
} from "asc-web-components";
import ModalDialogContainer from "./modal-dialog-container";

const domains = ["mail.ru", "gmail.com", "yandex.ru"];
const domainList = domains.map((domain, i) => (
  <span key={i}>
    <b>
      {domain}
      {i === domains.length - 1 ? "." : ", "}
    </b>
  </span>
));

const RegisterModalDialog = ({
  visible,
  loading,
  email,
  emailErr,
  t,
  onChangeEmail,
  onRegisterModalClose,
  onSendRegisterRequest,
}) => {
  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        bodyPadding="16px 0 0 0"
        onClose={onRegisterModalClose}
      >
        <ModalDialog.Header>
          <Text isBold={true} fontSize="21px">
            {t("RegisterTitle")}
          </Text>
        </ModalDialog.Header>
        <ModalDialog.Body>
          <Text key="text-body" isBold={false} fontSize="13px">
            {t("RegisterTextBodyBeforeDomainsList")} {domainList}{" "}
            {t("RegisterTextBodyAfterDomainsList")}
          </Text>

          <FieldContainer
            key="e-mail"
            isVertical={true}
            hasError={emailErr}
            labelVisible={false}
            errorMessage={t("RequiredFieldMessage")}
          >
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
              isDisabled={loading}
              value={email}
              onChange={onChangeEmail}
            />
          </FieldContainer>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            className="modal-dialog-button"
            key="SendBtn"
            label={
              loading ? t("RegisterProcessSending") : t("RegisterSendButton")
            }
            size="big"
            scale={false}
            primary={true}
            onClick={onSendRegisterRequest}
            isLoading={loading}
            isDisabled={loading}
            tabIndex={3}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    </ModalDialogContainer>
  );
};

RegisterModalDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  loading: PropTypes.bool.isRequired,
  email: PropTypes.string,
  emailErr: PropTypes.bool.isRequired,
  t: PropTypes.func.isRequired,
  onChangeEmail: PropTypes.func.isRequired,
  onSendRegisterRequest: PropTypes.func.isRequired,
  onRegisterModalClose: PropTypes.func.isRequired,
};

export default RegisterModalDialog;
