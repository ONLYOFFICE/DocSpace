import React, { useState, useEffect } from "react";
import styled from "styled-components";

import HistoryHeader from "./sub-components/HistoryHeader";
import HistoryFilterHeader from "./sub-components/HistoryFilterHeader";
import WebhookHistoryTable from "./sub-components/WebhookHistoryTable";
import { WebhookHistoryLoader } from "../sub-components/Loaders";

import { inject, observer } from "mobx-react";
import { useParams } from "react-router-dom";

const WebhookWrapper = styled.div`
  width: 100%;
`;

const WebhookHistory = (props) => {
  const { getWebhookHistory } = props;

  const [isLoading, setIsLoading] = useState(true);

  const { id } = useParams();

  useEffect(() => {
    (async () => {
      const webhookHistoryData = await getWebhookHistory({ configId: id });
      setHistoryWebhooks(webhookHistoryData);
      setIsLoading(false);
    })();
  }, []);

  const [historyWebhooks, setHistoryWebhooks] = useState([]);

  return (
    <WebhookWrapper>
      <HistoryHeader />
      {isLoading ? (
        <WebhookHistoryLoader />
      ) : (
        <main>
          <HistoryFilterHeader />
          <WebhookHistoryTable historyWebhooks={historyWebhooks} />
        </main>
      )}
    </WebhookWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { getWebhookHistory } = webhooksStore;

  return { getWebhookHistory };
})(observer(WebhookHistory));
