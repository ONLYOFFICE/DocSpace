import React, { useRef } from "react";

import styled from "styled-components";

import TableContainer from "@docspace/components/table-container/TableContainer";
import TableBody from "@docspace/components/table-container/TableBody";

import { WebhooksTableRow } from "../WebhooksTableRow";

import { Consumer } from "@docspace/components/utils/context";

import { inject, observer } from "mobx-react";
import { WebhookTableHeader } from "./WebhookTableHeader";

const TableWrapper = styled(TableContainer)`
  margin-top: 16px;
`;

const WebhooksTable = (props) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, retryWebhookEvent, viewAs } = props;

  const tableRef = useRef(null);

  return (
    <Consumer>
      {(context) =>
        viewAs === "table" ? (
          <TableWrapper
            forwardedRef={tableRef}
            style={{
              gridTemplateColumns: "200px 500px 110px 24px",
            }}>
            <WebhookTableHeader sectionWidth={context.sectionWidth} tableRef={tableRef} />
            <TableBody>
              {webhooks.map((webhook, index) => (
                <WebhooksTableRow
                  key={webhook.id}
                  webhook={webhook}
                  index={index}
                  toggleEnabled={toggleEnabled}
                  deleteWebhook={deleteWebhook}
                  editWebhook={editWebhook}
                  retryWebhookEvent={retryWebhookEvent}
                />
              ))}
            </TableBody>
          </TableWrapper>
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
