import React, { useRef } from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";

export const WebhookTableHeader = ({ sectionWidth, tableRef }) => {
  const columns = useRef([
    {
      key: "Name",
      title: "Name",
      resizable: true,
      enable: true,
      default: true,
      active: true,
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
  ]);
  return (
    <TableHeader
      columns={columns.current}
      containerRef={tableRef}
      sectionWidth={sectionWidth}
      showSettings={false}
      style={{ position: "absolute" }}
    />
  );
};
