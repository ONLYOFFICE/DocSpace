import React from "react";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import EmailInput from "@docspace/components/email-input";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import FieldContainer from "@docspace/components/field-container";

import ModalDialogContainer from "./modal-dialog-container";
import { TenantTrustedDomainsType } from "@docspace/common/constants";

const RegisterModalDialog = ({
  visible,
  loading,
  email,
  emailErr,
  t,
  onChangeEmail,
  onValidateEmail,
  onBlurEmail,
  onRegisterModalClose,
  onSendRegisterRequest,
  onKeyDown,
  trustedDomainsType,
  trustedDomains,
  errorText,
  isShowError,
}) => {
  const getDomains = () => {
    return trustedDomains.map((domain, i) => (
      <span key={i}>
        <b>
          {domain}
          {i === trustedDomains.length - 1 ? "." : ", "}
        </b>
      </span>
    ));
  };

  const getDomainsBlock = () => {
    return trustedDomainsType === TenantTrustedDomainsType.Custom ? (
      <>
        {t("RegisterTextBodyBeforeDomainsList")} {getDomains()}{" "}
      </>
    ) : (
      <></>
    );
  };

  return (
    <ModalDialogContainer
      displayType="modal"
      visible={visible}
      onClose={onRegisterModalClose}
      isLarge
    >
      <ModalDialog.Header>
        <Text isBold={true} fontSize="21px">
          {t("RegisterTitle")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text key="text-body" isBold={false} fontSize="13px">
          {getDomainsBlock()}
          {t("RegisterTextBodyAfterDomainsList")}
        </Text>

        <FieldContainer
          className="email-reg-field"
          key="e-mail"
          isVertical={true}
          hasError={isShowError && emailErr}
          labelVisible={false}
          errorMessage={
            errorText ? t(`Common:${errorText}`) : t("Common:RequiredField")
          }
        >
          <EmailInput
            hasError={isShowError && emailErr}
            placeholder={t("Common:RegistrationEmail")}
            isAutoFocussed={true}
            id="e-mail"
            name="e-mail"
            type="email"
            size="base"
            scale={true}
            tabIndex={1}
            isDisabled={loading}
            value={email}
            onChange={onChangeEmail}
            onValidateInput={onValidateEmail}
            onBlur={onBlurEmail}
            autoComplete="username"
            onKeyDown={onKeyDown}
          />
        </FieldContainer>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="modal-dialog-button"
          key="RegisterSendBtn"
          label={loading ? t("Common:Sending") : t("RegisterSendButton")}
          size="normal"
          scale={false}
          primary={true}
          onClick={onSendRegisterRequest}
          isLoading={loading}
          isDisabled={loading}
          tabIndex={3}
        />

        <Button
          className="modal-dialog-button"
          key="CancelBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale={false}
          primary={false}
          onClick={onRegisterModalClose}
          isLoading={loading}
          isDisabled={loading}
          tabIndex={2}
        />
      </ModalDialog.Footer>
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
  onValidateEmail: PropTypes.func.isRequired,
  onBlurEmail: PropTypes.func.isRequired,
  onSendRegisterRequest: PropTypes.func.isRequired,
  onKeyDown: PropTypes.func.isRequired,
  onRegisterModalClose: PropTypes.func.isRequired,
  trustedDomainsType: PropTypes.number,
  trustedDomains: PropTypes.array,
  errorText: PropTypes.string,
  isShowError: PropTypes.bool,
};

export default RegisterModalDialog;
