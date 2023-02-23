import React, { useState, useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import { LabledInput } from "../LabledInput";
import styled from "styled-components";
import { Hint } from "../../styled-components";
import { SSLVerification } from "../SSLVerification";
import { SecretKeyInput } from "../SecretKeyInput";

const WebhookCreateDialog = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    width: 100%;
    bottom: 0 !important;
    left: 0;
    padding: 16px;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  }

  .flex {
    display: flex;
    justify-content: space-between;

    :not(:last-child) {
      padding-bottom: 20px;
    }
  }

  .relative {
    position: relative;
  }
`;

export const CreateWebhookDialog = (props) => {
  const { visible, onClose, header } = props;
  const [isHistVisible, setIsHintVisible] = useState(true);

  const setHintInvisible = () => {
    setIsHintVisible(false);
  };

  const onKeyPress = (e) => (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    return () => window.removeEventListener("keyup", onKeyPress);
  });

  return (
    <WebhookCreateDialog visible={visible} onClose={onClose} displayType="aside">
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <Hint hidden={!isHistVisible} onClick={setHintInvisible}>
          This webhook will be assigned to all events in DocSpace
        </Hint>
        <LabledInput label="Webhook name" placeholder="Enter webhook name" />
        <LabledInput label="Payload URL" placeholder="Enter URL" />
        <SSLVerification />
        <SecretKeyInput/>
      </ModalDialog.Body>

      <ModalDialog.Footer></ModalDialog.Footer>
    </WebhookCreateDialog>
  );
};
