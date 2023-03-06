import React, { useState, useEffect, useRef } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import { LabledInput } from "../LabledInput";
import styled from "styled-components";
import { Hint } from "../../styled-components";
import { SSLVerification } from "../SSLVerification";
import { SecretKeyInput } from "../SecretKeyInput";

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

const DashedButton = styled.button`
  border: none;
  border-bottom: 1px dashed #333333;
  color: #333333;
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;
  padding: 0;
  background-color: transparent;
  margin-top: 6px;
  display: block;

  :hover {
    cursor: pointer;
  }
`;

const InfoHint = styled(Hint)`
  position: absolute;
  z-index: 2;

  width: 320px;
`;

export const WebhookDialog = ({
  visible,
  onClose,
  header,
  isSettingsModal,
  onSubmit,
  webhook,
  passwordSettings,
}) => {
  const onModalClose = () => {
    onClose();
    isSettingsModal && setIsResetVisible(true);
  };

  const onKeyPress = (e) => (e.key === "Esc" || e.key === "Escape") && onModalClose();
  const [isResetVisible, setIsResetVisible] = useState(isSettingsModal);

  const submitButtonRef = useRef();

  const [isPasswordValid, setIsPasswordValid] = useState(false);

  const [webhookInfo, setWebhookInfo] = useState({
    id: 0,
    title: webhook ? webhook.title : "",
    url: webhook ? webhook.url : "",
    secretKey: "",
    isEnabled: webhook ? webhook.isEnabled : true,
  });
  const onInputChange = (e) =>
    setWebhookInfo((prevWebhookInfo) => {
      prevWebhookInfo[e.target.name] = e.target.value;
      return prevWebhookInfo;
    });

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    //delete, when api will be connected
    setWebhookInfo((prevWebhookInfo) => ({
      ...prevWebhookInfo,
      id: Math.floor(Math.random() * 100) + 13,
    }));
    return () => window.removeEventListener("keyup", onKeyPress);
  }, []);

  return (
    <ModalDialog withFooterBorder visible={visible} onClose={onModalClose} displayType="aside">
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            onSubmit(webhookInfo);
            onModalClose();
          }}>
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
          />
          {isResetVisible ? (
            <InfoHint hidden={!isResetVisible}>
              You cannot retrieve your secret key again once it has been saved. If you've lost or
              forgotten this secret key, you can reset it, but all integrations using this secret
              will need to be updated.
              <DashedButton onClick={() => setIsResetVisible(false)}>Reset key</DashedButton>
            </InfoHint>
          ) : (
            <DashedButton>Generate</DashedButton>
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
            onClick={() => {
              isPasswordValid && submitButtonRef.current.click();
            }}
          />
          <Button label="Cancel" size="normal" onClick={onModalClose} />
        </Footer>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};
