import React, { useState, useEffect, useRef } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import { LabledInput } from "../LabledInput";
import styled from "styled-components";
import { Hint } from "../../styled-components";
import { SSLVerification } from "../SSLVerification";
import { SecretKeyInput } from "../SecretKeyInput";
import Link from "@docspace/components/link";

import { inject, observer } from "mobx-react";

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

const InfoHint = styled(Hint)`
  position: absolute;
  z-index: 2;

  width: 320px;
`;

const WebhookDialog = (props) => {
  const { visible, onClose, header, isSettingsModal, onSubmit, webhook, passwordSettings } = props;

  const [isResetVisible, setIsResetVisible] = useState(isSettingsModal);
  const [isPasswordValid, setIsPasswordValid] = useState(false);
  const [isFormBlank, setIsFormBlank] = useState(true);
  const [webhookInfo, setWebhookInfo] = useState({
    id: 0,
    title: webhook ? webhook.title : "",
    url: webhook ? webhook.url : "",
    secretKey: webhook ? webhook.secretKey : "",
    isEnabled: webhook ? webhook.isEnabled : true,
  });

  const submitButtonRef = useRef(null);
  const secretKeyInputRef = useRef(null);
  const generatePassword = () => {
    secretKeyInputRef.current.onGeneratePassword();
  };

  const onModalClose = () => {
    onClose();
    isSettingsModal && setIsResetVisible(true);
  };

  const hideReset = () => setIsResetVisible(false);

  const onInputChange = (e) => {
    setWebhookInfo((prevWebhookInfo) => ({ ...prevWebhookInfo, [e.target.name]: e.target.value }));
  };

  const handleSubmitClick = () => {
    if (isPasswordValid || isResetVisible) {
      submitButtonRef.current.click();
    }
  };

  const onFormSubmit = (e) => {
    e.preventDefault();
    onSubmit(webhookInfo);
    setWebhookInfo({
      id: 0,
      title: webhook ? webhook.title : "",
      url: webhook ? webhook.url : "",
      secretKey: webhook ? webhook.secretKey : "",
      isEnabled: webhook ? webhook.isEnabled : true,
    });
    setIsFormBlank(true);
    onModalClose();
  };

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    return () => window.removeEventListener("keyup", onKeyPress);
  }, []);

  const onKeyPress = (e) => (e.key === "Esc" || e.key === "Escape") && onModalClose();

  return (
    <ModalDialog withFooterBorder visible={visible} onClose={onModalClose} displayType="aside">
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <form onSubmit={onFormSubmit}>
          <Hint>This webhook will be assigned to all events in DocSpace</Hint>
          <LabledInput
            label="Webhook name"
            placeholder="Enter webhook name"
            name="title"
            value={webhookInfo.title}
            onChange={onInputChange}
            required
          />
          <LabledInput
            label="Payload URL"
            placeholder="Enter URL"
            name="url"
            value={webhookInfo.url}
            onChange={onInputChange}
            required
          />
          <SSLVerification />
          <SecretKeyInput
            isResetVisible={isResetVisible}
            name="secretKey"
            value={webhookInfo.secretKey}
            onChange={onInputChange}
            passwordSettings={passwordSettings}
            isPasswordValid={isPasswordValid}
            setIsPasswordValid={setIsPasswordValid}
            isFormBlank={isFormBlank}
            setIsFormBlank={setIsFormBlank}
            secretKeyInputRef={secretKeyInputRef}
            generatePassword={generatePassword}
          />
          {isResetVisible ? (
            <InfoHint hidden={!isResetVisible}>
              You cannot retrieve your secret key again once it has been saved. If you've lost or
              forgotten this secret key, you can reset it, but all integrations using this secret
              will need to be updated. <br />
              <Link
                type="action"
                fontWeight={600}
                isHovered={true}
                onClick={hideReset}
                style={{ marginTop: "6px", display: "inline-block" }}>
                Reset key
              </Link>
            </InfoHint>
          ) : (
            <></>
          )}

          <button type="submit" ref={submitButtonRef} hidden></button>
        </form>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer>
          <Button
            label={isSettingsModal ? "Save" : "Create"}
            size="normal"
            primary={true}
            onClick={handleSubmitClick}
          />
          <Button label="Cancel" size="normal" onClick={onModalClose} />
        </Footer>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { passwordSettings } = settingsStore;

  return {
    passwordSettings,
  };
})(observer(WebhookDialog));
