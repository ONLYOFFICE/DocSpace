import React, { useState, useRef } from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

const TABLE_VERSION = "6";
const TABLE_COLUMNS = `webhooksHistoryTableColumns_ver-${TABLE_VERSION}`;
const COLUMNS_SIZE = `webhooksHistoryColumnsSize_ver-${TABLE_VERSION}`;

const getColumns = (defaultColumns, userId) => {
  const storageColumns = localStorage.getItem(`${TABLE_COLUMNS}=${userId}`);
  const columns = [];

  if (storageColumns) {
    const splitColumns = storageColumns.split(",");

    for (let col of defaultColumns) {
      const column = splitColumns.find((key) => key === col.key);
      column ? (col.enable = true) : (col.enable = false);

      columns.push(col);
    }
    return columns;
  } else {
    return defaultColumns;
  }
};

const HistoryTableHeader = (props) => {
  const { userId, sectionWidth, tableRef } = props;
  const { t } = useTranslation(["Webhooks", "People"]);

  const defaultColumns = useRef([
    {
      key: "Event ID",
      title: t("EventID"),
      resizable: true,
      enable: true,
      default: true,
      active: true,
      minWidth: 150,
      onChange: onColumnChange,
    },
    {
      key: "Status",
      title: t("People:UserStatus"),
      enable: true,
      resizable: true,
      onChange: onColumnChange,
    },
    {
      key: "Delivery",
      title: t("Delivery"),
      enable: true,
      resizable: true,
      onChange: onColumnChange,
    },
  ]);

  const [columns, setColumns] = useState(getColumns(defaultColumns.current, userId));

  const onColumnChange = (key, e, userId) => {
    const columnIndex = columns.findIndex((c) => c.key === key);

    if (columnIndex === -1) return;

    setColumns((prevColumns) =>
      prevColumns.map((item, index) =>
        index === columnIndex ? { ...item, enable: !item.enable } : item,
      ),
    );

    const tableColumns = columns.map((c) => c.enable && c.key);
    localStorage.setItem(`${TABLE_COLUMNS}=${userId}`, tableColumns);
  };

  return (
    <TableHeader
      columns={columns}
      checkboxSize="32px"
      containerRef={tableRef}
      sectionWidth={sectionWidth}
      showSettings={false}
      useReactWindow
      setHideColumns={() => {}}
      infoPanelVisible={true}
      columnStorageName={`${COLUMNS_SIZE}=${userId}`}
    />
  );
};

export default inject(({ auth }) => {
  return {
    userId: auth.userStore.user.id,
  };
})(observer(HistoryTableHeader));
