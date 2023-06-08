import React, { useState, useRef, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

import styled from "styled-components";

import TableContainer from "@docspace/components/table-container/TableContainer";
import TableBody from "@docspace/components/table-container/TableBody";

import WebhooksTableRow from "./WebhooksTableRow";
import WebhookTableHeader from "./WebhookTableHeader";

import { Base } from "@docspace/components/themes";

const TableWrapper = styled(TableContainer)`
  margin-top: 16px;

  .header-container-text {
    font-size: 12px;
  }

  .table-container_header {
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

const TABLE_VERSION = "5";
const COLUMNS_SIZE = `webhooksConfigColumnsSize_ver-${TABLE_VERSION}`;
const INFO_PANEL_COLUMNS_SIZE = `infoPanelWebhooksConfigColumnsSize_ver-${TABLE_VERSION}`;

const WebhooksTableView = (props) => {
  const {
    webhooks,
    loadWebhooks,
    sectionWidth,
    viewAs,
    setViewAs,
    openSettingsModal,
    openDeleteModal,
    userId,
  } = props;

  const tableRef = useRef(null);
  const [hideColumns, setHideColumns] = useState(false);

  useEffect(() => {
    if (!sectionWidth) return;
    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  const columnStorageName = `${COLUMNS_SIZE}=${userId}`;
  const columnInfoPanelStorageName = `${INFO_PANEL_COLUMNS_SIZE}=${userId}`;

  return (
    <TableWrapper forwardedRef={tableRef} useReactWindow>
      <WebhookTableHeader
        sectionWidth={sectionWidth}
        tableRef={tableRef}
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        setHideColumns={setHideColumns}
      />
      <TableBody
        itemHeight={49}
        useReactWindow
        infoPanelVisible={false}
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        filesLength={webhooks.length}
        fetchMoreFiles={loadWebhooks}
        hasMoreFiles={false}
        itemCount={webhooks.length}>
        {webhooks.map((webhook, index) => (
          <WebhooksTableRow
            key={webhook.id}
            webhook={webhook}
            index={index}
            openSettingsModal={openSettingsModal}
            openDeleteModal={openDeleteModal}
            hideColumns={hideColumns}
          />
        ))}
      </TableBody>
    </TableWrapper>
  );
};

export default inject(({ webhooksStore, setup, auth }) => {
  const { webhooks, loadWebhooks } = webhooksStore;

  const { viewAs, setViewAs } = setup;
  const { id: userId } = auth.userStore.user;

  return {
    webhooks,
    viewAs,
    setViewAs,
    loadWebhooks,
    userId,
  };
})(observer(WebhooksTableView));
