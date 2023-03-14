import React, { useRef } from "react";

import styled from "styled-components";

import TableContainer from "@docspace/components/table-container/TableContainer";
import TableHeader from "@docspace/components/table-container/TableHeader";
import TableBody from "@docspace/components/table-container/TableBody";

import { WebhooksTableRow } from "../WebhooksTableRow";

import { Consumer } from "@docspace/components/utils/context";

const TableWrapper = styled(TableContainer)`
  margin-top: 16px;
`;

export const WebhooksTable = ({ webhooks, toggleEnabled, deleteWebhook, editWebhook }) => {
  const tableRef = useRef(null);

  const columns = useRef([
    {
      key: "Name",
      title: "Name",
      enable: true,
      default: true,
      active: true,
      minWidth: 200,
    },
    {
      key: "URL",
      title: "URL",
      enable: true,
      resizable: true,
    },
    {
      key: "State",
      title: "State",
      enable: true,
      resizable: true,
    },
    {
      key: "Settings",
      title: "",
    },
  ]);

  return (
    <Consumer>
      {(context) => {
        return (
          <TableWrapper forwardedRef={tableRef}>
            <TableHeader
              columns={columns.current}
              containerRef={tableRef}
              checkboxMargin="12px"
              checkboxSize="48px"
              sectionWidth={context.sectionWidth}
              showSettings={false}
            />
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
      }}
    </Consumer>
  );
};
