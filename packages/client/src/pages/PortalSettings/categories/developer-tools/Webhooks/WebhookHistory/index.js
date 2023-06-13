import React, { useState, useEffect, useTransition, Suspense } from "react";
import moment from "moment";
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

const parseUrl = (url) => {
  const urlObj = new URL(url);
  const searchParams = urlObj.searchParams;

  const params = {};
  for (const [key, value] of searchParams) {
    params[key] = value;
  }
  params.deliveryDate =
    params.deliveryDate === "null" ? null : moment(params.deliveryDate, "YYYY-MM-DD");
  params.deliveryFrom = moment(params.deliveryFrom, "HH:mm");
  params.deliveryTo = moment(params.deliveryTo, "HH:mm");
  params.status = JSON.parse(params.status);

  return params;
};

function hasNoSearchParams(url) {
  const urlObj = new URL(url);
  return urlObj.search === "";
}

const WebhookHistory = (props) => {
  const {
    historyItems,
    fetchHistoryItems,
    emptyCheckedIds,
    clearHistoryFilters,
    setHistoryFilters,
    formatFilters,
  } = props;

  const [isFetchFinished, setIsFetchFinished] = useState(false);
  const [isPending, startTransition] = useTransition();

  const { id } = useParams();

  const fetchItems = async () => {
    if (hasNoSearchParams(window.location)) {
      await fetchHistoryItems({
        configId: id,
      });
    } else {
      const parsedParams = parseUrl(window.location);
      setHistoryFilters(parsedParams);
      await fetchHistoryItems({
        ...formatFilters(parsedParams),
        configId: id,
      });
    }
    setIsFetchFinished(true);
  };

  useEffect(() => {
    startTransition(fetchItems);
    return clearHistoryFilters;
  }, []);

  const applyFilters = async ({ deliveryFrom, deliveryTo, groupStatus }) => {
    emptyCheckedIds();
    const params = { configId: id, deliveryFrom, deliveryTo, groupStatus };

    await fetchHistoryItems(params);
  };

  return (
    <WebhookWrapper>
      <Suspense fallback={<WebhookHistoryLoader />}>
        <main>
          <HistoryFilterHeader applyFilters={applyFilters} />
          {historyItems.length === 0 && isFetchFinished ? (
            <EmptyFilter applyFilters={applyFilters} />
          ) : (
            <WebhookHistoryTable />
          )}
        </main>
      </Suspense>
    </WebhookWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const {
    historyItems,
    fetchHistoryItems,
    emptyCheckedIds,
    clearHistoryFilters,
    setHistoryFilters,
    formatFilters,
  } = webhooksStore;

  return {
    historyItems,
    fetchHistoryItems,
    emptyCheckedIds,
    clearHistoryFilters,
    setHistoryFilters,
    formatFilters,
  };
})(observer(WebhookHistory));
