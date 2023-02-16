import React from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { Events } from "@docspace/common/constants";

class FilesTableHeader extends React.Component {
  constructor(props) {
    super(props);

    this.getTableColumns();

    this.isBeginScrolling = false;
  }

  getTableColumns = (fromUpdate = false) => {
    const { t, isRooms, isTrashFolder, getColumns } = this.props;

    const defaultColumns = [];

    if (isRooms) {
      const columns = [
        {
          key: "Name",
          title: t("Common:Name"),
          resizable: true,
          enable: this.props.roomColumnNameIsEnabled,
          default: true,
          sortBy: "AZ",
          minWidth: 210,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Type",
          title: t("Common:Type"),
          enable: this.props.roomColumnTypeIsEnabled,
          resizable: true,
          sortBy: "roomType",
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Tags",
          title: t("Common:Tags"),
          enable: this.props.roomColumnTagsIsEnabled,
          resizable: true,
          sortBy: "Tags",
          withTagRef: true,
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Owner",
          title: t("ByOwner"),
          enable: this.props.roomColumnOwnerIsEnabled,
          resizable: true,
          sortBy: "Author",
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Activity",
          title: t("ByLastModified"),
          enable: this.props.roomColumnActivityIsEnabled,
          resizable: true,
          sortBy: "DateAndTime",
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
      ];
      defaultColumns.push(...columns);
    } else if (isTrashFolder) {
      const columns = [
        {
          key: "Name",
          title: t("Common:Name"),
          resizable: true,
          enable: this.props.nameColumnIsEnabled,
          default: true,
          sortBy: "AZ",
          minWidth: 210,
          onClick: this.onFilter,
        },
        {
          key: "Room",
          title: t("Common:Room"),
          enable: this.props.roomColumnIsEnabled,
          resizable: true,
          sortBy: "Room",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "AuthorTrash",
          title: t("ByAuthor"),
          enable: this.props.authorTrashColumnIsEnabled,
          resizable: true,
          sortBy: "Author",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "CreatedTrash",
          title: t("ByCreation"),
          enable: this.props.createdTrashColumnIsEnabled,
          resizable: true,
          sortBy: "DateAndTimeCreation",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Erasure",
          title: t("ByErasure"),
          enable: this.props.erasureColumnIsEnabled,
          resizable: true,
          sortBy: "DateAndTime",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "SizeTrash",
          title: t("Common:Size"),
          enable: this.props.sizeTrashColumnIsEnabled,
          resizable: true,
          sortBy: "Size",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "TypeTrash",
          title: t("Common:Type"),
          enable: this.props.typeTrashColumnIsEnabled,
          resizable: true,
          sortBy: "Type",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "QuickButtons",
          title: "",
          enable: this.props.quickButtonsColumnIsEnabled,
          defaultSize: 75,
          resizable: false,
        },
      ];
      defaultColumns.push(...columns);
    } else {
      const columns = [
        {
          key: "Name",
          title: t("Common:Name"),
          resizable: true,
          enable: this.props.nameColumnIsEnabled,
          default: true,
          sortBy: "AZ",
          minWidth: 210,
          onClick: this.onFilter,
        },
        {
          key: "Author",
          title: t("ByAuthor"),
          enable: this.props.authorColumnIsEnabled,
          resizable: true,
          sortBy: "Author",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Created",
          title: t("ByCreation"),
          enable: this.props.createdColumnIsEnabled,
          resizable: true,
          sortBy: "DateAndTimeCreation",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Modified",
          title: t("ByLastModified"),
          enable: this.props.modifiedColumnIsEnabled,
          resizable: true,
          sortBy: "DateAndTime",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Size",
          title: t("Common:Size"),
          enable: this.props.sizeColumnIsEnabled,
          resizable: true,
          sortBy: "Size",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Type",
          title: t("Common:Type"),
          enable: this.props.typeColumnIsEnabled,
          resizable: true,
          sortBy: "Type",
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "QuickButtons",
          title: "",
          enable: this.props.quickButtonsColumnIsEnabled,
          defaultSize: 75,
          resizable: false,
        },
      ];
      defaultColumns.push(...columns);
    }

    const columns = getColumns(defaultColumns);
    const storageColumns = localStorage.getItem(this.props.tableStorageName);
    const splitColumns = storageColumns && storageColumns.split(",");
    const resetColumnsSize =
      (splitColumns && splitColumns.length !== columns.length) || !splitColumns;

    const tableColumns = columns.map((c) => c.enable && c.key);
    this.setTableColumns(tableColumns);
    if (fromUpdate) {
      this.setState({
        columns,
        resetColumnsSize,
      });
    } else {
      this.state = {
        columns,
        resetColumnsSize,
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
    if (
      this.props.isRooms !== prevProps.isRooms ||
      this.props.isTrashFolder !== prevProps.isTrashFolder
    ) {
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

  onColumnChange = (key) => {
    const { columns } = this.state;

    const columnIndex = columns.findIndex((c) => c.key === key);
    if (columnIndex === -1) return;

    this.props.setColumnEnable(key);

    columns[columnIndex].enable = !columns[columnIndex].enable;
    this.setState({ columns });

    const tableColumns = columns.map((c) => c.enable && c.key);
    this.setTableColumns(tableColumns);

    const event = new Event(Events.CHANGE_COLUMN);

    window.dispatchEvent(event);
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
      withPaging,
      tagRef,
      setHideColumns,
    } = this.props;

    const { columns, resetColumnsSize } = this.state;

    const sortBy = isRooms ? roomsFilter.sortBy : filter.sortBy;
    const sortOrder = isRooms ? roomsFilter.sortOrder : filter.sortOrder;

    // TODO: make some better
    let currentColumnStorageName = columnStorageName;
    let currentColumnInfoPanelStorageName = columnInfoPanelStorageName;

    if (columns.length === 5 && columnStorageName === filesColumnStorageName) {
      currentColumnStorageName = roomsColumnStorageName;
      currentColumnInfoPanelStorageName = roomsColumnInfoPanelStorageName;
    }

    if (columns.length === 7 && columnStorageName === roomsColumnStorageName) {
      currentColumnStorageName = filesColumnStorageName;
      currentColumnInfoPanelStorageName = filesColumnInfoPanelStorageName;
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
        resetColumnsSize={resetColumnsSize}
        sortingVisible={sortingVisible}
        infoPanelVisible={infoPanelVisible}
        useReactWindow={!withPaging}
        tagRef={tagRef}
        setHideColumns={setHideColumns}
      />
    );
  }
}

export default inject(
  ({ auth, filesStore, selectedFolderStore, treeFoldersStore, tableStore }) => {
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
    const {
      isRecentFolder,
      isRoomsFolder,
      isArchiveFolder,
      isTrashFolder,
    } = treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const withContent = canShare;
    const sortingVisible = !isRecentFolder;
    const { withPaging } = auth.settingsStore;

    const {
      tableStorageName,
      columnStorageName,
      columnInfoPanelStorageName,
      filesColumnStorageName,
      roomsColumnStorageName,
      filesColumnInfoPanelStorageName,
      roomsColumnInfoPanelStorageName,

      nameColumnIsEnabled,
      authorColumnIsEnabled,
      authorTrashColumnIsEnabled,
      createdColumnIsEnabled,
      createdTrashColumnIsEnabled,
      modifiedColumnIsEnabled,
      roomColumnIsEnabled,
      erasureColumnIsEnabled,
      sizeColumnIsEnabled,
      sizeTrashColumnIsEnabled,
      typeColumnIsEnabled,
      typeTrashColumnIsEnabled,
      quickButtonsColumnIsEnabled,

      roomColumnNameIsEnabled,
      roomColumnTypeIsEnabled,
      roomColumnTagsIsEnabled,
      roomColumnOwnerIsEnabled,
      roomColumnActivityIsEnabled,

      getColumns,
      setColumnEnable,
    } = tableStore;

    return {
      isHeaderChecked,
      filter,
      selectedFolderId: selectedFolderStore.id,
      withContent,
      sortingVisible,

      setIsLoading,
      fetchFiles,

      roomsFilter,
      fetchRooms,

      firstElemChecked,
      headerBorder,

      infoPanelVisible,
      withPaging,

      tableStorageName,
      columnStorageName,
      columnInfoPanelStorageName,
      filesColumnStorageName,
      roomsColumnStorageName,
      filesColumnInfoPanelStorageName,
      roomsColumnInfoPanelStorageName,

      nameColumnIsEnabled,
      authorColumnIsEnabled,
      authorTrashColumnIsEnabled,
      createdColumnIsEnabled,
      createdTrashColumnIsEnabled,
      modifiedColumnIsEnabled,
      roomColumnIsEnabled,
      erasureColumnIsEnabled,
      sizeColumnIsEnabled,
      sizeTrashColumnIsEnabled,
      typeColumnIsEnabled,
      typeTrashColumnIsEnabled,
      quickButtonsColumnIsEnabled,

      roomColumnNameIsEnabled,
      roomColumnTypeIsEnabled,
      roomColumnTagsIsEnabled,
      roomColumnOwnerIsEnabled,
      roomColumnActivityIsEnabled,

      getColumns,
      setColumnEnable,
      isRooms,
      isTrashFolder,
    };
  }
)(
  withTranslation(["Files", "Common", "Translations"])(
    observer(FilesTableHeader)
  )
);
