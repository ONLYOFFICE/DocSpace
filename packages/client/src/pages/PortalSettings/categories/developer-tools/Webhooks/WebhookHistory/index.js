import React, { useState, useEffect } from "react";
import styled from "styled-components";

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
  const { historyItems, fetchHistoryItems, setTitleHistory, setTitleDefault, emptyCheckedIds } =
    props;

  const [isLoading, setIsLoading] = useState(false);
  const [isFetchFinished, setIsFetchFinished] = useState(false);

  const { id } = useParams();

  useEffect(() => {
    setTitleHistory();
    (async () => {
      const timer = setTimeout(() => {
        !webhookDetails.status && setIsLoading(true);
      }, 300);

      historyItems.length === 0 &&
        (await fetchHistoryItems({
          configId: id,
          count: 30,
        }));
      setIsFetchFinished(true);

      clearTimeout(timer);
      setIsLoading(false);
    })();

    return setTitleDefault;
  }, []);

  const applyFilters = async ({ deliveryFrom, deliveryTo, groupStatus }) => {
    emptyCheckedIds();
    const params = { configId: id, deliveryFrom, deliveryTo, groupStatus, count: 30 };

    await fetchHistoryItems(params);
  };

  return (
    <WebhookWrapper>
      {isLoading ? (
        <WebhookHistoryLoader />
      ) : (
        <main>
          <HistoryFilterHeader applyFilters={applyFilters} />
          {historyItems.length === 0 && isFetchFinished ? (
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
  const { historyItems, fetchHistoryItems, setTitleHistory, setTitleDefault, emptyCheckedIds } =
    webhooksStore;

  return { historyItems, fetchHistoryItems, setTitleHistory, setTitleDefault, emptyCheckedIds };
})(observer(WebhookHistory));
