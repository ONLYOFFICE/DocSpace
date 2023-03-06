import Button from "@docspace/components/button";
import React, { useState, useEffect } from "react";
import { WebhookDialog } from "./sub-components/WebhookDialog";
import { Info } from "./sub-components/Info";
import { WebhooksList } from "./sub-components/WebhooksList";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";

import { inject, observer } from "mobx-react";

import styled from "styled-components";

const MainWrapper = styled.div`
  width: 100%;

  .toggleButton {
    display: flex;
    align-items: center;
  }
`;

const Webhooks = (props) => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const {
    webhooks,
    addWebhook,
    isWebhookExist,
    isWebhooksEmpty,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    passwordSettings,
  } = props;

  const closeModal = () => {
    setIsModalOpen(false);
  };

  useEffect(() => {
    setDocumentTitle("Developer Tools");
  }, []);

  return (
    <MainWrapper>
      <Info />
      <Button label="Create webhook" primary size="small" onClick={() => setIsModalOpen(true)} />
      {!isWebhooksEmpty && (
        <WebhooksList
          webhooks={webhooks}
          toggleEnabled={toggleEnabled}
          deleteWebhook={deleteWebhook}
          editWebhook={editWebhook}
        />
      )}
      <WebhookDialog
        visible={isModalOpen}
        onClose={closeModal}
        header="Create webhook"
        onSubmit={(webhookInfo) => {
          if (!isWebhookExist(webhookInfo)) {
            addWebhook(webhookInfo);
            closeModal();
          }
        }}
        passwordSettings={passwordSettings}
      />
    </MainWrapper>
  );
};

export default inject(({ webhooksStore, auth }) => {
  const { settingsStore } = auth;

  const { passwordSettings } = settingsStore;

  const {
    webhooks,
    addWebhook,
    isWebhookExist,
    isWebhooksEmpty,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
  } = webhooksStore;

  return {
    webhooks,
    addWebhook,
    isWebhookExist,
    isWebhooksEmpty,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    passwordSettings,
  };
})(observer(Webhooks));
