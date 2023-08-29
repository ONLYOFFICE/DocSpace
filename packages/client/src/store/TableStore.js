import { makeAutoObservable } from "mobx";
import { TableVersions } from "SRC_DIR/helpers/constants";

const TABLE_COLUMNS = `filesTableColumns_ver-${TableVersions.Files}`;
const TABLE_ROOMS_COLUMNS = `roomsTableColumns_ver-${TableVersions.Rooms}`;
const TABLE_TRASH_COLUMNS = `trashTableColumns_ver-${TableVersions.Trash}`;

const COLUMNS_SIZE = `filesColumnsSize_ver-${TableVersions.Files}`;
const COLUMNS_ROOMS_SIZE = `roomsColumnsSize_ver-${TableVersions.Rooms}`;
const COLUMNS_TRASH_SIZE = `trashColumnsSize_ver-${TableVersions.Trash}`;

const COLUMNS_SIZE_INFO_PANEL = `filesColumnsSizeInfoPanel_ver-${TableVersions.Files}`;
const COLUMNS_ROOMS_SIZE_INFO_PANEL = `roomsColumnsSizeInfoPanel_ver-${TableVersions.Rooms}`;
const COLUMNS_TRASH_SIZE_INFO_PANEL = `trashColumnsSizeInfoPanel_ver-${TableVersions.Trash}`;

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
  roomColumnIsEnabled = true;
  erasureColumnIsEnabled = true;
  createdColumnIsEnabled = true;
  modifiedColumnIsEnabled = true;
  sizeColumnIsEnabled = true;
  typeColumnIsEnabled = true;
  quickButtonsColumnIsEnabled = true;

  authorTrashColumnIsEnabled = true;
  createdTrashColumnIsEnabled = false;
  sizeTrashColumnIsEnabled = false;
  typeTrashColumnIsEnabled = false;

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

  setRoomColumn = (enable) => {
    this.roomColumnIsEnabled = enable;
  };

  setErasureColumn = (enable) => {
    this.erasureColumnIsEnabled = enable;
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

  setAuthorTrashColumn = (enable) => (this.authorTrashColumnIsEnabled = enable);
  setCreatedTrashColumn = (enable) =>
    (this.createdTrashColumnIsEnabled = enable);
  setSizeTrashColumn = (enable) => (this.sizeTrashColumnIsEnabled = enable);
  setTypeTrashColumn = (enable) => (this.typeTrashColumnIsEnabled = enable);

  setColumnsEnable = () => {
    const storageColumns = localStorage.getItem(this.tableStorageName);
    const splitColumns = storageColumns && storageColumns.split(",");

    if (splitColumns) {
      const {
        isRoomsFolder,
        isArchiveFolder,
        isTrashFolder,
      } = this.treeFoldersStore;
      const isRooms = isRoomsFolder || isArchiveFolder;

      if (isRooms) {
        this.setRoomColumnType(splitColumns.includes("Type"));
        this.setRoomColumnTags(splitColumns.includes("Tags"));
        this.setRoomColumnOwner(splitColumns.includes("Owner"));
        this.setRoomColumnActivity(splitColumns.includes("Activity"));
        return;
      }

      if (isTrashFolder) {
        this.setRoomColumn(splitColumns.includes("Room"));
        this.setAuthorTrashColumn(splitColumns.includes("AuthorTrash"));
        this.setCreatedTrashColumn(splitColumns.includes("CreatedTrash"));
        this.setErasureColumn(splitColumns.includes("Erasure"));
        this.setSizeTrashColumn(splitColumns.includes("SizeTrash"));
        this.setTypeTrashColumn(splitColumns.includes("TypeTrash"));
        this.setQuickButtonsColumn(splitColumns.includes("QuickButtons"));
        return;
      }

      this.setModifiedColumn(splitColumns.includes("Modified"));
      this.setAuthorColumn(splitColumns.includes("Author"));
      this.setCreatedColumn(splitColumns.includes("Created"));
      this.setSizeColumn(splitColumns.includes("Size"));
      this.setTypeColumn(splitColumns.includes("Type"));
      this.setQuickButtonsColumn(splitColumns.includes("QuickButtons"));
    }
  };

  setColumnEnable = (key) => {
    const {
      isRoomsFolder,
      isArchiveFolder,
      isTrashFolder,
    } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;

    switch (key) {
      case "Room":
        this.setRoomColumn(!this.roomColumnIsEnabled);
        return;

      case "Author":
        this.setAuthorColumn(!this.authorColumnIsEnabled);
        return;
      case "AuthorTrash":
        this.setAuthorTrashColumn(!this.authorTrashColumnIsEnabled);
        return;

      case "Created":
        this.setCreatedColumn(!this.createdColumnIsEnabled);
        return;
      case "CreatedTrash":
        this.setCreatedTrashColumn(!this.createdTrashColumnIsEnabled);
        return;

      case "Modified":
        this.setModifiedColumn(!this.modifiedColumnIsEnabled);
        return;

      case "Erasure":
        this.setErasureColumn(!this.erasureColumnIsEnabled);
        return;

      case "Size":
        this.setSizeColumn(!this.sizeColumnIsEnabled);
        return;
      case "SizeTrash":
        this.setSizeTrashColumn(!this.sizeTrashColumnIsEnabled);
        return;

      case "Type":
        isRooms
          ? this.setRoomColumnType(!this.roomColumnTypeIsEnabled)
          : this.setTypeColumn(!this.typeColumnIsEnabled);
        return;
      case "TypeTrash":
        this.setTypeTrashColumn(!this.typeTrashColumnIsEnabled);
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
    const {
      isRoomsFolder,
      isArchiveFolder,
      isTrashFolder,
    } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const userId = this.authStore.userStore.user?.id;

    return isRooms
      ? `${TABLE_ROOMS_COLUMNS}=${userId}`
      : isTrashFolder
      ? `${TABLE_TRASH_COLUMNS}=${userId}`
      : `${TABLE_COLUMNS}=${userId}`;
  }

  get columnStorageName() {
    const {
      isRoomsFolder,
      isArchiveFolder,
      isTrashFolder,
    } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const userId = this.authStore.userStore.user?.id;

    return isRooms
      ? `${COLUMNS_ROOMS_SIZE}=${userId}`
      : isTrashFolder
      ? `${COLUMNS_TRASH_SIZE}=${userId}`
      : `${COLUMNS_SIZE}=${userId}`;
  }

  get columnInfoPanelStorageName() {
    const {
      isRoomsFolder,
      isArchiveFolder,
      isTrashFolder,
    } = this.treeFoldersStore;
    const isRooms = isRoomsFolder || isArchiveFolder;
    const userId = this.authStore.userStore.user?.id;

    return isRooms
      ? `${COLUMNS_ROOMS_SIZE_INFO_PANEL}=${userId}`
      : isTrashFolder
      ? `${COLUMNS_TRASH_SIZE_INFO_PANEL}=${userId}`
      : `${COLUMNS_SIZE_INFO_PANEL}=${userId}`;
  }

  get filesColumnStorageName() {
    const userId = this.authStore.userStore.user?.id;
    return `${COLUMNS_SIZE}=${userId}`;
  }
  get roomsColumnStorageName() {
    const userId = this.authStore.userStore.user?.id;
    return `${COLUMNS_ROOMS_SIZE}=${userId}`;
  }
  get trashColumnStorageName() {
    const userId = this.authStore.userStore.user?.id;
    return `${COLUMNS_TRASH_SIZE}=${userId}`;
  }

  get filesColumnInfoPanelStorageName() {
    const userId = this.authStore.userStore.user?.id;
    return `${COLUMNS_SIZE_INFO_PANEL}=${userId}`;
  }
  get roomsColumnInfoPanelStorageName() {
    const userId = this.authStore.userStore.user?.id;
    return `${COLUMNS_ROOMS_SIZE_INFO_PANEL}=${userId}`;
  }
  get trashColumnInfoPanelStorageName() {
    const userId = this.authStore.userStore.user?.id;
    return `${COLUMNS_TRASH_SIZE_INFO_PANEL}=${userId}`;
  }
}

export default TableStore;
