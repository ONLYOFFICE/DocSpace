import { makeAutoObservable } from "mobx";
import { TableVersions } from "SRC_DIR/helpers/constants";

const TABLE_COLUMNS = `filesTableColumns_ver-${TableVersions.Files}`;
const TABLE_ROOMS_COLUMNS = `roomsTableColumns_ver-${TableVersions.Rooms}`;

const COLUMNS_SIZE = `filesColumnsSize_ver-${TableVersions.Files}`;
const COLUMNS_ROOMS_SIZE = `roomsColumnsSize_ver-${TableVersions.Rooms}`;

const COLUMNS_SIZE_INFO_PANEL = `filesColumnsSizeInfoPanel_ver-${TableVersions.Files}`;
const COLUMNS_ROOMS_SIZE_INFO_PANEL = `roomsColumnsSizeInfoPanel_ver-${TableVersions.Rooms}`;

class TableStore {
  authStore;
  treeFoldersStore;

  roomColumnNameIsEnabled = true; // always true
  roomColumnTypeIsEnabled = false;
  roomColumnTagsIsEnabled = true;
  roomColumnOwnerIsEnabled = false;
  roomColumnActivityIsEnabled = true;

  nameColumnIsEnabled = true; // always true
  authorColumnIsEnabled = false;
  createdColumnIsEnabled = true;
  modifiedColumnIsEnabled = true;
  sizeColumnIsEnabled = true;
  typeColumnIsEnabled = true;
  quickButtonsColumnIsEnabled = true;

  constructor(authStore, treeFoldersStore) {
    makeAutoObservable(this);

    this.authStore = authStore;
    this.treeFoldersStore = treeFoldersStore;
  }

  setRoomColumnType = (enable) => {
    this.roomColumnTypeIsEnabled = enable;
  };

  setRoomColumnTags = (enable) => {
    this.roomColumnTagsIsEnabled = enable;
  };

  setRoomColumnOwner = (enable) => {
    this.roomColumnOwnerIsEnabled = enable;
  };

  setRoomColumnActivity = (enable) => {
    this.roomColumnActivityIsEnabled = enable;
  };

  setAuthorColumn = (enable) => {
    this.authorColumnIsEnabled = enable;
  };
  setCreatedColumn = (enable) => {
    this.createdColumnIsEnabled = enable;
  };
  setModifiedColumn = (enable) => {
    this.modifiedColumnIsEnabled = enable;
  };
  setSizeColumn = (enable) => {
    this.sizeColumnIsEnabled = enable;
  };
  setTypeColumn = (enable) => {
    this.typeColumnIsEnabled = enable;
  };
  setQuickButtonsColumn = (enable) => {
    this.quickButtonsColumnIsEnabled = enable;
  };

  setColumnsEnable = () => {
    const storageColumns = localStorage.getItem(this.tableStorageName);
    const splitColumns = storageColumns && storageColumns.split(",");

    if (splitColumns) {
      const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;
      const isRooms = isRoomsFolder || isArchiveFolder;

      if (isRooms) {
        this.setRoomColumnType(splitColumns.includes("Type"));
        this.setRoomColumnTags(splitColumns.includes("Tags"));
        this.setRoomColumnOwner(splitColumns.includes("Owner"));
        this.setRoomColumnActivity(splitColumns.includes("Activity"));
      } else {
        this.setAuthorColumn(splitColumns.includes("Author"));
        this.setCreatedColumn(splitColumns.includes("Created"));
        this.setModifiedColumn(splitColumns.includes("Modified"));
        this.setSizeColumn(splitColumns.includes("Size"));
        this.setTypeColumn(splitColumns.includes("Type"));
        this.setQuickButtonsColumn(splitColumns.includes("QuickButtons"));
      }
    }
  };

  setColumnEnable = (key) => {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;

    switch (key) {
      case "Author":
        this.setAuthorColumn(!this.authorColumnIsEnabled);
        return;
      case "Created":
        this.setCreatedColumn(!this.createdColumnIsEnabled);
        return;
      case "Modified":
        this.setModifiedColumn(!this.modifiedColumnIsEnabled);
        return;
      case "Size":
        this.setSizeColumn(!this.sizeColumnIsEnabled);
        return;
      case "Type":
        isRooms
          ? this.setRoomColumnType(!this.roomColumnTypeIsEnabled)
          : this.setTypeColumn(!this.typeColumnIsEnabled);
        return;
      case "QuickButtons":
        this.setQuickButtonsColumn(!this.quickButtonsColumnIsEnabled);
        return;
      case "Owner":
        this.setRoomColumnOwner(!this.roomColumnOwnerIsEnabled);
        return;
      case "Tags":
        this.setRoomColumnTags(!this.roomColumnTagsIsEnabled);
        return;
      case "Activity":
        this.setRoomColumnActivity(!this.roomColumnActivityIsEnabled);
        return;
      default:
        return;
    }
  };

  getColumns = (defaultColumns) => {
    const storageColumns = localStorage.getItem(this.tableStorageName);
    const splitColumns = storageColumns && storageColumns.split(",");

    const columns = [];

    if (splitColumns) {
      this.setColumnsEnable();

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

  get tableStorageName() {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const userId = this.authStore.userStore.user.id;

    return isRooms
      ? `${TABLE_ROOMS_COLUMNS}=${userId}`
      : `${TABLE_COLUMNS}=${userId}`;
  }

  get columnStorageName() {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const userId = this.authStore.userStore.user.id;

    return isRooms
      ? `${COLUMNS_ROOMS_SIZE}=${userId}`
      : `${COLUMNS_SIZE}=${userId}`;
  }

  get columnInfoPanelStorageName() {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const userId = this.authStore.userStore.user.id;

    return isRooms
      ? `${COLUMNS_ROOMS_SIZE_INFO_PANEL}=${userId}`
      : `${COLUMNS_SIZE_INFO_PANEL}=${userId}`;
  }

  get filesColumnStorageName() {
    const userId = this.authStore.userStore.user.id;
    return `${COLUMNS_SIZE}=${userId}`;
  }
  get roomsColumnStorageName() {
    const userId = this.authStore.userStore.user.id;
    return `${COLUMNS_ROOMS_SIZE}=${userId}`;
  }
  get filesColumnInfoPanelStorageName() {
    const userId = this.authStore.userStore.user.id;
    return `${COLUMNS_SIZE_INFO_PANEL}=${userId}`;
  }
  get roomsColumnInfoPanelStorageName() {
    const userId = this.authStore.userStore.user.id;
    return `${COLUMNS_ROOMS_SIZE_INFO_PANEL}=${userId}`;
  }
}

export default TableStore;
