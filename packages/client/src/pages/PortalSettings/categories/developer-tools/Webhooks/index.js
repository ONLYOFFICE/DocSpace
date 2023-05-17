import Button from "@docspace/components/button";
import React, { useState, useEffect } from "react";
import WebhookDialog from "./sub-components/WebhookDialog";
import { WebhookInfo } from "./sub-components/WebhookInfo";
import WebhooksTable from "./sub-components/WebhooksTable";

import { inject, observer } from "mobx-react";

import styled, { css } from "styled-components";
import { WebhookConfigsLoader } from "./sub-components/Loaders";

import { isMobile } from "@docspace/components/utils/device";

const MainWrapper = styled.div`
  width: 100%;

  .toggleButton {
    display: flex;
    align-items: center;
  }
`;

const StyledCreateButton = styled(Button)`
  ${() =>
    isMobile() &&
    css`
      position: fixed;
      z-index: 2;
      width: calc(100% - 32px);
      bottom: 16px;
    `}
`;

const Webhooks = (props) => {
  const { state, loadWebhooks, addWebhook, isWebhookExist, isWebhooksEmpty, setDocumentTitle } =
    props;

  setDocumentTitle("Webhooks");

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
    loadWebhooks();
  }, []);

  return state === "pending" ? (
    <WebhookConfigsLoader />
  ) : state === "success" ? (
    <MainWrapper>
      <WebhookInfo />
      <StyledCreateButton
        label="Create webhook"
        primary
        size={isMobile() ? "normal" : "small"}
        onClick={openModal}
      />
      {!isWebhooksEmpty && <WebhooksTable />}
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

export default inject(({ webhooksStore, auth }) => {
  const { state, loadWebhooks, addWebhook, isWebhookExist, isWebhooksEmpty } = webhooksStore;
  const { setDocumentTitle } = auth;

  return {
    state,
    loadWebhooks,
    addWebhook,
    isWebhookExist,
    isWebhooksEmpty,
    setDocumentTitle,
  };
})(observer(Webhooks));