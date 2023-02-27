import Button from "@docspace/components/button";
import React, { useState, useEffect } from "react";
import { WebhookDialog } from "./sub-components/WebhookDialog";
import { Info } from "./sub-components/Info";
import { WebhooksList } from "./sub-components/WebhooksList";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";

import styled from "styled-components";

const MainWrapper = styled.div`
  width: 100%;
  
  .toggleButton {
    display: flex;
    align-items: center;
  }
`;

const Webhooks = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const [webhooks, setWebhooks] = useState([
    {
      title: "Webhook 1",
      responseCode: "200",
      responseStatus: "success",
      url: "https://webhook.site/3d9f41d8-30dc-4f55-8b78-1649f4118c56",
      isEnabled: true,
    },
    {
      title: "Webhook 2",
      responseCode: "404",
      responseStatus: "error",
      url: "https://webhook.site/3d9f41d8-30dc-4f55-8b78-16",
      isEnabled: false,
    },
  ]);

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
      {webhooks.length > 0 && <WebhooksList webhooks={webhooks} setWebhooks={setWebhooks} />}
      <WebhookDialog
        visible={isModalOpen}
        onClose={closeModal}
        header="Create webhook"
        onSubmit={(webhookInfo) => {
          if (!webhooks.find((webhook) => webhook.url === webhookInfo.url)) {
            setWebhooks((prevWebhooks) => [webhookInfo, ...prevWebhooks]);
            closeModal();
          }
        }}
      />
    </MainWrapper>
  );
};

export default Webhooks;
