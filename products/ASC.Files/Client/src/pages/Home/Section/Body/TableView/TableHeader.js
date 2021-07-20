import React, { useState } from "react";
import TableHeader from "@appserver/components/table-container/TableHeader";
import TableGroupMenu from "@appserver/components/table-container/TableGroupMenu";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { FilterType } from "@appserver/common/constants";
import DropDownItem from "@appserver/components/drop-down-item";

const FilesTableHeader = (props) => {
  const {
    t,
    filter,
    selectedFolderId,
    containerRef,
    isHeaderVisible,
    isHeaderChecked,
    isHeaderIndeterminate,
    getHeaderMenu,
    setSelected,
    setIsLoading,
    fetchFiles,
  } = props;

  const onColumnChange = (e) => {
    const key = e.currentTarget.dataset.key;
    const columnIndex = columns.findIndex((c) => c.key === key);

    if (columnIndex === -1) return;

    columns[columnIndex].enable = !columns[columnIndex].enable;
    setColumns([...columns]);
  };

  const onNameClick = (val) => {
    const newFilter = filter.clone();
    newFilter.sortOrder = val ? "ascending" : "descending";

    setIsLoading(true);
    fetchFiles(selectedFolderId, newFilter).finally(() => setIsLoading(false));
  };

  const defaultColumns = [
    {
      key: "Name",
      title: t("Common:Name"),
      resizable: true,
      enable: true,
      default: true,
      onClick: onNameClick,
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
    {
      key: "Modified",
      title: t("Common:Type"),
      enable: false,
      resizable: true,
      onChange: onColumnChange,
    },
    {
      key: "Size",
      title: t("Common:Size"),
      enable: true,
      resizable: false,
      onChange: onColumnChange,
    },
    {
      key: "Type",
      title: t("Common:Type"),
      enable: false,
      resizable: true,
      onChange: onColumnChange,
    },
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

  const onSelect = (e) => {
    const key = e.currentTarget.dataset.key;
    setSelected(key);
  };

  const checkboxOptions = (
    <>
      <DropDownItem label={t("All")} data-key="all" onClick={onSelect} />
      <DropDownItem
        label={t("Translations:Folders")}
        data-key={FilterType.FoldersOnly}
        onClick={onSelect}
      />
      <DropDownItem
        label={t("Common:Documents")}
        data-key={FilterType.DocumentsOnly}
        onClick={onSelect}
      />
      <DropDownItem
        label={t("Translations:Presentations")}
        data-key={FilterType.PresentationsOnly}
        onClick={onSelect}
      />
      <DropDownItem
        label={t("Translations:Spreadsheets")}
        data-key={FilterType.SpreadsheetsOnly}
        onClick={onSelect}
      />
      <DropDownItem
        label={t("Images")}
        data-key={FilterType.ImagesOnly}
        onClick={onSelect}
      />
      <DropDownItem
        label={t("Media")}
        data-key={FilterType.MediaOnly}
        onClick={onSelect}
      />
      <DropDownItem
        label={t("Archives")}
        data-key={FilterType.ArchiveOnly}
        onClick={onSelect}
      />
      <DropDownItem
        label={t("AllFiles")}
        data-key={FilterType.FilesOnly}
        onClick={onSelect}
      />
    </>
  );

  return isHeaderVisible ? (
    <TableGroupMenu
      checkboxOptions={checkboxOptions}
      containerRef={containerRef}
      onSelect={onSelect}
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

export default inject(
  ({ filesStore, filesActionsStore, selectedFolderStore }) => {
    const {
      setSelected,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      setIsLoading,
      filter,
      fetchFiles,
    } = filesStore;
    const { getHeaderMenu } = filesActionsStore;

    console.log("filter", filter);

    return {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      filter,
      selectedFolderId: selectedFolderStore.id,

      setSelected,
      setIsLoading,
      fetchFiles,
      getHeaderMenu,
    };
  }
)(
  withTranslation(["Home", "Common", "Translations"])(
    observer(FilesTableHeader)
  )
);
