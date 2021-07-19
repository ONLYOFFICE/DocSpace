import React, { useState } from "react";
import TableHeader from "@appserver/components/table-container/TableHeader";
import TableGroupMenu from "@appserver/components/table-container/TableGroupMenu";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

const FilesTableHeader = (props) => {
  const {
    t,
    containerRef,
    isHeaderVisible,
    isHeaderChecked,
    isHeaderIndeterminate,
    getHeaderMenu,
    setSelected,
  } = props;

  const onColumnChange = (e) => {
    const key = e.currentTarget.dataset.key;
    const columnIndex = columns.findIndex((c) => c.key === key);

    if (columnIndex === -1) return;

    columns[columnIndex].enable = !columns[columnIndex].enable;
    setColumns([...columns]);
  };

  const defaultColumns = [
    {
      key: "Name",
      title: t("Common:Name"),
      resizable: true,
      enable: true,
      default: true,
      onChange: onColumnChange,
    },
    {
      key: "Author",
      title: t("ByAuthor"),
      enable: true,
      resizable: true,
      onChange: onColumnChange,
    },
    {
      key: "Created",
      title: t("ByCreationDate"),
      enable: true,
      resizable: true,
      onChange: onColumnChange,
    },
    // {
    //   key: "Modified",
    //   title: t("Common:Type"),
    //   enable: true,
    //   resizable: true,
    //   onChange: onColumnChange,
    // },
    {
      key: "Size",
      title: t("Common:Size"),
      enable: true,
      resizable: false,
      onChange: onColumnChange,
    },
    // {
    //   key: "Type",
    //   title: t("Common:Type"),
    //   enable: true,
    //   resizable: true,
    //   onChange: onColumnChange,
    // },
    {
      key: 5,
      title: "",
      enable: true,
      resizable: false,
      onChange: onColumnChange,
    },
  ];

  const [columns, setColumns] = useState(defaultColumns);

  const onChange = (checked) => {
    setSelected(checked ? "all" : "none");
  };

  return isHeaderVisible ? (
    <TableGroupMenu
      containerRef={containerRef}
      onChange={onChange}
      isChecked={isHeaderChecked}
      isIndeterminate={isHeaderIndeterminate}
      headerMenu={getHeaderMenu(t)}
    />
  ) : (
    <TableHeader
      setSelected={setSelected}
      containerRef={containerRef}
      columns={columns}
    />
  );
};

export default inject(({ filesStore, filesActionsStore }) => {
  const {
    setSelected,
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
  } = filesStore;
  const { getHeaderMenu } = filesActionsStore;

  return {
    setSelected,
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,

    getHeaderMenu,
  };
})(
  withTranslation(["Home", "Common", "Translations"])(
    observer(FilesTableHeader)
  )
);
