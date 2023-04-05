import React from "react";

import { inject, observer } from "mobx-react";
import { Consumer } from "@docspace/components/utils/context";

import WebhooksTableView from "./WebhooksTableView";
import WebhooksRowView from "./WebhooksRowView";

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
          <WebhooksRowView sectionWidth={context.sectionWidth} webhooks={webhooks} />
        )
      }
    </Consumer>
  );
};
export default inject(({ setup }) => {
  const { viewAs } = setup;

  return {
    viewAs,
  };
})(observer(WebhooksTable));
