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
  const { historyItems, fetchHistoryItems, hideTitle, showTitle, emptyCheckedIds } = props;

  const [isLoading, setIsLoading] = useState(true);

  const { id } = useParams();

  useEffect(() => {
    hideTitle();
    (async () => {
      await fetchHistoryItems({
        configId: id,
        count: 30,
      });
      setIsLoading(false);
    })();
    return showTitle;
  }, []);

  const applyFilters = async ({ deliveryFrom, deliveryTo, groupStatus }) => {
    emptyCheckedIds();
    const params = { configId: id, deliveryFrom, deliveryTo, groupStatus, count: 30 };

    await fetchHistoryItems(params);
  };

  return (
    <WebhookWrapper>
      <HistoryHeader />
      {isLoading ? (
        <WebhookHistoryLoader />
      ) : (
        <main>
          <HistoryFilterHeader applyFilters={applyFilters} />
          {historyItems.length === 0 ? (
            <EmptyFilter applyFilters={applyFilters} />
          ) : (
            <WebhookHistoryTable />
          )}
        </main>
      )}
    </WebhookWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { historyItems, fetchHistoryItems, hideTitle, showTitle, emptyCheckedIds } = webhooksStore;

  return { historyItems, fetchHistoryItems, hideTitle, showTitle, emptyCheckedIds };
})(observer(WebhookHistory));
