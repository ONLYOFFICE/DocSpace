import React, { useRef } from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";

const HistoryTableHeader = (props) => {
  const { sectionWidth, tableRef, withPaging } = props;

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
      checkboxSize="32px"
      containerRef={tableRef}
      sectionWidth={sectionWidth}
      // useReactWindow={!withPaging}
      showSettings={false}
      style={{ position: "absolute" }}
    />
  );
};

export default inject(({ settingsStore }) => {
  const { withPaging } = settingsStore;

  return {
    withPaging,
  };
})(observer(HistoryTableHeader));
