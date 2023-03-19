import Button from "@docspace/components/button";
import React, { useState, useEffect } from "react";
import WebhookDialog from "./sub-components/WebhookDialog";
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
    state,
    loadWebhooks,
    addWebhook,
    isWebhookExist,
    isWebhooksEmpty,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    retryWebhookEvent,
  } = props;

  const [isModalOpen, setIsModalOpen] = useState(false);

  const onCreateWebhook = async (webhookInfo) => {
    if (!isWebhookExist(webhookInfo)) {
      await addWebhook(webhookInfo);
      closeModal();
    }
  };

  const closeModal = () => setIsModalOpen(false);
  const openModal = () => setIsModalOpen(true);

  useEffect(() => {
    setDocumentTitle("Developer Tools");
    loadWebhooks();
  }, []);

  return state === "pending" ? (
    "loading"
  ) : state === "success" ? (
    <MainWrapper>
      <WebhookInfo />
      <Button label="Create webhook" primary size="small" onClick={openModal} />
      {!isWebhooksEmpty && (
        <WebhooksTable
          webhooks={webhooks}
          toggleEnabled={toggleEnabled}
          deleteWebhook={deleteWebhook}
          editWebhook={editWebhook}
          retryWebhookEvent={retryWebhookEvent}
        />
      )}
      <WebhookDialog
        visible={isModalOpen}
        onClose={closeModal}
        header="Create webhook"
        onSubmit={onCreateWebhook}
      />
    </MainWrapper>
  ) : state === "error" ? (
    "error"
  ) : (
    ""
  );
};

export default inject(({ webhooksStore }) => {
  const {
    webhooks,
    state,
    loadWebhooks,
    addWebhook,
    isWebhookExist,
    isWebhooksEmpty,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    retryWebhookEvent,
  } = webhooksStore;

  return {
    webhooks,
    state,
    loadWebhooks,
    addWebhook,
    isWebhookExist,
    isWebhooksEmpty,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    retryWebhookEvent,
  };
})(observer(Webhooks));
