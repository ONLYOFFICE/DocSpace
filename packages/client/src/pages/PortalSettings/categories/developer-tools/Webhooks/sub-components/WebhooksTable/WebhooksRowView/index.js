import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";

import { isMobile } from "react-device-detect";

import RowContainer from "@docspace/components/row-container";

import { WebhookRow } from "./WebhookRow";

const WebhooksRowView = (props) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, sectionWidth, viewAs, setViewAs } =
    props;

  useEffect(() => {
    if (viewAs !== "table" && viewAs !== "row") return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  return (
    <RowContainer useReactWindow={false}>
      {webhooks.map((webhook) => (
        <WebhookRow
          key={webhook.id}
          webhook={webhook}
          sectionWidth={sectionWidth}
          toggleEnabled={toggleEnabled}
          deleteWebhook={deleteWebhook}
          editWebhook={editWebhook}
        />
      ))}
    </RowContainer>
  );
};

export default inject(({ webhooksStore, setup }) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook } = webhooksStore;

  const { viewAs, setViewAs } = setup;

  return {
    webhooks,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    viewAs,
    setViewAs,
  };
})(observer(WebhooksRowView));
