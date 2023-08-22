import React from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { Events } from "@docspace/common/constants";
import { SortByFieldName } from "../../../../../helpers/constants";

class FilesTableHeader extends React.Component {
  constructor(props) {
    super(props);

    this.getTableColumns();

    this.isBeginScrolling = false;
  }

  getTableColumns = (fromUpdate = false) => {
    const {
      t,
      isRooms,
      isTrashFolder,
      getColumns,
      columnStorageName,
      columnInfoPanelStorageName,
      isPublicRoom,
    } = this.props;

    const defaultColumns = [];

    if (isRooms) {
      const columns = [
        {
          key: "Name",
          title: t("Common:Name"),
          resizable: true,
          enable: this.props.roomColumnNameIsEnabled,
          default: true,
          sortBy: SortByFieldName.Name,
          minWidth: 210,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Type",
          title: t("Common:Type"),
          enable: this.props.roomColumnTypeIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.RoomType,
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Tags",
          title: t("Common:Tags"),
          enable: this.props.roomColumnTagsIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Tags,
          withTagRef: true,
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Owner",
          title: t("Common:Owner"),
          enable: this.props.roomColumnOwnerIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Author,
          onChange: this.onColumnChange,
          onClick: this.onRoomsFilter,
        },
        {
          key: "Activity",
          title: t("ByLastModified"),
          enable: this.props.roomColumnActivityIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.ModifiedDate,
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
          sortBy: SortByFieldName.Name,
          minWidth: 210,
          onClick: this.onFilter,
        },
        {
          key: "Room",
          title: t("Common:Room"),
          enable: this.props.roomColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Room,
          // onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "AuthorTrash",
          title: t("ByAuthor"),
          enable: this.props.authorTrashColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Author,
          // onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "CreatedTrash",
          title: t("ByCreation"),
          enable: this.props.createdTrashColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.CreationDate,
          // onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Erasure",
          title: t("ByErasure"),
          enable: this.props.erasureColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.ModifiedDate,
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "SizeTrash",
          title: t("Common:Size"),
          enable: this.props.sizeTrashColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Size,
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "TypeTrash",
          title: t("Common:Type"),
          enable: this.props.typeTrashColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Type,
          // onClick: this.onFilter,
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
      const authorBlock = !isPublicRoom
        ? {
            key: "Author",
            title: t("ByAuthor"),
            enable: this.props.authorColumnIsEnabled,
            resizable: true,
            sortBy: SortByFieldName.Author,
            // onClick: this.onFilter,
            onChange: this.onColumnChange,
          }
        : {};

      const columns = [
        {
          key: "Name",
          title: t("Common:Name"),
          resizable: true,
          enable: this.props.nameColumnIsEnabled,
          default: true,
          sortBy: SortByFieldName.Name,
          minWidth: 210,
          onClick: this.onFilter,
        },
        { ...authorBlock },
        {
          key: "Created",
          title: t("ByCreation"),
          enable: this.props.createdColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.CreationDate,
          // onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Modified",
          title: t("ByLastModified"),
          enable: this.props.modifiedColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.ModifiedDate,
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Size",
          title: t("Common:Size"),
          enable: this.props.sizeColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Size,
          onClick: this.onFilter,
          onChange: this.onColumnChange,
        },
        {
          key: "Type",
          title: t("Common:Type"),
          enable: this.props.typeColumnIsEnabled,
          resizable: true,
          sortBy: SortByFieldName.Type,
          // onClick: this.onFilter,
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
        columnStorageName,
        columnInfoPanelStorageName,
      });
    } else {
      this.state = {
        columns,
        resetColumnsSize,
        columnStorageName,
        columnInfoPanelStorageName,
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
    const {
      isRooms,
      isTrashFolder,
      columnStorageName,
      columnInfoPanelStorageName,
    } = this.props;

    if (
      isRooms !== prevProps.isRooms ||
      isTrashFolder !== prevProps.isTrashFolder ||
      columnStorageName !== prevProps.columnStorageName ||
      columnInfoPanelStorageName !== prevProps.columnInfoPanelStorageName
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
    const { filter, setIsLoading, isPublicRoom, publicRoomKey } = this.props;
    const newFilter = filter.clone();

    if (newFilter.sortBy !== sortBy) {
      newFilter.sortBy = sortBy;
    } else {
      newFilter.sortOrder =
        newFilter.sortOrder === "ascending" ? "descending" : "ascending";
    }

    setIsLoading(true);

    if (isPublicRoom) {
      window.DocSpace.navigate(
        `${
          window.DocSpace.location.pathname
        }?key=${publicRoomKey}&${newFilter.toUrlParams()}`
      );
    } else {
      window.DocSpace.navigate(
        `${window.DocSpace.location.pathname}?${newFilter.toUrlParams()}`
      );
    }
  };

  onRoomsFilter = (sortBy) => {
    const { roomsFilter, setIsLoading, navigate, location } = this.props;

    const newFilter = roomsFilter.clone();
    if (newFilter.sortBy !== sortBy) {
      newFilter.sortBy = sortBy;
    } else {
      newFilter.sortOrder =
        newFilter.sortOrder === "ascending" ? "descending" : "ascending";
    }

    setIsLoading(true);

    navigate(`${location.pathname}?${newFilter.toUrlParams()}`);
  };

  render() {
    const {
      t,
      containerRef,
      isHeaderChecked,
      filter,
      roomsFilter,
      isRooms,
      sectionWidth,
      firstElemChecked,
      sortingVisible,
      infoPanelVisible,

      withPaging,
      tagRef,
      setHideColumns,
    } = this.props;

    const {
      columns,
      resetColumnsSize,
      columnStorageName,
      columnInfoPanelStorageName,
    } = this.state;

    const sortBy = isRooms ? roomsFilter.sortBy : filter.sortBy;
    const sortOrder = isRooms ? roomsFilter.sortOrder : filter.sortOrder;

    return (
      <TableHeader
        isLengthenHeader={firstElemChecked || isHeaderChecked}
        checkboxSize="32px"
        sorted={sortOrder === "descending"}
        sortBy={sortBy}
        containerRef={containerRef}
        columns={columns}
        columnStorageName={columnStorageName}
        columnInfoPanelStorageName={columnInfoPanelStorageName}
        sectionWidth={sectionWidth}
        resetColumnsSize={resetColumnsSize}
        sortingVisible={sortingVisible}
        infoPanelVisible={infoPanelVisible}
        useReactWindow={!withPaging}
        tagRef={tagRef}
        setHideColumns={setHideColumns}
        settingsTitle={t("Files:TableSettingsTitle")}
      />
    );
  }
}

export default inject(
  ({
    auth,
    filesStore,
    selectedFolderStore,
    treeFoldersStore,
    tableStore,
    publicRoomStore,
    clientLoadingStore,
  }) => {
    const { isVisible: infoPanelVisible } = auth.infoPanelStore;

    const {
      isHeaderChecked,

      filter,

      canShare,
      firstElemChecked,
      headerBorder,
      roomsFilter,
    } = filesStore;
    const { isRecentFolder, isRoomsFolder, isArchiveFolder, isTrashFolder } =
      treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const withContent = canShare;
    const sortingVisible = !isRecentFolder;
    const { withPaging } = auth.settingsStore;

    const {
      tableStorageName,
      columnStorageName,
      columnInfoPanelStorageName,

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

    const { isPublicRoom, publicRoomKey } = publicRoomStore;

    return {
      isHeaderChecked,
      filter,
      selectedFolderId: selectedFolderStore.id,
      withContent,
      sortingVisible,

      setIsLoading: clientLoadingStore.setIsSectionBodyLoading,

      roomsFilter,

      firstElemChecked,
      headerBorder,

      infoPanelVisible,
      withPaging,

      tableStorageName,
      columnStorageName,
      columnInfoPanelStorageName,

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
      isPublicRoom,
      publicRoomKey,
    };
  }
)(
  withTranslation(["Files", "Common", "Translations", "Notifications"])(
    observer(FilesTableHeader)
  )
);
