import React, { useState, useEffect, useRef } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import { LabledInput } from "./LabledInput";
import styled from "styled-components";
import { Hint } from "../styled-components";
import { SSLVerification } from "./SSLVerification";
import SecretKeyInput from "./SecretKeyInput";

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

const WebhookDialog = (props) => {
  const { visible, onClose, header, isSettingsModal, onSubmit, webhook } = props;

  const [isResetVisible, setIsResetVisible] = useState(isSettingsModal);
  const [isPasswordValid, setIsPasswordValid] = useState(false);

  const [webhookInfo, setWebhookInfo] = useState({
    id: webhook ? webhook.id : 0,
    name: webhook ? webhook.name : "",
    uri: webhook ? webhook.uri : "",
    secretKey: webhook ? webhook.secretKey : "",
    enabled: webhook ? webhook.enabled : true,
    ssl: webhook ? webhook.ssl : true,
  });

  const submitButtonRef = useRef(null);

  const onModalClose = () => {
    onClose();
    isSettingsModal && setIsResetVisible(true);
  };

  const onInputChange = (e) => {
    e.target.name &&
      setWebhookInfo((prevWebhookInfo) => ({
        ...prevWebhookInfo,
        [e.target.name]: e.target.value,
      }));
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
      id: webhook ? webhook.id : 0,
      name: webhook ? webhook.name : "",
      uri: webhook ? webhook.uri : "",
      secretKey: webhook ? webhook.secretKey : "",
      enabled: webhook ? webhook.enabled : true,
    });
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
            name="name"
            value={webhookInfo.name}
            onChange={onInputChange}
            required
          />
          <LabledInput
            label="Payload URL"
            placeholder="Enter URL"
            name="uri"
            value={webhookInfo.uri}
            onChange={onInputChange}
            required
          />
          <SSLVerification value={webhookInfo.ssl} onChange={onInputChange} />
          <SecretKeyInput
            isResetVisible={isResetVisible}
            name="secretKey"
            value={webhookInfo.secretKey}
            onChange={onInputChange}
            isPasswordValid={isPasswordValid}
            setIsPasswordValid={setIsPasswordValid}
            setIsResetVisible={setIsResetVisible}
          />
          

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

export default WebhookDialog;
