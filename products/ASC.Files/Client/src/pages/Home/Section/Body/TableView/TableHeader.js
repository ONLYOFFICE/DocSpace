import React from "react";
import TableHeader from "@appserver/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

const TABLE_VERSION = "2";
const TABLE_COLUMNS = `filesTableColumns_ver-${TABLE_VERSION}`;
const COLUMNS_SIZE = `filesColumnsSize_ver-${TABLE_VERSION}`;

class FilesTableHeader extends React.Component {
  constructor(props) {
    super(props);

    const { t, withContent, personal, userId } = props;

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
        defaultSize: 120,
        resizable: false,
      },
    ];

    personal && defaultColumns.splice(1, 1);

    const storageColumns = localStorage.getItem(`${TABLE_COLUMNS}=${userId}`);
    const splitColumns = storageColumns && storageColumns.split(",");
    const columns = this.getColumns(defaultColumns, splitColumns);
    const resetColumnsSize =
      (splitColumns && splitColumns.length !== columns.length) || !splitColumns;
    const tableColumns = columns.map((c) => c.enable && c.key);
    this.setTableColumns(tableColumns);

    this.state = { columns, resetColumnsSize };
  }

  setTableColumns = (tableColumns) => {
    localStorage.setItem(`${TABLE_COLUMNS}=${this.props.userId}`, tableColumns);
  };

  componentDidUpdate(prevProps) {
    const { columns } = this.state;
    if (this.props.withContent !== prevProps.withContent) {
      const columnIndex = columns.findIndex((c) => c.key === "Share");
      if (columnIndex === -1) return;

      columns[columnIndex].enable = this.props.withContent;
      this.setState({ columns });
    }
  }

  getColumns = (defaultColumns, splitColumns) => {
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
  };

  onColumnChange = (key, e) => {
    const { columns } = this.state;

    const columnIndex = columns.findIndex((c) => c.key === key);
    if (columnIndex === -1) return;

    columns[columnIndex].enable = !columns[columnIndex].enable;
    this.setState({ columns });

    const tableColumns = columns.map((c) => c.enable && c.key);
    this.setTableColumns(tableColumns);
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

  render() {
    const { containerRef, filter, sectionWidth, userId } = this.props;
    const { sortBy, sortOrder } = filter;
    const { columns, resetColumnsSize } = this.state;

    return (
      <TableHeader
        sorted={sortOrder === "descending"}
        sortBy={sortBy}
        containerRef={containerRef}
        columns={columns}
        columnStorageName={`${COLUMNS_SIZE}=${userId}`}
        sectionWidth={sectionWidth}
        resetColumnsSize={resetColumnsSize}
      />
    );
  }
}

export default inject(
  ({ auth, filesStore, selectedFolderStore, treeFoldersStore }) => {
    const {
      isHeaderVisible,
      setIsLoading,
      filter,
      fetchFiles,
      canShare,
    } = filesStore;
    const { isPrivacyFolder } = treeFoldersStore;

    const withContent = canShare || (canShare && isPrivacyFolder && isDesktop);
    const { personal } = auth.settingsStore;

    return {
      isHeaderVisible,
      filter,
      selectedFolderId: selectedFolderStore.id,
      withContent,
      personal,

      setIsLoading,
      fetchFiles,
      userId: auth.userStore.user.id,
    };
  }
)(
  withTranslation(["Home", "Common", "Translations"])(
    observer(FilesTableHeader)
  )
);
