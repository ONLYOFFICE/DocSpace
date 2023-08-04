import React, { useState } from "react";
import styled from "styled-components";
import Button from "@docspace/components/button";
import EmailInput from "@docspace/components/email-input";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import Textarea from "@docspace/components/textarea";
import FieldContainer from "@docspace/components/field-container";
import { smallTablet } from "@docspace/components/utils/device";
import { sendRecoverRequest } from "@docspace/common/api/settings";
import toastr from "@docspace/components/toast/toastr";
import { useTranslation } from "react-i18next";

interface IRecoverAccessModalDialogProps {
  visible: boolean;
  onClose: () => void;
  textBody: string;
  emailPlaceholderText: string;
  id?: string;
}

const ModalDialogContainer = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    @media (max-width: 1024px) {
      width: 90%;
    }
  }

  .recover-button-dialog {
    @media ${smallTablet} {
      width: 100%;
    }
  }

  .text-body {
    margin-bottom: 16px;
  }

  .textarea {
    margin-bottom: 0;
  }
`;

const RecoverAccessModalDialog: React.FC<IRecoverAccessModalDialogProps> = ({
  visible,
  onClose,
  textBody,
  emailPlaceholderText,
  id,
}) => {
  const [loading, setLoading] = useState(false);

  const [email, setEmail] = useState("");
  const [emailErr, setEmailErr] = useState(false);
  const [emailErrorMessage, setEmailErrorMessage] = useState("");

  const [description, setDescription] = useState("");
  const [descErr, setDescErr] = useState(false);

  const [isShowError, setIsShowError] = useState(false);

  const { t } = useTranslation(["Login", "Common"]);

  const onRecoverModalClose = () => {
    setEmail("");
    setEmailErr(false);
    setDescription("");
    setDescErr(false);
    setIsShowError(false);
    onClose && onClose();
  };

  const onChangeEmail = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(e.currentTarget.value);
    setEmailErr(false);
    setIsShowError(false);
  };

  const onValidateEmail = (res) => {
    setEmailErr(!res.isValid);
    setEmailErrorMessage(res.errors[0]);
  };

  const onBlurEmail = () => {
    setIsShowError(true);
  };

  const onChangeDescription = (e: React.ChangeEvent<HTMLInputElement>) => {
    setDescription(e.currentTarget.value);
    setDescErr(false);
  };

  const onSendRecoverRequest = () => {
    if (!email.trim() || emailErr) {
      setIsShowError(true);
      return setEmailErr(true);
    }
    if (!description.trim()) {
      return setDescErr(true);
    }

    setLoading(true);
    sendRecoverRequest(email, description)
      .then((res: string) => {
        setLoading(false);
        toastr.success(res);
      })
      .catch((error) => {
        setLoading(false);
        toastr.error(error);
      })
      .finally(onRecoverModalClose);
  };

  return (
    <ModalDialogContainer
      visible={visible}
      onClose={onRecoverModalClose}
      isLarge
      id={id}
    >
      <ModalDialog.Header>
        <Text isBold={true} fontSize="21px">
          {t("Common:RecoverTitle")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text
          key="text-body"
          className="text-body"
          isBold={false}
          fontSize="13px"
          noSelect
        >
          {textBody}
        </Text>
        <FieldContainer
          key="e-mail"
          isVertical={true}
          labelVisible={false}
          hasError={isShowError && emailErr}
          errorMessage={
            emailErrorMessage
              ? t(`Common:${emailErrorMessage}`)
              : t("Common:RequiredField")
          }
        >
          <EmailInput
            hasError={isShowError && emailErr}
            placeholder={emailPlaceholderText}
            isAutoFocussed={true}
            id="recover-access-modal_email"
            name="e-mail"
            type="email"
            size="base"
            scale={true}
            tabIndex={3}
            isDisabled={loading}
            value={email}
            onChange={onChangeEmail}
            onValidateInput={onValidateEmail}
            onBlur={onBlurEmail}
          />
        </FieldContainer>
        <FieldContainer
          className="textarea"
          key="text-description"
          isVertical={true}
          hasError={descErr}
          labelVisible={false}
          errorMessage={t("Common:RequiredField")}
        >
          <Textarea
            id="recover-access-modal_description"
            heightScale={false}
            hasError={descErr}
            placeholder={t("Common:RecoverDescribeYourProblemPlaceholder")}
            tabIndex={3}
            value={description}
            onChange={onChangeDescription}
            isDisabled={loading}
            heightTextArea={70}
          />
        </FieldContainer>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="recover-access-modal_send"
          className="recover-button-dialog"
          key="RecoverySendBtn"
          label={loading ? t("Common:Sending") : t("Common:SendButton")}
          size="normal"
          primary={true}
          onClick={onSendRecoverRequest}
          isLoading={loading}
          isDisabled={loading}
          tabIndex={3}
        />
        <Button
          id="recover-access-modal_cancel"
          className="recover-button-dialog"
          key="SendBtn-recover-close"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onRecoverModalClose}
          isLoading={loading}
          isDisabled={loading}
          tabIndex={3}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default RecoverAccessModalDialog;
