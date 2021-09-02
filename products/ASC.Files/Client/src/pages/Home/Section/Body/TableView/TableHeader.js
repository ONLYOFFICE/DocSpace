import React from "react";
import TableHeader from "@appserver/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { FilterType } from "@appserver/common/constants";
import DropDownItem from "@appserver/components/drop-down-item";

const TABLE_COLUMNS = "filesTableColumns";

class FilesTableHeader extends React.Component {
  constructor(props) {
    super(props);

    const { t, withContent } = props;

    const defaultColumns = [
      {
        key: "Name",
        title: t("Common:Name"),
        resizable: true,
        enable: true,
        default: true,
        sortBy: "AZ",
        minWidth: 180,
        onClick: this.onFilter,
      },
      {
        key: "Author",
        title: t("ByAuthor"),
        enable: true,
        resizable: true,
        sortBy: "Author",
        onClick: this.onFilter,
        onChange: this.onColumnChange,
      },
      {
        key: "Created",
        title: t("ByCreationDate"),
        enable: false,
        resizable: true,
        sortBy: "DateAndTimeCreation",
        onClick: this.onFilter,
        onChange: this.onColumnChange,
      },
      {
        key: "Modified",
        title: t("ByLastModifiedDate"),
        enable: true,
        resizable: true,
        sortBy: "DateAndTime",
        onClick: this.onFilter,
        onChange: this.onColumnChange,
      },
      {
        key: "Size",
        title: t("Common:Size"),
        enable: true,
        resizable: true,
        sortBy: "Size",
        onClick: this.onFilter,
        onChange: this.onColumnChange,
      },
      {
        key: "Type",
        title: t("Common:Type"),
        enable: false,
        resizable: true,
        sortBy: "Type",
        onClick: this.onFilter,
        onChange: this.onColumnChange,
      },
      {
        key: "Share",
        title: "",
        enable: withContent,
        defaultSize: 80,
        resizable: false,
      },
    ];

    const columns = this.getColumns(defaultColumns);

    this.state = { columns };
  }

  componentDidUpdate(prevProps) {
    const { columns } = this.state;
    if (this.props.withContent !== prevProps.withContent) {
      const columnIndex = columns.findIndex((c) => c.key === "Share");
      if (columnIndex === -1) return;

      columns[columnIndex].enable = this.props.withContent;
      this.setState({ columns });
    }
  }

  getColumns = (defaultColumns) => {
    const storageColumns = localStorage.getItem(TABLE_COLUMNS);
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

  onColumnChange = (key, e) => {
    const { columns } = this.state;

    const columnIndex = columns.findIndex((c) => c.key === key);
    if (columnIndex === -1) return;

    columns[columnIndex].enable = !columns[columnIndex].enable;
    this.setState({ columns });

    const tableColumns = columns.map((c) => c.enable && c.key);
    localStorage.setItem(TABLE_COLUMNS, tableColumns);
  };

  onFilter = (sortBy) => {
    const { filter, selectedFolderId, setIsLoading, fetchFiles } = this.props;
    const newFilter = filter.clone();

    if (newFilter.sortBy !== sortBy) {
      newFilter.sortBy = sortBy;
    } else {
      newFilter.sortOrder =
        newFilter.sortOrder === "ascending" ? "descending" : "ascending";
    }

    setIsLoading(true);
    fetchFiles(selectedFolderId, newFilter).finally(() => setIsLoading(false));
  };

  onChange = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  onSelect = (e) => {
    const key = e.currentTarget.dataset.key;
    this.props.setSelected(key);
  };

  setSelected = (checked) => {
    this.props.setSelected && this.props.setSelected(checked ? "all" : "none");
  };

  render() {
    const {
      t,
      containerRef,
      isHeaderVisible,
      isHeaderChecked,
      isHeaderIndeterminate,
      getHeaderMenu,
      filter,
      sectionWidth,
    } = this.props;

    const { sortBy, sortOrder } = filter;

    const { columns } = this.state;

    const checkboxOptions = (
      <>
        <DropDownItem label={t("All")} data-key="all" onClick={this.onSelect} />
        <DropDownItem
          label={t("Translations:Folders")}
          data-key={FilterType.FoldersOnly}
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("Common:Documents")}
          data-key={FilterType.DocumentsOnly}
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("Translations:Presentations")}
          data-key={FilterType.PresentationsOnly}
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("Translations:Spreadsheets")}
          data-key={FilterType.SpreadsheetsOnly}
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("Images")}
          data-key={FilterType.ImagesOnly}
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("Media")}
          data-key={FilterType.MediaOnly}
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("Archives")}
          data-key={FilterType.ArchiveOnly}
          onClick={this.onSelect}
        />
        <DropDownItem
          label={t("AllFiles")}
          data-key={FilterType.FilesOnly}
          onClick={this.onSelect}
        />
      </>
    );

    return (
      <TableHeader
        checkboxSize="32px"
        sorted={sortOrder === "descending"}
        sortBy={sortBy}
        setSelected={this.setSelected}
        containerRef={containerRef}
        columns={columns}
        columnStorageName="filesColumnsSize"
        sectionWidth={sectionWidth}
        isHeaderVisible={isHeaderVisible}
        checkboxOptions={checkboxOptions}
        onChange={this.onChange}
        isChecked={isHeaderChecked}
        isIndeterminate={isHeaderIndeterminate}
        headerMenu={getHeaderMenu(t)}
      />
    );
  }
}

export default inject(
  ({
    filesStore,
    filesActionsStore,
    selectedFolderStore,
    treeFoldersStore,
  }) => {
    const {
      setSelected,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      setIsLoading,
      filter,
      fetchFiles,
      canShare,
    } = filesStore;
    const { getHeaderMenu } = filesActionsStore;
    const { isPrivacyFolder } = treeFoldersStore;

    const withContent = canShare || (canShare && isPrivacyFolder && isDesktop);

    return {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      filter,
      selectedFolderId: selectedFolderStore.id,
      withContent,

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
