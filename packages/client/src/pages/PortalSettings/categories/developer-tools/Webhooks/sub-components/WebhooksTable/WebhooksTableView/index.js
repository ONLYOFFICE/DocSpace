import React, { useRef, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

import styled from "styled-components";

import TableContainer from "@docspace/components/table-container/TableContainer";
import TableBody from "@docspace/components/table-container/TableBody";

import { WebhooksTableRow } from "./WebhooksTableRow";
import WebhookTableHeader from "./WebhookTableHeader";

import { Base } from "@docspace/components/themes";

const TableWrapper = styled(TableContainer)`
  margin-top: 16px;

  .table-container_caption-header {
    position: absolute;
  }

  .table-list-item {
    margin-top: -1px;
    &:hover {
      cursor: pointer;
      background-color: ${(props) => (props.theme.isBase ? "#F8F9F9" : "#282828")};
    }
  }
`;

TableWrapper.defaultProps = { theme: Base };

const WebhooksTableView = (props) => {
  const {
    webhooks,
    loadWebhooks,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    sectionWidth,
    viewAs,
    setViewAs,
    setTitleHistory,
  } = props;

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
      }}
      useReactWindow>
      <WebhookTableHeader sectionWidth={sectionWidth} tableRef={tableRef} />
      <TableBody
        itemHeight={49}
        useReactWindow
        filesLength={webhooks.length}
        fetchMoreFiles={loadWebhooks}
        hasMoreFiles={false}
        itemCount={webhooks.length}>
        {webhooks.map((webhook, index) => (
          <WebhooksTableRow
            key={webhook.id}
            webhook={webhook}
            index={index}
            toggleEnabled={toggleEnabled}
            deleteWebhook={deleteWebhook}
            editWebhook={editWebhook}
            setTitleHistory={setTitleHistory}
          />
        ))}
      </TableBody>
    </TableWrapper>
  );
};

export default inject(({ webhooksStore, setup }) => {
  const { webhooks, toggleEnabled, deleteWebhook, editWebhook, loadWebhooks, setTitleHistory } =
    webhooksStore;

  const { viewAs, setViewAs } = setup;

  return {
    webhooks,
    toggleEnabled,
    deleteWebhook,
    editWebhook,
    viewAs,
    setViewAs,
    loadWebhooks,
    setTitleHistory,
  };
})(observer(WebhooksTableView));
