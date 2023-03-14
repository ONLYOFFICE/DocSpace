import Button from "@docspace/components/button";
import React, { useState, useEffect } from "react";
import { WebhookDialog } from "./sub-components/WebhookDialog";
import { WebhookInfo } from "./sub-components/WebhookInfo";
import { WebhooksTable } from "./sub-components/WebhooksTable";
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

  const [isModalOpen, setIsModalOpen] = useState(false);

  const onCreateWebhook = (webhookInfo) => {
    if (!isWebhookExist(webhookInfo)) {
      addWebhook(webhookInfo);
      closeModal();
    }
  };

  const closeModal = () => setIsModalOpen(false);
  const openModal = () => setIsModalOpen(true);

  useEffect(() => {
    setDocumentTitle("Developer Tools");
  }, []);

  return (
    <MainWrapper>
      <WebhookInfo />
      <Button label="Create webhook" primary size="small" onClick={openModal} />
      {!isWebhooksEmpty && (
        <WebhooksTable
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
        onSubmit={onCreateWebhook}
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
