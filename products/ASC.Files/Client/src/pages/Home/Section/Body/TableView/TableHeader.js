import React from "react";
import TableHeader from "@appserver/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { FilterType } from "@appserver/common/constants";
import DropDownItem from "@appserver/components/drop-down-item";

const TABLE_COLUMNS = "filesTableColumns";
const COLUMNS_SIZE = "filesColumnsSize";

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
        defaultSize: 80,
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
      userId,
      cbMenuItems,
      getCheckboxItemLabel,
    } = this.props;

    const { sortBy, sortOrder } = filter;

    const { columns, resetColumnsSize } = this.state;

    const checkboxOptions = (
      <>
        {cbMenuItems.map((key) => {
          const label = getCheckboxItemLabel(t, key);
          return (
        <DropDownItem
              key={key}
              label={label}
              data-key={key}
          onClick={this.onSelect}
        />
          );
        })}
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
        columnStorageName={`${COLUMNS_SIZE}=${userId}`}
        sectionWidth={sectionWidth}
        isHeaderVisible={isHeaderVisible}
        checkboxOptions={checkboxOptions}
        onChange={this.onChange}
        isChecked={isHeaderChecked}
        isIndeterminate={isHeaderIndeterminate}
        headerMenu={getHeaderMenu(t)}
        resetColumnsSize={resetColumnsSize}
      />
    );
  }
}

export default inject(
  ({
    auth,
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
      cbMenuItems,
      getCheckboxItemLabel,
    } = filesStore;
    const { getHeaderMenu } = filesActionsStore;
    const { isPrivacyFolder } = treeFoldersStore;

    const withContent = canShare || (canShare && isPrivacyFolder && isDesktop);
    const { personal } = auth.settingsStore;

    return {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      filter,
      selectedFolderId: selectedFolderStore.id,
      withContent,
      personal,

      setSelected,
      setIsLoading,
      fetchFiles,
      getHeaderMenu,
      userId: auth.userStore.user.id,
      cbMenuItems,
      getCheckboxItemLabel,
    };
  }
)(
  withTranslation(["Home", "Common", "Translations"])(
    observer(FilesTableHeader)
  )
);
