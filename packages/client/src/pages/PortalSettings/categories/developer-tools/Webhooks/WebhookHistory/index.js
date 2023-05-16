import React, { useState, useEffect } from "react";
import styled from "styled-components";

import HistoryHeader from "./sub-components/HistoryHeader";
import HistoryFilterHeader from "./sub-components/HistoryFilterHeader";
import WebhookHistoryTable from "./sub-components/WebhookHistoryTable";
import { WebhookHistoryLoader } from "../sub-components/Loaders";

import { inject, observer } from "mobx-react";
import { useParams } from "react-router-dom";
import EmptyFilter from "./sub-components/EmptyFilter";

const WebhookWrapper = styled.div`
  width: 100%;
`;

const WebhookHistory = (props) => {
  const { historyItems, fetchWebhookHistory, hideTitle, showTitle, emptyCheckedIds } = props;

  const [isLoading, setIsLoading] = useState(true);
  const [statusFilters, setStatusFilters] = useState(null);

  const { id } = useParams();

  useEffect(() => {
    hideTitle();
    (async () => {
      await fetchWebhookHistory({ configId: id });
      setIsLoading(false);
    })();
    return showTitle;
  }, []);

  const applyFilters = async ({ deliveryFrom, deliveryTo, groupStatus }) => {
    emptyCheckedIds();
    const params = { configId: id, deliveryFrom, deliveryTo, groupStatus };

    await fetchWebhookHistory(params);
  };

  return (
    <WebhookWrapper>
      <HistoryHeader />
      {isLoading ? (
        <WebhookHistoryLoader />
      ) : (
        <main>
          <HistoryFilterHeader
            applyFilters={applyFilters}
            statusFilters={statusFilters}
            setStatusFilters={setStatusFilters}
          />
          {historyItems.length === 0 ? (
            <EmptyFilter setStatusFilters={setStatusFilters} applyFilters={applyFilters} />
          ) : (
            <WebhookHistoryTable />
          )}
        </main>
      )}
    </WebhookWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { historyItems, fetchWebhookHistory, hideTitle, showTitle, emptyCheckedIds } =
    webhooksStore;

  return { historyItems, fetchWebhookHistory, hideTitle, showTitle, emptyCheckedIds };
})(observer(WebhookHistory));
