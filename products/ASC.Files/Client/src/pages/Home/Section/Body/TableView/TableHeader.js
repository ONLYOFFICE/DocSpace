import React from "react";
import TableHeader from "@appserver/components/table-container/TableHeader";
import TableGroupMenu from "@appserver/components/table-container/TableGroupMenu";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

const FilesTableHeader = (props) => {
  const {
    t,
    columns,
    containerRef,
    isHeaderVisible,
    isHeaderChecked,
    isHeaderIndeterminate,
    getHeaderMenu,
    setSelected,
  } = props;

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
    <TableHeader containerRef={containerRef} columns={columns} />
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
