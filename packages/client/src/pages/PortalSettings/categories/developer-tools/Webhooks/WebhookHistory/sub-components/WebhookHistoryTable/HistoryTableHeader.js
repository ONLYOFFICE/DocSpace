import React, { useRef } from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";

export const HistoryTableHeader = ({ sectionWidth, tableRef }) => {
  const columns = useRef([
    {
      key: "Event ID",
      title: "Event ID",
      resizable: true,
      enable: true,
      default: true,
      active: true,
    },
    {
      key: "Status",
      title: "Status",
      enable: true,
      resizable: true,
    },
    {
      key: "Delivery",
      title: "Delivery",
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
