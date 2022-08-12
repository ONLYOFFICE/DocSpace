import React from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

class FilesTableHeader extends React.Component {
  constructor(props) {
    super(props);

    this.getTableColumns();

    this.isBeginScrolling = false;
  }

  getTableColumns = (fromUpdate = false) => {
    const { t, personal, tableStorageName, isRooms } = this.props;

    const defaultColumns = [];
    if (isRooms) {
      const columns = [
        {
          key: "Name",
          title: t("Common:Name"),
          resizable: true,
          enable: true,
          default: true,
          sortBy: "AZ",
          minWidth: 210,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Type",
          title: t("Common:Type"),
          enable: false,
          resizable: true,
          sortBy: "roomType",
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Tags",
          title: t("Tags"),
          enable: true,
          resizable: true,
          sortBy: "Tags",
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Owner",
          title: t("ByOwner"),
          enable: false,
          resizable: true,
          sortBy: "Author",
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Activity",
          title: t("ByLastModifiedDate"),
          enable: true,
          resizable: true,
          sortBy: "DateAndTime",
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
      ];

      defaultColumns.push(...columns);
    } else {
      const columns = [
        {
          key: "Name",
          title: t("Common:Name"),
          resizable: true,
          enable: true,
          default: true,
          sortBy: "AZ",
          minWidth: 210,
          onClick: this.onFilter,
        },
        {
          key: "Author",
          title: t("ByAuthor"),
          enable: false,
          resizable: true,
          sortBy: "Author",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Created",
          title: t("ByCreationDate"),
          enable: true,
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
          enable: true,
          resizable: true,
          sortBy: "Type",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "QuickButtons",
          title: "",
          enable: true,
          defaultSize: 75,
          resizable: false,
        },
      ];

      personal && columns.splice(1, 1);

      defaultColumns.push(...columns);
    }

    const storageColumns = localStorage.getItem(tableStorageName);
    const splitColumns = storageColumns && storageColumns.split(",");
    const columns = this.getColumns(defaultColumns, splitColumns);
    const resetColumnsSize =
      (splitColumns && splitColumns.length !== columns.length) || !splitColumns;

    const tableColumns = columns.map((c) => c.enable && c.key);
    this.setTableColumns(tableColumns);
    if (fromUpdate) {
      this.setState({
        columns: columns,
        resetColumnsSize: resetColumnsSize,
        isRooms: isRooms,
      });
    } else {
      this.state = {
        columns: columns,
        resetColumnsSize: resetColumnsSize,
        isRooms: isRooms,
      };
    }
  };

  setTableColumns = (tableColumns) => {
    localStorage.setItem(this.props.tableStorageName, tableColumns);
  };

  componentDidMount() {
    this.customScrollElm = document.getElementsByClassName("section-scroll")[0];
    this.customScrollElm.addEventListener("scroll", this.onBeginScroll);
  }

  onBeginScroll = () => {
    const { firstElemChecked } = this.props;

    const currentScrollPosition = this.customScrollElm.scrollTop;
    const elem = document.getElementById("table-container_caption-header");

    if (currentScrollPosition === 0) {
      this.isBeginScrolling = false;

      this.props.headerBorder &&
        elem?.classList?.add("hotkeys-lengthen-header");

      !firstElemChecked && elem?.classList?.remove("lengthen-header");
      return;
    }

    if (!this.isBeginScrolling) {
      elem?.classList?.remove("hotkeys-lengthen-header");
      elem?.classList?.add("lengthen-header");
    }

    this.isBeginScrolling = true;
  };
  componentDidUpdate(prevProps) {
    if (this.props.isRooms !== this.state.isRooms) {
      return this.getTableColumns(true);
    }

    const { columns } = this.state;
    if (this.props.withContent !== prevProps.withContent) {
      const columnIndex = columns.findIndex((c) => c.key === "Share");
      if (columnIndex === -1) return;

      columns[columnIndex].enable = this.props.withContent;
      this.setState({ columns });
    }

    if (this.props.headerBorder) {
      const elem = document.getElementById("table-container_caption-header");
      elem?.classList?.add("hotkeys-lengthen-header");
    } else {
      const elem = document.getElementById("table-container_caption-header");
      elem?.classList?.remove("hotkeys-lengthen-header");
    }
  }

  componentWillUnmount() {
    this.customScrollElm.removeEventListener("scroll", this.onBeginScroll);
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

  onRoomsFilter = (sortBy) => {
    const {
      roomsFilter,
      selectedFolderId,
      setIsLoading,
      fetchRooms,
    } = this.props;

    const newFilter = roomsFilter.clone();
    if (newFilter.sortBy !== sortBy) {
      newFilter.sortBy = sortBy;
    } else {
      newFilter.sortOrder =
        newFilter.sortOrder === "ascending" ? "descending" : "ascending";
    }

    setIsLoading(true);
    fetchRooms(selectedFolderId, newFilter).finally(() => setIsLoading(false));
  };

  render() {
    const {
      containerRef,
      isHeaderChecked,
      filter,
      roomsFilter,
      isRooms,
      sectionWidth,
      firstElemChecked,
      sortingVisible,
      infoPanelVisible,
      columnStorageName,
      filesColumnStorageName,
      roomsColumnStorageName,
      columnInfoPanelStorageName,
      filesColumnInfoPanelStorageName,
      roomsColumnInfoPanelStorageName,
    } = this.props;

    // const { sortBy, sortOrder } = filter;
    const { columns, resetColumnsSize } = this.state;

    const sortBy = isRooms ? roomsFilter.sortBy : filter.sortBy;
    const sortOrder = isRooms ? roomsFilter.sortOrder : filter.sortOrder;

    // TODO: make some better
    let needReset = this.props.isRooms !== this.state.isRooms;
    let currentColumnStorageName = columnStorageName;
    let currentColumnInfoPanelStorageName = columnInfoPanelStorageName;

    if (columns.length === 5 && columnStorageName === filesColumnStorageName) {
      currentColumnStorageName = roomsColumnStorageName;
      currentColumnInfoPanelStorageName = roomsColumnInfoPanelStorageName;
      needReset = true;
    }

    if (columns.length === 7 && columnStorageName === roomsColumnStorageName) {
      currentColumnStorageName = filesColumnStorageName;
      currentColumnInfoPanelStorageName = filesColumnInfoPanelStorageName;
      needReset = true;
    }

    return (
      <TableHeader
        isLengthenHeader={firstElemChecked || isHeaderChecked}
        checkboxSize="32px"
        sorted={sortOrder === "descending"}
        sortBy={sortBy}
        containerRef={containerRef}
        columns={columns}
        columnStorageName={currentColumnStorageName}
        columnInfoPanelStorageName={currentColumnInfoPanelStorageName}
        sectionWidth={sectionWidth}
        resetColumnsSize={resetColumnsSize || needReset}
        sortingVisible={sortingVisible}
        infoPanelVisible={infoPanelVisible}
        useReactWindow
      />
    );
  }
}

export default inject(
  ({ auth, filesStore, selectedFolderStore, treeFoldersStore }) => {
    const { isVisible: infoPanelVisible } = auth.infoPanelStore;

    const {
      isHeaderChecked,
      setIsLoading,
      filter,
      fetchFiles,
      canShare,
      firstElemChecked,
      headerBorder,

      roomsFilter,
      fetchRooms,
    } = filesStore;
    const { isPrivacyFolder, isRecentFolder } = treeFoldersStore;

    const withContent = canShare || (canShare && isPrivacyFolder && isDesktop);
    const sortingVisible = !isRecentFolder;
    const { personal } = auth.settingsStore;

    return {
      isHeaderChecked,
      filter,
      selectedFolderId: selectedFolderStore.id,
      withContent,
      personal,
      sortingVisible,

      setIsLoading,
      fetchFiles,

      roomsFilter,
      fetchRooms,

      firstElemChecked,
      headerBorder,

      infoPanelVisible,
    };
  }
)(
  withTranslation(["Files", "Common", "Translations"])(
    observer(FilesTableHeader)
  )
);
