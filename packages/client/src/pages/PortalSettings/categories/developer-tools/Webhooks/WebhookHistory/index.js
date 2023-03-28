import React from "react";
import styled from "styled-components";

import HistoryHeader from "./sub-components/HistoryHeader";
import HistoryFilterHeader from "./sub-components/HistoryFilterHeader";
import WebhookHistoryTable from './sub-components/WebhookHistoryTable'

const WebhookWrapper = styled.div`
  width: 100%;
`;

const WebhookHistory = () => {
  return (
    <WebhookWrapper>
      <HistoryHeader />
      <main>
        <HistoryFilterHeader />
        <WebhookHistoryTable />
      </main>
    </WebhookWrapper>
  );
};

export default WebhookHistory;
