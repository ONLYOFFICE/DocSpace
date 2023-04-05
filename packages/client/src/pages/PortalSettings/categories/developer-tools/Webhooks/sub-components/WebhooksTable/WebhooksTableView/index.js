import React, { useRef, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

import styled from "styled-components";

import TableContainer from "@docspace/components/table-container/TableContainer";
import TableBody from "@docspace/components/table-container/TableBody";

import { WebhooksTableRow } from "./WebhooksTableRow";

import { WebhookTableHeader } from "./WebhookTableHeader";

const TableWrapper = styled(TableContainer)`
  margin-top: 16px;
`;

const WebhooksTableView = (props) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, sectionWidth, viewAs, setViewAs } =
    props;

  const tableRef = useRef(null);

  useEffect(() => {
    if (!sectionWidth) return;
    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

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
})(observer(WebhooksTableView));
