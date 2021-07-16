import React from "react";
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

  const columns = [
    {
      key: 0,
      title: t("Common:Name"),
      resizable: true,
    },
    {
      key: 1,
      title: t("ByAuthor"),
      resizable: true,
    },
    {
      key: 2,
      title: t("ByCreationDate"),
      resizable: true,
    },
    // {
    //   key: 3,
    //   title: t("Common:Type"),
    //   resizable: true,
    // },
    {
      key: 4,
      title: t("Common:Size"),
      resizable: false,
    },
    {
      key: 5,
      title: "",
      resizable: false,
    },
  ];

  const onChange = (checked) => {
    setSelected(checked ? "all" : "none");
  };

  return isHeaderVisible ? (
    <TableGroupMenu
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
