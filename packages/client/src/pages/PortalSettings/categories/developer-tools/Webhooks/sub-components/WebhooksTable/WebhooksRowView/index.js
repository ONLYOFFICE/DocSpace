import React, { useEffect } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { isMobile } from "react-device-detect";

import RowContainer from "@docspace/components/row-container";

import { WebhookRow } from "./WebhookRow";

const StyledRowContainer = styled(RowContainer)`
  margin-top: 16px;
`;

const WebhooksRowView = (props) => {
  const {
    webhooks,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    sectionWidth,
    viewAs,
    setViewAs,
    setTitleHistory,
  } = props;

  useEffect(() => {
    if (viewAs !== "table" && viewAs !== "row") return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return (
    <StyledRowContainer useReactWindow={false}>
      {webhooks.map((webhook) => (
        <WebhookRow
          key={webhook.id}
          webhook={webhook}
          sectionWidth={sectionWidth}
          toggleEnabled={toggleEnabled}
          deleteWebhook={deleteWebhook}
          editWebhook={editWebhook}
          setTitleHistory={setTitleHistory}
        />
      ))}
    </StyledRowContainer>
  );
};

export default inject(({ webhooksStore, setup }) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, setTitleHistory } = webhooksStore;

  const { viewAs, setViewAs } = setup;

  return {
    webhooks,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    viewAs,
    setViewAs,
    setTitleHistory,
  };
})(observer(WebhooksRowView));
