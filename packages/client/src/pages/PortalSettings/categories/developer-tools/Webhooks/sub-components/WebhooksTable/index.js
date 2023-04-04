import React from "react";

import { Consumer } from "@docspace/components/utils/context";

import { inject, observer } from "mobx-react";
import WebhooksTableView from "./WebhooksTableView";

const WebhooksTable = (props) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, viewAs } = props;

  return (
    <Consumer>
      {(context) =>
        viewAs === "table" ? (
          <WebhooksTableView
            webhooks={webhooks}
            toggleEnabled={toggleEnabled}
            deleteWebhook={deleteWebhook}
            editWebhook={editWebhook}
            sectionWidth={context.sectionWidth}
          />
        ) : (
          <></>
        )
      }
    </Consumer>
  );
};

export default inject(({ webhooksStore, setup }) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, retryWebhookEvent } = webhooksStore;

  const { viewAs } = setup;

  return {
    webhooks,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    retryWebhookEvent,
    viewAs,
  };
})(observer(WebhooksTable));
