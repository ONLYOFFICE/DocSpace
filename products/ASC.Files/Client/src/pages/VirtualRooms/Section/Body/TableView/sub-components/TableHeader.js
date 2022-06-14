import React from "react";

import TableHeader from "@appserver/components/table-container/TableHeader";

const TableHeaderContent = ({
  tableStorageName,
  columnStorageName,
  containerRef,
  sectionWidth,
}) => {
  const [columns, setColumns] = React.useState(null);
  const [resetColumnsSize, setResetColumnsSize] = React.useState(false);

  React.useEffect(() => {
    const defaultColumns = [
      {
        key: "Name",
        title: "Name",
        resizable: true,
        enable: true,
        default: true,
        sortBy: "AZ",
        minWidth: 210,
      },
      {
        key: "Type",
        title: "Type",
        enable: false,
        resizable: true,
        sortBy: "Author",
        onChange: onColumnChange,
      },
      {
        key: "Tags",
        title: "Tags",
        enable: true,
        resizable: true,
        sortBy: "DateAndTimeCreation",
        onChange: onColumnChange,
      },
      {
        key: "Owner",
        title: "Owner",
        enable: false,
        resizable: true,
        sortBy: "DateAndTime",
        onChange: onColumnChange,
      },
      {
        key: "Activity",
        title: "Last activity",
        enable: true,
        resizable: true,
        sortBy: "Size",
        onChange: onColumnChange,
      },
      {
        key: "Size",
        title: "Size",
        enable: false,
        resizable: true,
        sortBy: "Type",
        onChange: onColumnChange,
      },
    ];

    const storageColumns = localStorage.getItem(tableStorageName);
    const splitColumns = storageColumns && storageColumns.split(",");
    const newColumns = getColumns(defaultColumns, splitColumns);
    const resetColumnsSize =
      (splitColumns && splitColumns.length !== newColumns.length) ||
      !splitColumns;
    const tableColumns = defaultColumns.map((c) => c.enable && c.key);

    setColumns(newColumns);
    setResetColumnsSize(resetColumnsSize);
    setTableColumns(tableColumns);
  }, [tableStorageName, getColumns]);

  const getColumns = React.useCallback((defaultColumns, splitColumns) => {
    const columns = [];

    if (splitColumns) {
      for (let col of defaultColumns) {
        const column = splitColumns.find((key) => key === col.key);
        column ? (col.enable = true) : (col.enable = false);

        columns.push(col);
      }
      return columns;
    } else {
      return defaultColumns;
    }
  }, []);

  const onColumnChange = React.useCallback(
    (key) => {
      setColumns((prevState) => {
        const newColumns = [...prevState];
        const columnIndex = newColumns.findIndex((c) => c.key === key);
        if (columnIndex === -1) return newColumns;

        newColumns[columnIndex].enable = !newColumns[columnIndex].enable;

        const tableColumns = newColumns.map((c) => c.enable && c.key);
        setTableColumns(tableColumns);

        return newColumns;
      });
    },
    [setTableColumns]
  );

  const setTableColumns = React.useCallback(
    (tableColumns) => {
      localStorage.setItem(tableStorageName, tableColumns);
    },
    [tableStorageName]
  );

  return columns ? (
    <TableHeader
      sectionWidth={sectionWidth}
      checkboxSize="32px"
      columns={columns}
      columnStorageName={columnStorageName}
      containerRef={containerRef}
      resetColumnsSize={resetColumnsSize}
    />
  ) : (
    <></>
  );
};

export default TableHeaderContent;
