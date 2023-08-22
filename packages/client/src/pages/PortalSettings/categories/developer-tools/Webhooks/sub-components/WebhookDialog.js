import React, { useState, useEffect, useRef } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import { LabledInput } from "./LabledInput";
import styled from "styled-components";
import { Hint } from "../styled-components";
import { SSLVerification } from "./SSLVerification";
import SecretKeyInput from "./SecretKeyInput";
import { useTranslation } from "react-i18next";

const StyledWebhookForm = styled.form`
  margin-top: 7px;

  .margin-0 {
    margin: 0;
  }
`;

const Footer = styled.div`
  width: 100%;
  display: flex;

  button {
    width: 100%;
  }
  button:first-of-type {
    margin-right: 10px;
  }
`;

function validateUrl(url) {
  try {
    new URL(url);
  } catch (error) {
    return false;
  }
  return true;
}

const WebhookDialog = (props) => {
  const {
    visible,
    onClose,
    header,
    isSettingsModal,
    onSubmit,
    webhook,
    additionalId,
  } = props;

  const [isResetVisible, setIsResetVisible] = useState(isSettingsModal);

  const [isPasswordValid, setIsPasswordValid] = useState(false);
  const [isValid, setIsValid] = useState({
    name: true,
    uri: true,
    secretKey: true,
  });

  const { t } = useTranslation(["Webhooks", "Common"]);

  const [webhookInfo, setWebhookInfo] = useState({
    id: webhook ? webhook.id : 0,
    name: webhook ? webhook.name : "",
    uri: webhook ? webhook.uri : "",
    secretKey: "",
    enabled: webhook ? webhook.enabled : true,
    ssl: webhook ? webhook.ssl : true,
  });

  const [passwordInputKey, setPasswordInputKey] = useState(0);

  const submitButtonRef = useRef(null);

  const onModalClose = () => {
    onClose();
    isSettingsModal && setIsResetVisible(true);
  };

  const onInputChange = (e) => {
    if (e.target.name) {
      !isValid[e.target.name] &&
        setIsValid((prevIsValid) => ({
          ...prevIsValid,
          [e.target.name]: true,
        }));
      setWebhookInfo((prevWebhookInfo) => ({
        ...prevWebhookInfo,
        [e.target.name]: e.target.value,
      }));
    }
  };

  const handleSubmitClick = () => {
    const isUrlValid = validateUrl(webhookInfo.uri);
    setIsValid(() => ({
      uri: isUrlValid,
      name: webhookInfo.name !== "",
      secretKey: isPasswordValid,
    }));

    if (isUrlValid && (isPasswordValid || isResetVisible)) {
      submitButtonRef.current.click();
    }
  };

  const onFormSubmit = (e) => {
    e.preventDefault();
    onSubmit(webhookInfo);
    setWebhookInfo({
      id: webhook ? webhook.id : 0,
      name: "",
      uri: "",
      secretKey: "",
      enabled: true,
    });
    setPasswordInputKey((prevKey) => prevKey + 1);
    onModalClose();
  };

  const cleanUpEvent = () => window.removeEventListener("keyup", onKeyPress);

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    return cleanUpEvent;
  }, []);

  useEffect(() => {
    setWebhookInfo({
      id: webhook ? webhook.id : 0,
      name: webhook ? webhook.name : "",
      uri: webhook ? webhook.uri : "",
      secretKey: "",
      enabled: webhook ? webhook.enabled : true,
      ssl: webhook ? webhook.ssl : true,
    });
  }, [webhook]);

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onModalClose();

  return (
    <ModalDialog
      withFooterBorder
      visible={visible}
      onClose={onModalClose}
      displayType="aside"
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledWebhookForm onSubmit={onFormSubmit}>
          {!isSettingsModal && <Hint>{t("WebhookCreationHint")}</Hint>}
          <LabledInput
            id={additionalId + "-name-input"}
            label={t("WebhookName")}
            placeholder={t("EnterWebhookName")}
            name="name"
            value={webhookInfo.name}
            onChange={onInputChange}
            hasError={!isValid.name}
            className={isSettingsModal ? "margin-0" : ""}
            required
          />
          <LabledInput
            id={additionalId + "-payload-url-input"}
            label={t("PayloadUrl")}
            placeholder={t("EnterUrl")}
            name="uri"
            value={webhookInfo.uri}
            onChange={onInputChange}
            hasError={!isValid.uri}
            required
          />
          <SSLVerification value={webhookInfo.ssl} onChange={onInputChange} />
          <SecretKeyInput
            isResetVisible={isResetVisible}
            name="secretKey"
            value={webhookInfo.secretKey}
            onChange={onInputChange}
            isPasswordValid={isValid.secretKey}
            setIsPasswordValid={setIsPasswordValid}
            setIsResetVisible={setIsResetVisible}
            passwordInputKey={passwordInputKey}
            additionalId={additionalId}
          />

          <button type="submit" ref={submitButtonRef} hidden></button>
        </StyledWebhookForm>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer>
          <Button
            id={isSettingsModal ? "save-button" : "create-button"}
            label={
              isSettingsModal ? t("Common:SaveButton") : t("Common:Create")
            }
            size="normal"
            primary={true}
            onClick={handleSubmitClick}
          />
          <Button
            id="cancel-button"
            label={t("Common:CancelButton")}
            size="normal"
            onClick={onModalClose}
          />
        </Footer>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default WebhookDialog;
