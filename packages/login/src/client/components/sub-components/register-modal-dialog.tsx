import React from "react";
import Button from "@docspace/components/button";
import EmailInput from "@docspace/components/email-input";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import FieldContainer from "@docspace/components/field-container";
import { useTranslation } from "react-i18next";
import ModalDialogContainer from "./modal-dialog-container";
import { TenantTrustedDomainsType } from "@docspace/common/constants";

interface IRegisterModalDialogProps {
  visible: boolean;
  loading: boolean;
  email?: string;
  emailErr: boolean;
  onChangeEmail: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onValidateEmail: (res: IEmailValid) => void;
  onBlurEmail: () => void;
  onSendRegisterRequest: () => void;
  onKeyDown: (e: KeyboardEvent) => void;
  onRegisterModalClose: () => void;
  trustedDomainsType?: number;
  trustedDomains?: string[];
  errorText?: string;
  isShowError?: boolean;
}

const RegisterModalDialog: React.FC<IRegisterModalDialogProps> = ({
  visible,
  loading,
  email,
  emailErr,
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
  const { t } = useTranslation("Login");

  const getDomains = () => {
    if (trustedDomains)
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
            placeholder={t("RegistrationEmail")}
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
          key="SendBtn"
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

export default RegisterModalDialog;
