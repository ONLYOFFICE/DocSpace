import React from "react";
import { inject, observer } from "mobx-react";

import TableHeader from "@appserver/components/table-container/TableHeader";

const TableHeaderContent = ({
  tableStorageName,
  columnStorageName,
  containerRef,
  sectionWidth,

  filter,
  fetchRooms,

  setIsLoading,
}) => {
  const [columns, setColumns] = React.useState(null);
  const [resetColumnsSize, setResetColumnsSize] = React.useState(false);

  const onFilter = (sortBy, e) => {
    e.preventDefault();
    e.stopPropagation();

    const newFilter = filter.clone();

    if (newFilter.sortBy !== sortBy) {
      newFilter.sortBy = sortBy;
    } else {
      newFilter.sortOrder =
        newFilter.sortOrder === "ascending" ? "descending" : "ascending";
    }

    setIsLoading(true);
    fetchRooms(newFilter.searchArea, newFilter).finally(() =>
      setIsLoading(false)
    );
  };

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
        onClick: onFilter,
      },
      {
        key: "Type",
        title: "Type",
        enable: false,
        resizable: true,
        sortBy: "Type",
        onChange: onColumnChange,
        onClick: onFilter,
      },
      {
        key: "Tags",
        title: "Tags",
        enable: true,
        resizable: true,
        sortBy: "Tags",
        onChange: onColumnChange,
        onClick: onFilter,
      },
      {
        key: "Owner",
        title: "Owner",
        enable: false,
        resizable: true,
        sortBy: "Author",
        onChange: onColumnChange,
        onClick: onFilter,
      },
      {
        key: "Activity",
        title: "Last activity",
        enable: true,
        resizable: true,
        sortBy: "DateAndTime",
        onChange: onColumnChange,
        onClick: onFilter,
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
  }, [tableStorageName, getColumns, filter]);

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

  const { sortBy, sortOrder } = filter;

  return columns ? (
    <TableHeader
      sectionWidth={sectionWidth}
      checkboxSize="32px"
      sorted={sortOrder === "descending"}
      sortBy={sortBy}
      columns={columns}
      columnStorageName={columnStorageName}
      containerRef={containerRef}
      resetColumnsSize={resetColumnsSize}
      sortingVisible={true}
    />
  ) : (
    <></>
  );
};

export default inject(({ roomsStore, filesStore }) => {
  const { setIsLoading } = filesStore;

  const { filter, fetchRooms } = roomsStore;

  return { filter, fetchRooms, setIsLoading };
})(observer(TableHeaderContent));
