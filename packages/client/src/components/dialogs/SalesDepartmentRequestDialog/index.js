import React, { useState } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { I18nextProvider, useTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import TextInput from "@docspace/components/text-input";
import Text from "@docspace/components/text";
import ModalDialog from "@docspace/components/modal-dialog";
import Textarea from "@docspace/components/textarea";
import FieldContainer from "@docspace/components/field-container";
import { smallTablet } from "@docspace/components/utils/device";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";

const ModalDialogContainer = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    @media (max-width: 1024px) {
      width: 90%;
    }
  }

  .text-body {
    margin-bottom: 16px;
  }
`;

const SalesDepartmentRequestDialog = ({
  visible,
  onClose,
  sendPaymentRequest,
}) => {
  const { t, ready } = useTranslation([
    "SalesDepartmentRequestDialog",
    "Common",
  ]);

  const [isLoading, setIsLoading] = useState(false);

  const [email, setEmail] = useState("");
  const [isValidEmail, setIsValidEmail] = useState(true);

  const [description, setDescription] = useState("");
  const [isValidDescription, setIsValidDescription] = useState(true);

  const [name, setName] = useState("");
  const [isValidName, setIsValidName] = useState(true);

  const onCloseModal = () => {
    onClose && onClose();
  };

  const onChangeEmail = (e) => {
    setEmail(e.currentTarget.value);
    setIsValidEmail(true);
  };

  const onChangeDescription = (e) => {
    setDescription(e.currentTarget.value);
    setIsValidName(true);
  };
  const onChangeName = (e) => {
    setName(e.currentTarget.value);
    setIsValidDescription(true);
  };
  const onSendRequest = async () => {
    const isEmailValid = email.trim();
    const isDescriptionValid = description.trim();
    const isNameValid = name.trim();

    if (!isEmailValid || !isDescriptionValid || !isNameValid) {
      setIsValidEmail(isEmailValid);
      setIsValidName(isNameValid);
      setIsValidDescription(isDescriptionValid);
      return;
    }

    await sendPaymentRequest(email, name, description);
    onClose && onClose();
  };

  return (
    <ModalDialogContainer
      visible={visible}
      onClose={onCloseModal}
      autoMaxHeight
      isLarge
      isLoading={!ready}
    >
      <ModalDialog.Header>
        <Text isBold={true} fontSize="21px">
          {t("SalesDepartmentRequest")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text
          key="text-body"
          className="text-body"
          isBold={false}
          fontSize="13px"
        >
          {t("YouWillBeContacted")}
        </Text>

        <FieldContainer
          className="name_field"
          key="name"
          isVertical
          hasError={!isValidName}
          labelVisible={false}
          errorMessage={t("Common:RequiredField")}
        >
          <TextInput
            id="your-name"
            hasError={!isValidName}
            name="name"
            type="text"
            size="base"
            scale
            tabIndex={1}
            placeholder={t("YourName")}
            isAutoFocussed
            isDisabled={isLoading}
            value={name}
            onChange={onChangeName}
          />
        </FieldContainer>

        <FieldContainer
          className="e-mail_field"
          key="e-mail"
          isVertical
          labelVisible={false}
          hasError={!isValidEmail}
          errorMessage={t("Common:RequiredField")}
        >
          <TextInput
            hasError={!isValidEmail}
            id="registration-email"
            name="e-mail"
            type="text"
            size="base"
            scale
            tabIndex={2}
            placeholder={t("Common:RegistrationEmail")}
            isDisabled={isLoading}
            value={email}
            onChange={onChangeEmail}
          />
        </FieldContainer>

        <FieldContainer
          className="description_field"
          key="text-description"
          isVertical
          hasError={!isValidDescription}
          labelVisible={false}
          errorMessage={t("Common:RequiredField")}
        >
          <Textarea
            id="request-details"
            heightScale={false}
            hasError={!isValidDescription}
            placeholder={t("RequestDetails")}
            tabIndex={3}
            value={description}
            onChange={onChangeDescription}
            isDisabled={isLoading}
          />
        </FieldContainer>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="send-button"
          label={isLoading ? t("Common:Sending") : t("Common:SendButton")}
          size="normal"
          primary={true}
          onClick={onSendRequest}
          isLoading={isLoading}
          isDisabled={isLoading}
          tabIndex={3}
        />
        <Button
          className="cancel-button"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onCloseModal}
          isLoading={isLoading}
          isDisabled={isLoading}
          tabIndex={3}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

SalesDepartmentRequestDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default inject(({ payments }) => {
  const { sendPaymentRequest } = payments;

  return {
    sendPaymentRequest,
  };
})(observer(SalesDepartmentRequestDialog));
