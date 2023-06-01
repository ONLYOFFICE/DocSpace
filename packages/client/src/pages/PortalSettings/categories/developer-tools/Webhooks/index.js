import Button from "@docspace/components/button";
import React, { useState, useEffect } from "react";
import WebhookDialog from "./sub-components/WebhookDialog";
import WebhookInfo from "./sub-components/WebhookInfo";
import WebhooksTable from "./sub-components/WebhooksTable";

import { inject, observer } from "mobx-react";

import styled from "styled-components";
import { WebhookConfigsLoader } from "./sub-components/Loaders";
import { Base } from "@docspace/components/themes";

import { isMobile } from "@docspace/components/utils/device";

import { useTranslation } from "react-i18next";

const MainWrapper = styled.div`
  width: 100%;
  margin-top: 5px;

  .toggleButton {
    display: flex;
    align-items: center;
  }
`;

const ButtonSeating = styled.div`
  position: fixed;
  z-index: 2;
  width: 100vw;
  height: 73px;
  bottom: 0;
  left: 0;
  background-color: ${(props) => props.theme.backgroundColor};

  display: flex;
  justify-content: center;
  align-items: center;
`;

ButtonSeating.defaultProps = { theme: Base };

const StyledCreateButton = styled(Button)`
  width: calc(100% - 32px);
`;

const Webhooks = (props) => {
  const { state, loadWebhooks, addWebhook, isWebhookExist, isWebhooksEmpty, setDocumentTitle } =
    props;

  const { t, ready } = useTranslation(["Webhooks", "Common"]);

  setDocumentTitle(t("Webhooks"));

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

  return state === "pending" || !ready ? (
    <WebhookConfigsLoader />
  ) : state === "success" ? (
    <MainWrapper>
      <WebhookInfo />
      {isMobile() ? (
        <ButtonSeating>
          <StyledCreateButton
            label={t("CreateWebhook")}
            primary
            size={"normal"}
            onClick={openModal}
          />
        </ButtonSeating>
      ) : (
        <Button label={t("CreateWebhook")} primary size={"small"} onClick={openModal} />
      )}

      {!isWebhooksEmpty && <WebhooksTable />}
      <WebhookDialog
        visible={isModalOpen}
        onClose={closeModal}
        header={t("CreateWebhook")}
        onSubmit={onCreateWebhook}
      />
    </MainWrapper>
  ) : state === "error" ? (
    t("Common:Error")
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
