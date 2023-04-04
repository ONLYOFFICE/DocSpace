import React, { useRef } from "react";

import styled from "styled-components";

import TableContainer from "@docspace/components/table-container/TableContainer";
import TableBody from "@docspace/components/table-container/TableBody";

import { WebhooksTableRow } from "./WebhooksTableRow";

import { WebhookTableHeader } from "./WebhookTableHeader";

const TableWrapper = styled(TableContainer)`
  margin-top: 16px;
`;

const WebhooksTableView = (props) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, sectionWidth } = props;

  const tableRef = useRef(null);

  return (
    <TableWrapper
      forwardedRef={tableRef}
      style={{
        gridTemplateColumns: "200px 500px 110px 24px",
      }}>
      <WebhookTableHeader sectionWidth={sectionWidth} tableRef={tableRef} />
      <TableBody>
        {webhooks.map((webhook, index) => (
          <WebhooksTableRow
            key={webhook.id}
            webhook={webhook}
            index={index}
            toggleEnabled={toggleEnabled}
            deleteWebhook={deleteWebhook}
            editWebhook={editWebhook}
          />
        ))}
      </TableBody>
    </TableWrapper>
  );
};

export default WebhooksTableView;
