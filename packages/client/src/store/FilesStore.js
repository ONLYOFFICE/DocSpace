import axios from "axios";
import { makeAutoObservable, runInAction } from "mobx";
import api from "@docspace/common/api";
import {
  FileType,
  FilterType,
  FolderType,
  FileStatus,
  RoomsType,
  RoomsTypeValues,
  RoomsProviderType,
} from "@docspace/common/constants";

import { combineUrl } from "@docspace/common/utils";
import { updateTempContent } from "@docspace/common/utils";
import { isMobile, isMobileOnly } from "react-device-detect";
import toastr from "@docspace/components/toast/toastr";
import config from "PACKAGE_FILE";
import { thumbnailStatuses } from "@docspace/client/src/helpers/filesConstants";
import { openDocEditor as openEditor } from "@docspace/client/src/helpers/filesUtils";
import { getDaysRemaining } from "@docspace/common/utils";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import {
  getCategoryType,
  getCategoryTypeByFolderType,
} from "SRC_DIR/helpers/utils";
import { isDesktop } from "@docspace/components/utils/device";
import { getContextMenuKeysByType } from "SRC_DIR/helpers/plugins";
import { PluginContextMenuItemType } from "SRC_DIR/helpers/plugins/constants";
import debounce from "lodash.debounce";

const { FilesFilter, RoomsFilter } = api;
const storageViewAs = localStorage.getItem("viewAs");

let requestCounter = 0;

const NotFoundHttpCode = 404;
const ForbiddenHttpCode = 403;
const PaymentRequiredHttpCode = 402;
const UnauthorizedHttpCode = 401;

const THUMBNAILS_CACHE = 500;
let timerId;

class FilesStore {
  authStore;

  selectedFolderStore;
  treeFoldersStore;
  filesSettingsStore;
  thirdPartyStore;

  accessRightsStore;

  isLoaded = false;
  isLoading = false;

  viewAs =
    isMobile && storageViewAs !== "tile" ? "row" : storageViewAs || "table";

  dragging = false;
  privacyInstructions = "https://www.onlyoffice.com/private-rooms.aspx";

  isInit = false;
  isUpdatingRowItem = false;
  passwordEntryProcess = false;

  tooltipPageX = 0;
  tooltipPageY = 0;
  startDrag = false;

  firstLoad = true;
  alreadyFetchingRooms = false;

  files = [];
  folders = [];

  selection = [];
  bufferSelection = null;
  selected = "close";

  filter = FilesFilter.getDefault(); //TODO: FILTER
  roomsFilter = RoomsFilter.getDefault();

  categoryType = getCategoryType(window.location);

  loadTimeout = null;
  hotkeyCaret = null;
  hotkeyCaretStart = null;
  activeFiles = [];
  activeFolders = [];

  firstElemChecked = false;
  headerBorder = false;

  enabledHotkeys = true;

  createdItem = null;
  scrollToItem = null;

  roomCreated = false;

  isLoadingFilesFind = false;
  pageItemsLength = null;
  isHidePagination = false;
  trashIsEmpty = false;
  mainButtonMobileVisible = true;
  filesIsLoading = false;

  isEmptyPage = false;
  isLoadedFetchFiles = false;

  tempActionFilesIds = [];
  tempActionFoldersIds = [];
  operationAction = false;

  isErrorRoomNotAvailable = false;

  roomsController = null;
  filesController = null;

  clearSearch = false;

  isLoadedEmptyPage = false;
  isMuteCurrentRoomNotifications = false;
  isPreview = false;
  tempFilter = null;

  highlightFile = {};
  thumbnails = new Set();

  constructor(
    authStore,
    selectedFolderStore,
    treeFoldersStore,
    filesSettingsStore,
    thirdPartyStore,
    accessRightsStore
  ) {
    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;

    makeAutoObservable(this);
    this.authStore = authStore;

    this.selectedFolderStore = selectedFolderStore;
    this.treeFoldersStore = treeFoldersStore;
    this.filesSettingsStore = filesSettingsStore;
    this.thirdPartyStore = thirdPartyStore;
    this.accessRightsStore = accessRightsStore;

    this.roomsController = new AbortController();
    this.filesController = new AbortController();
    const { socketHelper } = authStore.settingsStore;

    socketHelper.on("s:modify-folder", async (opt) => {
      console.log("[WS] s:modify-folder", opt);

      if (!(this.isLoading || this.operationAction))
        switch (opt?.cmd) {
          case "create":
            this.wsModifyFolderCreate(opt);
            break;
          case "update":
            this.wsModifyFolderUpdate(opt);
            break;
          case "delete":
            this.wsModifyFolderDelete(opt);
            break;
        }

      if (opt?.cmd === "create") {
        if (opt?.type === "file" && opt?.id)
          this.selectedFolderStore.filesCount++;
        if (opt?.type === "folder" && opt?.id)
          this.selectedFolderStore.foldersCount++;
        this.authStore.infoPanelStore.reloadSelection();
      } else if (opt?.cmd === "delete") {
        if (opt?.type === "file" && opt?.id)
          this.selectedFolderStore.filesCount--;
        if (opt?.type === "folder" && opt?.id)
          this.selectedFolderStore.foldersCount--;
        this.authStore.infoPanelStore.reloadSelection();
      }
    });

    socketHelper.on("refresh-folder", (id) => {
      if (!id || this.isLoading) return;

      //console.log(
      //  `selected folder id ${this.selectedFolderStore.id} an changed folder id ${id}`
      //);

      if (
        this.selectedFolderStore.id == id &&
        this.authStore.settingsStore.withPaging //TODO: no longer deletes the folder in other tabs
      ) {
        console.log("[WS] refresh-folder", id);
        this.fetchFiles(id, this.filter);
      }
    });

    socketHelper.on("s:markasnew-folder", ({ folderId, count }) => {
      console.log(`[WS] markasnew-folder ${folderId}:${count}`);

      const foundIndex =
        folderId && this.folders.findIndex((x) => x.id === folderId);
      if (foundIndex == -1) return;

      runInAction(() => {
        this.folders[foundIndex].new = count >= 0 ? count : 0;
        this.treeFoldersStore.fetchTreeFolders();
      });
    });

    socketHelper.on("s:markasnew-file", ({ fileId, count }) => {
      console.log(`[WS] markasnew-file ${fileId}:${count}`);

      const foundIndex = fileId && this.files.findIndex((x) => x.id === fileId);

      this.treeFoldersStore.fetchTreeFolders();
      if (foundIndex == -1) return;

      this.updateFileStatus(
        foundIndex,
        count > 0
          ? this.files[foundIndex].fileStatus | FileStatus.IsNew
          : this.files[foundIndex].fileStatus & ~FileStatus.IsNew
      );
    });

    //WAIT FOR RESPONSES OF EDITING FILE
    socketHelper.on("s:start-edit-file", (id) => {
      const foundIndex = this.files.findIndex((x) => x.id === id);
      if (foundIndex == -1) return;

      console.log(`[WS] s:start-edit-file`, id, this.files[foundIndex].title);

      this.updateSelectionStatus(
        id,
        this.files[foundIndex].fileStatus | FileStatus.IsEditing,
        true
      );

      this.updateFileStatus(
        foundIndex,
        this.files[foundIndex].fileStatus | FileStatus.IsEditing
      );
    });

    socketHelper.on("s:stop-edit-file", (id) => {
      const foundIndex = this.files.findIndex((x) => x.id === id);
      if (foundIndex == -1) return;

      console.log(`[WS] s:stop-edit-file`, id, this.files[foundIndex].title);

      this.updateSelectionStatus(
        id,
        this.files[foundIndex].fileStatus & ~FileStatus.IsEditing,
        false
      );

      this.updateFileStatus(
        foundIndex,
        this.files[foundIndex].fileStatus & ~FileStatus.IsEditing
      );

      this.getFileInfo(id);

      this.createThumbnail(this.files[foundIndex]);
    });
  }

  debounceRemoveFiles = debounce(() => {
    this.removeFiles(this.tempActionFilesIds);
  }, 1000);

  debounceRemoveFolders = debounce(() => {
    this.removeFiles(null, this.tempActionFoldersIds);
  }, 1000);

  wsModifyFolderCreate = async (opt) => {
    if (opt?.type === "file" && opt?.id) {
      const foundIndex = this.files.findIndex((x) => x.id === opt?.id);

      const file = JSON.parse(opt?.data);

      if (this.selectedFolderStore.id !== file.folderId) {
        const movedToIndex = this.getFolderIndex(file.folderId);
        if (movedToIndex > -1) this.folders[movedToIndex].filesCount++;
        return;
      }

      //To update a file version
      if (foundIndex > -1 && !this.authStore.settingsStore.withPaging) {
        this.getFileInfo(file.id);
        this.checkSelection(file);
      }

      if (foundIndex > -1) return;

      const fileInfo = await api.files.getFileInfo(file.id);

      if (this.files.findIndex((x) => x.id === opt?.id) > -1) return;
      console.log("[WS] create new file", fileInfo.id, fileInfo.title);

      const newFiles = [fileInfo, ...this.files];

      if (
        newFiles.length > this.filter.pageCount &&
        this.authStore.settingsStore.withPaging
      ) {
        newFiles.pop(); // Remove last
      }

      const newFilter = this.filter;
      newFilter.total += 1;

      runInAction(() => {
        this.setFilter(newFilter);
        this.setFiles(newFiles);
        this.treeFoldersStore.fetchTreeFolders();
      });
    } else if (opt?.type === "folder" && opt?.id) {
      const foundIndex = this.folders.findIndex((x) => x.id === opt?.id);

      if (foundIndex > -1) return;

      const folder = JSON.parse(opt?.data);

      if (this.selectedFolderStore.id !== folder.parentId) {
        const movedToIndex = this.getFolderIndex(folder.parentId);
        if (movedToIndex > -1) this.folders[movedToIndex].foldersCount++;
      }

      if (
        this.selectedFolderStore.id !== folder.parentId ||
        (folder.roomType &&
          folder.createdBy.id === this.authStore.userStore.user.id &&
          this.roomCreated)
      )
        return (this.roomCreated = false);

      const folderInfo = await api.files.getFolderInfo(folder.id);

      console.log("[WS] create new folder", folderInfo.id, folderInfo.title);

      const newFolders = [folderInfo, ...this.folders];

      if (
        newFolders.length > this.filter.pageCount &&
        this.authStore.settingsStore.withPaging
      ) {
        newFolders.pop(); // Remove last
      }

      const newFilter = this.filter;
      newFilter.total += 1;

      runInAction(() => {
        this.setFilter(newFilter);
        this.setFolders(newFolders);
      });
    }
  };

  wsModifyFolderUpdate = (opt) => {
    if (opt?.type === "file" && opt?.data) {
      const file = JSON.parse(opt?.data);
      if (!file || !file.id) return;

      this.getFileInfo(file.id); //this.setFile(file);
      console.log("[WS] update file", file.id, file.title);

      this.checkSelection(file);
    } else if (opt?.type === "folder" && opt?.data) {
      const folder = JSON.parse(opt?.data);
      if (!folder || !folder.id) return;

      api.files
        .getFolderInfo(folder.id)
        .then(() => this.setFolder(folderInfo))
        .catch(() => {
          // console.log("Folder deleted")
        });

      console.log("[WS] update folder", folder.id, folder.title);

      if (this.selection?.length) {
        const foundIndex = this.selection?.findIndex((x) => x.id === folder.id);
        if (foundIndex > -1) {
          runInAction(() => {
            this.selection[foundIndex] = folder;
          });
        }
      }

      if (this.bufferSelection) {
        const foundIndex = [this.bufferSelection].findIndex(
          (x) => x.id === folder.id
        );
        if (foundIndex > -1) {
          runInAction(() => {
            this.bufferSelection[foundIndex] = folder;
          });
        }
      }
    }
  };

  wsModifyFolderDelete = (opt) => {
    if (opt?.type === "file" && opt?.id) {
      const foundIndex = this.files.findIndex((x) => x.id === opt?.id);
      if (foundIndex == -1) return;

      console.log(
        "[WS] delete file",
        this.files[foundIndex].id,
        this.files[foundIndex].title
      );

      // this.setFiles(
      //   this.files.filter((_, index) => {
      //     return index !== foundIndex;
      //   })
      // );

      // const newFilter = this.filter.clone();
      // newFilter.total -= 1;
      // this.setFilter(newFilter);

      const tempActionFilesIds = JSON.parse(
        JSON.stringify(this.tempActionFilesIds)
      );
      tempActionFilesIds.push(this.files[foundIndex].id);

      this.setTempActionFilesIds(tempActionFilesIds);
      this.debounceRemoveFiles();

      // Hide pagination when deleting files
      runInAction(() => {
        this.isHidePagination = true;
      });

      runInAction(() => {
        if (
          this.files.length === 0 &&
          this.folders.length === 0 &&
          this.pageItemsLength > 1
        ) {
          this.isLoadingFilesFind = true;
        }
      });
    } else if (opt?.type === "folder" && opt?.id) {
      const foundIndex = this.folders.findIndex((x) => x.id === opt?.id);
      if (foundIndex == -1) return;

      console.log(
        "[WS] delete folder",
        this.folders[foundIndex].id,
        this.folders[foundIndex].title
      );

      const tempActionFoldersIds = JSON.parse(
        JSON.stringify(this.tempActionFoldersIds)
      );
      tempActionFoldersIds.push(this.folders[foundIndex].id);

      this.setTempActionFoldersIds(tempActionFoldersIds);
      this.debounceRemoveFolders();

      runInAction(() => {
        this.isHidePagination = true;
      });

      runInAction(() => {
        if (
          this.files.length === 0 &&
          this.folders.length === 0 &&
          this.pageItemsLength > 1
        ) {
          this.isLoadingFilesFind = true;
        }
      });
    }
  };

  setIsErrorRoomNotAvailable = (state) => {
    this.isErrorRoomNotAvailable = state;
  };

  setTempActionFilesIds = (tempActionFilesIds) => {
    this.tempActionFilesIds = tempActionFilesIds;
  };

  setTempActionFoldersIds = (tempActionFoldersIds) => {
    this.tempActionFoldersIds = tempActionFoldersIds;
  };

  setOperationAction = (operationAction) => {
    this.operationAction = operationAction;
  };

  setClearSearch = (clearSearch) => {
    this.clearSearch = clearSearch;
  };

  setIsPreview = (predicate) => {
    this.isPreview = predicate;
  };

  setTempFilter = (filser) => {
    this.tempFilter = filser;
  };

  setHighlightFile = (highlightFile) => {
    const { highlightFileId, isFileHasExst } = highlightFile;

    runInAction(() => {
      this.highlightFile = {
        id: highlightFileId,
        isExst: isFileHasExst,
      };
    });

    if (timerId) {
      clearTimeout(timerId);
      timerId = null;
    }

    if (Object.keys(highlightFile).length === 0) return;

    timerId = setTimeout(() => {
      runInAction(() => {
        this.highlightFile = {};
      });
    }, 1000);
  };

  checkSelection = (file) => {
    if (this.selection) {
      const foundIndex = this.selection?.findIndex((x) => x.id === file.id);
      if (foundIndex > -1) {
        runInAction(() => {
          this.selection[foundIndex] = file;
        });
      }
    }

    if (this.bufferSelection) {
      const foundIndex = [this.bufferSelection].findIndex(
        (x) => x.id === file.id
      );
      if (foundIndex > -1) {
        runInAction(() => {
          this.bufferSelection[foundIndex] = file;
        });
      }
    }
  };

  updateSelectionStatus = (id, status, isEditing) => {
    const index = this.selection.findIndex((x) => x.id === id);

    if (index !== -1) {
      this.selection[index].fileStatus = status;
      this.selection[index].isEditing = isEditing;
    }
  };

  addActiveItems = (files, folders) => {
    if (folders && folders.length) {
      if (!this.activeFolders.length) {
        this.setActiveFolders(folders);
      } else {
        folders.map((item) => this.activeFolders.push(item));
      }
    }

    if (files && files.length) {
      if (!this.activeFiles.length) {
        this.setActiveFiles(files);
      } else {
        files.map((item) => this.activeFiles.push(item));
      }
    }
  };

  setActiveFiles = (activeFiles) => {
    this.activeFiles = activeFiles;
  };

  setActiveFolders = (activeFolders) => {
    this.activeFolders = activeFolders;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setViewAs = (viewAs) => {
    this.viewAs = viewAs;
    localStorage.setItem("viewAs", viewAs);
    viewAs === "tile" && this.createThumbnails();
  };

  setPageItemsLength = (pageItemsLength) => {
    this.pageItemsLength = pageItemsLength;
  };

  setDragging = (dragging) => {
    this.dragging = dragging;
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setTooltipPosition = (tooltipPageX, tooltipPageY) => {
    this.tooltipPageX = tooltipPageX;
    this.tooltipPageY = tooltipPageY;
  };

  setStartDrag = (startDrag) => {
    this.selection = this.selection.filter(
      (x) => !x.providerKey || x.id !== x.rootFolderId
    ); // removed root thirdparty folders
    this.startDrag = startDrag;
  };

  setIsEmptyPage = (isEmptyPage) => {
    this.isEmptyPage = isEmptyPage;
  };

  setIsLoadedEmptyPage = (isLoadedEmptyPage) => {
    this.isLoadedEmptyPage = isLoadedEmptyPage;
  };

  get tooltipOptions() {
    if (!this.dragging) return null;

    const selectionLength = this.selection.length;
    const elementTitle = selectionLength && this.selection[0].title;
    const singleElement = selectionLength === 1;
    const filesCount = singleElement ? elementTitle : selectionLength;
    const { isShareFolder, isCommonFolder } = this.treeFoldersStore;

    let operationName;

    if (this.authStore.isAdmin && isShareFolder) {
      operationName = "copy";
    } else if (!this.authStore.isAdmin && (isShareFolder || isCommonFolder)) {
      operationName = "copy";
    } else {
      operationName = "move";
    }

    return {
      filesCount,
      operationName,
    };
  }

  initFiles = () => {
    if (this.isInit) return;

    const { isAuthenticated, settingsStore } = this.authStore;
    const { getFilesSettings } = this.filesSettingsStore;

    const {
      getPortalCultures,
      getIsEncryptionSupport,
      getEncryptionKeys,
      //setModuleInfo,
      isDesktopClient,
    } = settingsStore;

    //setModuleInfo(config.homepage, config.id);

    const requests = [];

    updateTempContent();
    if (!isAuthenticated) {
      return this.setIsLoaded(true);
    } else {
      updateTempContent(isAuthenticated);
    }

    if (!this.isEditor) {
      requests.push(
        getPortalCultures(),
        this.treeFoldersStore.fetchTreeFolders().then((treeFolders) => {
          if (!treeFolders || !treeFolders.length) return;

          const trashFolder = treeFolders.find(
            (f) => f.rootFolderType == FolderType.TRASH
          );

          if (!trashFolder) return;

          const isEmpty = !trashFolder.foldersCount && !trashFolder.filesCount;

          this.setTrashIsEmpty(isEmpty);
        })
      );

      if (isDesktopClient) {
        requests.push(getIsEncryptionSupport(), getEncryptionKeys());
      }
    }
    requests.push(getFilesSettings());

    return Promise.all(requests).then(() => this.setIsInit(true));
  };

  setIsInit = (isInit) => {
    this.isInit = isInit;
  };

  reset = () => {
    this.isInit = false;
    this.isLoaded = false;
    this.isLoading = false;
    this.firstLoad = true;

    this.alreadyFetchingRooms = false;

    this.files = [];
    this.folders = [];

    this.selection = [];
    this.bufferSelection = null;
    this.selected = "close";
  };
  setFirstLoad = (firstLoad) => {
    this.firstLoad = firstLoad;
  };

  setFiles = (files) => {
    const { socketHelper } = this.authStore.settingsStore;
    if (files.length === 0 && this.files.length === 0) return;

    if (this.files?.length > 0) {
      socketHelper.emit({
        command: "unsubscribe",
        data: {
          roomParts: this.files.map((f) => `FILE-${f.id}`),
          individual: true,
        },
      });
    }

    this.files = files;

    if (this.files?.length > 0) {
      socketHelper.emit({
        command: "subscribe",
        data: {
          roomParts: this.files.map((f) => `FILE-${f.id}`),
          individual: true,
        },
      });

      // this.files?.forEach((file) =>
      //   console.log("[WS] subscribe to file's changes", file.id, file.title)
      // );
    }

    this.createThumbnails();
  };

  setFolders = (folders) => {
    const { socketHelper } = this.authStore.settingsStore;
    if (folders.length === 0 && this.folders.length === 0) return;

    if (this.folders?.length > 0) {
      socketHelper.emit({
        command: "unsubscribe",
        data: {
          roomParts: this.folders.map((f) => `DIR-${f.id}`),
          individual: true,
        },
      });
    }

    this.folders = folders;

    if (this.folders?.length > 0) {
      socketHelper.emit({
        command: "subscribe",
        data: {
          roomParts: this.folders.map((f) => `DIR-${f.id}`),
          individual: true,
        },
      });
    }
  };

  getFileIndex = (id) => {
    const index = this.files.findIndex((x) => x.id === id);
    return index;
  };

  updateFileStatus = (index, status) => {
    if (index < 0) return;

    this.files[index].fileStatus = status;
  };
  updateRoomMute = (index, status) => {
    if (index < 0) return;

    this.folders[index].mute = status;
  };
  setFile = (file) => {
    const index = this.files.findIndex((x) => x.id === file.id);
    if (index !== -1) {
      this.files[index] = file;
      this.createThumbnail(file);
    }
  };

  updateSelection = (id) => {
    const indexFileList = this.filesList.findIndex(
      (filelist) => filelist.id === id
    );
    const indexSelectedRoom = this.selection.findIndex(
      (room) => room.id === id
    );

    if (~indexFileList && ~indexSelectedRoom) {
      this.selection[indexSelectedRoom] = this.filesList[indexFileList];
    }
    if (this.bufferSelection) {
      this.bufferSelection = this.filesList.find(
        (file) => file.id === this.bufferSelection.id
      );
    }
  };

  getFolderIndex = (id) => {
    const index = this.folders.findIndex((x) => x.id === id);
    return index;
  };

  updateFolder = (index, folder) => {
    if (index !== -1) this.folders[index] = folder;

    this.updateSelection(folder.id);
  };

  setFolder = (folder) => {
    const index = this.getFolderIndex(folder.id);

    this.updateFolder(index, folder);
  };

  getFilesChecked = (file, selected) => {
    if (!file.parentId) {
      if (this.activeFiles.includes(file.id)) return false;
    } else {
      if (this.activeFolders.includes(file.id)) return false;
    }

    const type = file.fileType;
    const roomType = file.roomType;

    switch (selected) {
      case "all":
        return true;
      case FilterType.FoldersOnly.toString():
        return file.parentId;
      case FilterType.DocumentsOnly.toString():
        return type === FileType.Document;
      case FilterType.PresentationsOnly.toString():
        return type === FileType.Presentation;
      case FilterType.SpreadsheetsOnly.toString():
        return type === FileType.Spreadsheet;
      case FilterType.ImagesOnly.toString():
        return type === FileType.Image;
      case FilterType.MediaOnly.toString():
        return type === FileType.Video || type === FileType.Audio;
      case FilterType.ArchiveOnly.toString():
        return type === FileType.Archive;
      case FilterType.FilesOnly.toString():
        return type || !file.parentId;
      case `room-${RoomsType.FillingFormsRoom}`:
        return roomType === RoomsType.FillingFormsRoom;
      case `room-${RoomsType.CustomRoom}`:
        return roomType === RoomsType.CustomRoom;
      case `room-${RoomsType.EditingRoom}`:
        return roomType === RoomsType.EditingRoom;
      case `room-${RoomsType.ReviewRoom}`:
        return roomType === RoomsType.ReviewRoom;
      case `room-${RoomsType.ReadOnlyRoom}`:
        return roomType === RoomsType.ReadOnlyRoom;
      default:
        return false;
    }
  };

  getFilesBySelected = (files, selected) => {
    let newSelection = [];
    files.forEach((file) => {
      const checked = this.getFilesChecked(file, selected);

      if (checked) newSelection.push(file);
    });

    return newSelection;
  };

  setSelected = (selected) => {
    if (selected === "close" || selected === "none") {
      this.setBufferSelection(null);
      this.setHotkeyCaretStart(null);
      this.setHotkeyCaret(null);
    }

    this.selected = selected;
    const files = this.filesList;
    this.selection = this.getFilesBySelected(files, selected);
  };

  setHotkeyCaret = (hotkeyCaret) => {
    if (hotkeyCaret || this.hotkeyCaret) {
      this.hotkeyCaret = hotkeyCaret;
    }
  };

  setHotkeyCaretStart = (hotkeyCaretStart) => {
    this.hotkeyCaretStart = hotkeyCaretStart;
  };

  setSelection = (selection) => {
    this.selection = selection;
  };

  setSelections = (added, removed, clear = false) => {
    if (clear) {
      this.selection = [];
    }

    let newSelections = JSON.parse(JSON.stringify(this.selection));

    for (let item of added) {
      if (!item) return;

      const value =
        this.viewAs === "tile"
          ? item.getAttribute("value")
          : item.getElementsByClassName("files-item")
          ? item.getElementsByClassName("files-item")[0]?.getAttribute("value")
          : null;

      if (!value) return;
      const splitValue = value && value.split("_");

      const fileType = splitValue[0];
      const id = splitValue.slice(1, -3).join("_");

      if (fileType === "file") {
        const isFound =
          this.selection.findIndex((f) => f.id == id && !f.isFolder) === -1;

        if (this.activeFiles.findIndex((f) => f == id) === -1) {
          isFound &&
            newSelections.push(
              this.filesList.find((f) => f.id == id && !f.isFolder)
            );
        }
      } else if (this.activeFolders.findIndex((f) => f == id) === -1) {
        const isFound =
          this.selection.findIndex((f) => f.id == id && f.isFolder) === -1;

        const selectableFolder = this.filesList.find(
          (f) => f.id == id && f.isFolder
        );
        selectableFolder.isFolder = true;

        isFound && newSelections.push(selectableFolder);
      }
    }

    for (let item of removed) {
      if (!item) return;

      const value =
        this.viewAs === "tile"
          ? item.getAttribute("value")
          : item.getElementsByClassName("files-item")
          ? item.getElementsByClassName("files-item")[0]?.getAttribute("value")
          : null;

      const splitValue = value && value.split("_");

      const fileType = splitValue[0];
      const id = splitValue.slice(1, -3).join("_");

      if (fileType === "file") {
        if (this.activeFiles.findIndex((f) => f == id) === -1) {
          newSelections = newSelections.filter(
            (f) => !(f.id == id && !f.isFolder)
          );
        }
      } else if (this.activeFolders.findIndex((f) => f == id) === -1) {
        newSelections = newSelections.filter(
          (f) => !(f.id == id && f.isFolder)
        );
      }
    }

    this.setSelection(newSelections);
  };

  setBufferSelection = (bufferSelection) => {
    this.bufferSelection = bufferSelection;
  };

  setIsLoadedFetchFiles = (isLoadedFetchFiles) => {
    this.isLoadedFetchFiles = isLoadedFetchFiles;
  };

  //TODO: FILTER
  setFilesFilter = (filter) => {
    const key = `UserFilter=${this.authStore.userStore.user.id}`;
    const value = `${filter.sortBy},${filter.pageCount},${filter.sortOrder}`;
    localStorage.setItem(key, value);

    this.setFilterUrl(filter);
    this.filter = filter;

    runInAction(() => {
      if (filter && this.isHidePagination) {
        this.isHidePagination = false;
      }
    });

    runInAction(() => {
      if (filter && this.isLoadingFilesFind) {
        this.isLoadingFilesFind = false;
      }
    });
  };

  resetUrl = () => {
    this.setFilesFilter(this.tempFilter);
  };

  setRoomsFilter = (filter) => {
    const key = `UserRoomsFilter=${this.authStore.userStore.user.id}`;
    const value = `${filter.sortBy},${filter.pageCount},${filter.sortOrder}`;
    localStorage.setItem(key, value);

    if (!this.authStore.settingsStore.withPaging) filter.pageCount = 100;

    this.setFilterUrl(filter, true);
    this.roomsFilter = filter;

    runInAction(() => {
      if (filter && this.isHidePagination) {
        this.isHidePagination = false;
      }
    });

    runInAction(() => {
      if (filter && this.isLoadingFilesFind) {
        this.isLoadingFilesFind = false;
      }
    });
  };

  setFilter = (filter) => {
    if (!this.authStore.settingsStore.withPaging) filter.pageCount = 100;
    this.filter = filter;
  };

  setFilesOwner = (folderIds, fileIds, ownerId) => {
    return api.files.setFileOwner(folderIds, fileIds, ownerId);
  };

  setFilterUrl = (filter) => {
    const filterParamsStr = filter.toUrlParams();

    const url = getCategoryUrl(this.categoryType, filter.folder);

    const pathname = `${url}?${filterParamsStr}`;

    const currentUrl = window.location.href.replace(window.location.origin, "");
    const newUrl = combineUrl(
      window.DocSpaceConfig?.proxy?.url,
      config.homepage,
      pathname
    );

    if (newUrl === currentUrl) return;

    window.DocSpace.navigate(newUrl, {
      state: {
        fromAccounts:
          window.DocSpace.location.pathname.includes("accounts/filter"),
        fromSettings: window.DocSpace.location.pathname.includes("settings"),
      },
      replace: !location.search,
    });
  };

  isEmptyLastPageAfterOperation = (newSelection) => {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;

    const selection =
      newSelection || this.selection?.length || [this.bufferSelection].length;

    const filter =
      isRoomsFolder || isArchiveFolder ? this.roomsFilter : this.filter;

    return (
      selection &&
      filter.page > 0 &&
      !filter.hasNext() &&
      selection === this.files.length + this.folders.length
    );
  };

  resetFilterPage = () => {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;

    let newFilter;

    newFilter =
      isRoomsFolder || isArchiveFolder
        ? this.roomsFilter.clone()
        : this.filter.clone();

    newFilter.page--;

    return newFilter;
  };

  refreshFiles = async () => {
    const res = await this.fetchFiles(this.selectedFolderStore.id, this.filter);
    return res;
  };

  fetchFiles = (
    folderId,
    filter,
    clearFilter = true,
    withSubfolders = false,
    clearSelection = true
  ) => {
    const { setSelectedNode } = this.treeFoldersStore;

    if (this.isLoading) {
      this.roomsController.abort();
      this.roomsController = new AbortController();
    }

    this.scrollToTop();

    const filterData = filter ? filter.clone() : FilesFilter.getDefault();
    filterData.folder = folderId;

    if (folderId === "@my" && this.authStore.userStore.user.isVisitor)
      return this.fetchRooms();

    this.setIsErrorRoomNotAvailable(false);
    this.setIsLoadedFetchFiles(false);

    const filterStorageItem =
      this.authStore.userStore.user?.id &&
      localStorage.getItem(`UserFilter=${this.authStore.userStore.user.id}`);

    if (filterStorageItem && !filter) {
      const splitFilter = filterStorageItem.split(",");

      filterData.sortBy = splitFilter[0];
      filterData.pageCount = +splitFilter[1];
      filterData.sortOrder = splitFilter[2];
    }

    if (!this.authStore.settingsStore.withPaging) {
      filterData.page = 0;
      filterData.pageCount = 100;
    }

    setSelectedNode([folderId + ""]);

    return api.files
      .getFolder(folderId, filterData, this.filesController.signal)
      .then(async (data) => {
        filterData.total = data.total;

        if (data.total > 0) {
          const lastPage = filterData.getLastPage();

          if (filterData.page > lastPage) {
            filterData.page = lastPage;

            return this.fetchFiles(
              folderId,
              filterData,
              clearFilter,
              withSubfolders
            );
          }
        }

        runInAction(() => {
          this.categoryType = getCategoryTypeByFolderType(
            data.current.rootFolderType,
            data.current.parentId
          );
        });

        if (this.isPreview) {
          //save filter for after closing preview change url
          this.setTempFilter(filterData);
        } else {
          this.setFilesFilter(filterData); //TODO: FILTER
        }

        const isPrivacyFolder =
          data.current.rootFolderType === FolderType.Privacy;

        runInAction(() => {
          const isEmptyList = [...data.folders, ...data.files].length === 0;

          if (filter && isEmptyList) {
            const {
              authorType,
              search,
              withSubfolders,
              filterType,
              searchInContent,
            } = filter;
            const isFiltered =
              authorType ||
              search ||
              !withSubfolders ||
              filterType ||
              searchInContent;

            if (isFiltered) {
              this.setIsEmptyPage(false);
            } else {
              this.setIsEmptyPage(isEmptyList);
            }
          } else {
            this.setIsEmptyPage(isEmptyList);
          }
          this.setFolders(isPrivacyFolder && isMobile ? [] : data.folders);
          this.setFiles(isPrivacyFolder && isMobile ? [] : data.files);
        });

        if (clearFilter) {
          if (clearSelection) {
            this.setSelected("close");
          }
        }

        const navigationPath = await Promise.all(
          data.pathParts.map(async (folder) => {
            const { Rooms, Archive } = FolderType;

            let folderId = folder;

            if (
              data.current.providerKey &&
              data.current.rootFolderType === Rooms &&
              this.treeFoldersStore.myRoomsId
            ) {
              folderId = this.treeFoldersStore.myRoomsId;
            }

            const folderInfo =
              data.current.id === folderId
                ? data.current
                : await api.files.getFolderInfo(folderId);

            const {
              id,
              title,
              roomType,
              rootFolderId,
              rootFolderType,
              parentId,
              mute,
            } = folderInfo;

            const isRootRoom =
              rootFolderId === id &&
              (rootFolderType === Rooms || rootFolderType === Archive);

            if (parentId === rootFolderId) {
              runInAction(() => {
                this.isMuteCurrentRoomNotifications = mute;
              });
            }

            return {
              id: folderId,
              title,
              isRoom: !!roomType,
              isRootRoom,
            };
          })
        ).then((res) => {
          return res
            .filter((item, index) => index !== res.length - 1)
            .reverse();
        });

        this.selectedFolderStore.setSelectedFolder({
          folders: data.folders,
          ...data.current,
          pathParts: data.pathParts,
          navigationPath: navigationPath,
          ...{ new: data.new },
        });

        const selectedFolder = {
          selectedFolder: { ...this.selectedFolderStore },
        };

        if (this.createdItem) {
          const newItem = this.filesList.find(
            (item) => item.id === this.createdItem.id
          );

          if (newItem) {
            this.setBufferSelection(newItem);
            this.setScrollToItem({
              id: newItem.id,
              type: this.createdItem.type,
            });
          }

          this.setCreatedItem(null);
        }

        return Promise.resolve(selectedFolder);
      })
      .catch((err) => {
        console.error(err);

        if (err?.response?.status === 402)
          this.authStore.currentTariffStatusStore.setPortalTariff();

        if (requestCounter > 0) return;

        requestCounter++;
        const isUserError = [
          NotFoundHttpCode,
          ForbiddenHttpCode,
          PaymentRequiredHttpCode,
          UnauthorizedHttpCode,
        ].includes(err?.response?.status);

        if (isUserError) {
          runInAction(() => {
            this.isErrorRoomNotAvailable = true;
          });
        } else {
          if (axios.isCancel(err)) {
            console.log("Request canceled", err.message);
          } else {
            toastr.error(err);
          }
        }
      })
      .finally(() => {
        this.setIsLoadedFetchFiles(true);
      });
  };

  fetchRooms = (
    folderId,
    filter,
    clearFilter = true,
    withSubfolders = false,
    clearSelection = true
  ) => {
    const { setSelectedNode, roomsFolderId } = this.treeFoldersStore;

    if (this.isLoading) {
      this.filesController.abort();
      this.filesController = new AbortController();
    }

    const filterData = !!filter ? filter.clone() : RoomsFilter.getDefault();

    const filterStorageItem = localStorage.getItem(
      `UserRoomsFilter=${this.authStore.userStore.user.id}`
    );

    if (filterStorageItem && !filter) {
      const splitFilter = filterStorageItem.split(",");

      filterData.sortBy = splitFilter[0];
      filterData.pageCount = +splitFilter[1];
      filterData.sortOrder = splitFilter[2];
    }

    if (!this.authStore.settingsStore.withPaging) {
      filterData.page = 0;
      filterData.pageCount = 100;
    }

    if (folderId) setSelectedNode([folderId + ""]);

    const request = () =>
      api.rooms
        .getRooms(filterData, this.roomsController.signal)
        .then(async (data) => {
          if (!folderId) setSelectedNode([data.current.id + ""]);

          filterData.total = data.total;

          if (data.total > 0) {
            const lastPage = filterData.getLastPage();

            if (filterData.page > lastPage) {
              filterData.page = lastPage;

              return this.fetchRooms(folderId, filterData);
            }
          }

          runInAction(() => {
            this.categoryType = getCategoryTypeByFolderType(
              data.current.rootFolderType,
              data.current.parentId
            );
          });

          this.setRoomsFilter(filterData);

          runInAction(() => {
            const isEmptyList = data.folders.length === 0;
            if (filter && isEmptyList) {
              const {
                subjectId,
                filterValue,
                type,
                withSubfolders: withRoomsSubfolders,
                searchInContent: searchInContentRooms,
                tags,
                withoutTags,
              } = filter;

              const isFiltered =
                subjectId ||
                filterValue ||
                type ||
                withRoomsSubfolders ||
                searchInContentRooms ||
                tags ||
                withoutTags;

              if (isFiltered) {
                this.setIsEmptyPage(false);
              } else {
                this.setIsEmptyPage(isEmptyList);
              }
            } else {
              this.setIsEmptyPage(isEmptyList);
            }

            this.setFolders(data.folders);
            this.setFiles([]);
          });

          if (clearFilter) {
            if (clearSelection) {
              this.setSelected("close");
            }
          }

          this.selectedFolderStore.setSelectedFolder({
            folders: data.folders,
            ...data.current,
            pathParts: data.pathParts,
            navigationPath: [],
            ...{ new: data.new },
          });

          const selectedFolder = {
            selectedFolder: { ...this.selectedFolderStore },
          };

          if (this.createdItem) {
            const newItem = this.filesList.find(
              (item) => item.id === this.createdItem.id
            );

            if (newItem) {
              this.setBufferSelection(newItem);
              this.setScrollToItem({
                id: newItem.id,
                type: this.createdItem.type,
              });
            }

            this.setCreatedItem(null);
          }
          this.setIsErrorRoomNotAvailable(false);
          return Promise.resolve(selectedFolder);
        })
        .catch((err) => {
          if (err?.response?.status === 402)
            this.authStore.currentTariffStatusStore.setPortalTariff();

          if (axios.isCancel(err)) {
            console.log("Request canceled", err.message);
          } else {
            toastr.error(err);
          }
        });

    return request();
  };

  setAlreadyFetchingRooms = (alreadyFetchingRooms) => {
    this.alreadyFetchingRooms = alreadyFetchingRooms;
  };

  isFileSelected = (fileId, parentId) => {
    const item = this.selection.find(
      (x) => x.id === fileId && x.parentId === parentId
    );

    return item !== undefined;
  };

  selectFile = (file) => {
    const { id, parentId } = file;
    const isFileSelected = this.isFileSelected(id, parentId);
    if (!isFileSelected) this.selection.push(file);
  };

  deselectFile = (file) => {
    const { id, parentId } = file;
    const isFileSelected = this.isFileSelected(id, parentId);
    if (isFileSelected) {
      let selectionIndex = this.selection.findIndex(
        (x) => x.parentId === parentId && x.id === id
      );

      if (selectionIndex !== -1) {
        this.selection = this.selection.filter(
          (x, index) => index !== selectionIndex
        );
      }
    }
  };

  removeOptions = (options, toRemoveArray) =>
    options.filter((o) => !toRemoveArray.includes(o));

  removeSeparator = (options) => {
    const newOptions = options.map((o, index) => {
      if (index === 0 && o.includes("separator")) {
        return false;
      }

      if (index === options.length - 1 && o.includes("separator")) {
        return false;
      }

      if (
        o?.includes("separator") &&
        options[index + 1].includes("separator")
      ) {
        return false;
      }

      return o;
    });

    return newOptions.filter((o) => o);
  };

  getFilesContextOptions = (item) => {
    const isFile = !!item.fileExst || item.contentLength;
    const isRoom = !!item.roomType;
    const isFavorite =
      (item.fileStatus & FileStatus.IsFavorite) === FileStatus.IsFavorite;

    const isThirdPartyItem = !!item.providerKey;
    const hasNew =
      item.new > 0 || (item.fileStatus & FileStatus.IsNew) === FileStatus.IsNew;
    const canConvert = this.filesSettingsStore.extsConvertible[item.fileExst];
    const isEncrypted = item.encrypted;
    const isDocuSign = false; //TODO: need this prop;
    const isEditing = false; // (item.fileStatus & FileStatus.IsEditing) === FileStatus.IsEditing;

    const { isRecycleBinFolder, isMy, isArchiveFolder } = this.treeFoldersStore;

    const { enablePlugins } = this.authStore.settingsStore;

    const isThirdPartyFolder =
      item.providerKey && item.id === item.rootFolderId;

    const isMyFolder = isMy(item.rootFolderType);

    const { isDesktopClient } = this.authStore.settingsStore;

    const pluginAllKeys =
      enablePlugins && getContextMenuKeysByType(PluginContextMenuItemType.All);

    const canRenameItem = item.security?.Rename;

    const canMove = this.accessRightsStore.canMoveItems({
      ...item,
      ...{ editing: isEditing },
    });

    const canDelete = !isEditing && item.security?.Delete;

    const canCopy = item.security?.Copy;
    const canDuplicate = item.security?.Duplicate;

    if (isFile) {
      const shouldFillForm = item.viewAccessability.WebRestrictedEditing;
      const canLockFile = item.security?.Lock;
      const canChangeVersionFileHistory =
        !isEditing && item.security?.EditHistory;

      const canViewVersionFileHistory = item.security?.ReadHistory;
      const canFillForm = item.security?.FillForms;

      const canEditFile = item.security.Edit && item.viewAccessability.WebEdit;
      const canOpenPlayer =
        item.viewAccessability.ImageView || item.viewAccessability.MediaView;
      const canViewFile = item.viewAccessability.WebView;

      const isMasterForm = item.fileExst === ".docxf";

      let fileOptions = [
        //"open",
        "select",
        "fill-form",
        "edit",
        "preview",
        "view",
        "make-form",
        "separator0",
        "link-for-room-members",
        // "sharing-settings",
        // "external-link",
        "owner-change",
        // "link-for-portal-users",
        "send-by-email",
        "docu-sign",
        "version", //category
        "finalize-version",
        "show-version-history",
        "show-info",
        "block-unblock-version", //need split
        "separator1",
        "open-location",
        "mark-read",
        // "mark-as-favorite",
        // "remove-from-favorites",
        "download",
        "download-as",
        "convert",
        "move", //category
        "move-to",
        "copy-to",
        "copy",
        "restore",
        "rename",
        "separator2",
        // "unsubscribe",
        "delete",
      ];

      if (!canLockFile) {
        fileOptions = this.removeOptions(fileOptions, [
          "block-unblock-version",
        ]);
      }

      if (!canChangeVersionFileHistory) {
        fileOptions = this.removeOptions(fileOptions, ["finalize-version"]);
      }

      if (!canViewVersionFileHistory) {
        fileOptions = this.removeOptions(fileOptions, ["show-version-history"]);
      }

      if (!canChangeVersionFileHistory && !canViewVersionFileHistory) {
        fileOptions = this.removeOptions(fileOptions, ["version"]);
        if (item.rootFolderType === FolderType.Archive) {
          fileOptions = this.removeOptions(fileOptions, ["separator0"]);
        }
      }

      if (!canRenameItem) {
        fileOptions = this.removeOptions(fileOptions, ["rename"]);
      }

      if (canOpenPlayer || !canEditFile) {
        fileOptions = this.removeOptions(fileOptions, ["edit"]);
      }

      if (!(shouldFillForm && canFillForm)) {
        fileOptions = this.removeOptions(fileOptions, ["fill-form"]);
      }

      if (!canDelete) {
        fileOptions = this.removeOptions(fileOptions, ["delete"]);
      }

      if (!canMove) {
        fileOptions = this.removeOptions(fileOptions, ["move-to"]);
      }

      if (!canCopy) {
        fileOptions = this.removeOptions(fileOptions, ["copy-to"]);
      }

      if (!canDuplicate) {
        fileOptions = this.removeOptions(fileOptions, ["copy"]);
      }
      if (!canMove && !canCopy && !canDuplicate) {
        fileOptions = this.removeOptions(fileOptions, ["move"]);
      }

      if (!(isMasterForm && canDuplicate))
        fileOptions = this.removeOptions(fileOptions, ["make-form"]);

      if (item.rootFolderType === FolderType.Archive) {
        fileOptions = this.removeOptions(fileOptions, [
          "mark-read",
          "mark-as-favorite",
          "remove-from-favorites",
        ]);
      }

      if (!canConvert) {
        fileOptions = this.removeOptions(fileOptions, ["download-as"]);
      }

      if (!canConvert || isEncrypted) {
        fileOptions = this.removeOptions(fileOptions, ["convert"]);
      }

      if (!canViewFile || isRecycleBinFolder) {
        fileOptions = this.removeOptions(fileOptions, ["preview"]);
      }

      if (!canOpenPlayer || isRecycleBinFolder) {
        fileOptions = this.removeOptions(fileOptions, ["view"]);
      }

      if (!isDocuSign) {
        fileOptions = this.removeOptions(fileOptions, ["docu-sign"]);
      }

      if (
        isEditing ||
        item.rootFolderType === FolderType.Archive
        // ||
        // (isFavoritesFolder && !isFavorite) ||
        // isFavoritesFolder ||
        // isRecentFolder
      )
        fileOptions = this.removeOptions(fileOptions, ["separator2"]);

      // if (isFavorite) {
      //   fileOptions = this.removeOptions(fileOptions, ["mark-as-favorite"]);
      // } else {
      //   fileOptions = this.removeOptions(fileOptions, [
      //     "remove-from-favorites",
      //   ]);

      //   if (isFavoritesFolder) {
      //     fileOptions = this.removeOptions(fileOptions, ["mark-as-favorite"]);
      //   }
      // }

      if (isEncrypted) {
        fileOptions = this.removeOptions(fileOptions, [
          "open",
          "link-for-room-members",
          // "link-for-portal-users",
          // "external-link",
          "send-by-email",
          "mark-as-favorite",
        ]);
      }

      // if (isFavoritesFolder || isRecentFolder) {
      //   fileOptions = this.removeOptions(fileOptions, [
      //     //"unsubscribe",
      //   ]);
      // }

      if (!isRecycleBinFolder)
        fileOptions = this.removeOptions(fileOptions, ["restore"]);

      if (enablePlugins && !isRecycleBinFolder) {
        const pluginFilesKeys = getContextMenuKeysByType(
          PluginContextMenuItemType.Files
        );

        pluginAllKeys && pluginAllKeys.forEach((key) => fileOptions.push(key));
        pluginFilesKeys &&
          pluginFilesKeys.forEach((key) => fileOptions.push(key));
      }

      if (!this.canShareOwnerChange(item)) {
        fileOptions = this.removeOptions(fileOptions, ["owner-change"]);
      }

      if (isThirdPartyItem) {
        fileOptions = this.removeOptions(fileOptions, ["owner-change"]);
      }

      if (!hasNew) {
        fileOptions = this.removeOptions(fileOptions, ["mark-read"]);
      }

      if (
        !(
          // isRecentFolder ||
          // isFavoritesFolder ||
          (isMyFolder && (this.filterType || this.filterSearch))
        )
      ) {
        fileOptions = this.removeOptions(fileOptions, ["open-location"]);
      }

      if (isMyFolder || isRecycleBinFolder) {
        fileOptions = this.removeOptions(fileOptions, [
          "link-for-room-members",
        ]);
      }

      // if (isPrivacyFolder) {
      //   fileOptions = this.removeOptions(fileOptions, [
      //     "preview",
      //     "view",
      //     "separator0",
      //     "download-as",
      //   ]);

      //   // if (!isDesktopClient) {
      //   //   fileOptions = this.removeOptions(fileOptions, ["sharing-settings"]);
      //   // }
      // }

      fileOptions = this.removeSeparator(fileOptions);

      return fileOptions;
    } else if (isRoom) {
      const canInviteUserInRoom = item.security?.EditAccess;
      const canRemoveRoom = item.security?.Delete;

      const canArchiveRoom = item.security?.Move;
      const canPinRoom = item.security?.Pin;

      const canEditRoom = item.security?.EditRoom;

      const canViewRoomInfo = item.security?.Read;
      const canMuteRoom = item.security?.Mute;

      let roomOptions = [
        "select",
        "open",
        "separator0",
        "link-for-room-members",
        "reconnect-storage",
        "edit-room",
        "invite-users-to-room",
        "room-info",
        "pin-room",
        "unpin-room",
        "mute-room",
        "unmute-room",
        "separator1",
        "archive-room",
        "unarchive-room",
        "delete",
      ];

      if (!canEditRoom) {
        roomOptions = this.removeOptions(roomOptions, [
          "edit-room",
          "reconnect-storage",
        ]);
      }

      if (!canInviteUserInRoom) {
        roomOptions = this.removeOptions(roomOptions, ["invite-users-to-room"]);
      }

      if (!canArchiveRoom) {
        roomOptions = this.removeOptions(roomOptions, [
          "archive-room",
          "unarchive-room",
        ]);
      }

      if (!canRemoveRoom) {
        roomOptions = this.removeOptions(roomOptions, ["delete"]);
      }

      if (!canArchiveRoom && !canRemoveRoom) {
        roomOptions = this.removeOptions(roomOptions, ["separator1"]);
      }

      if (!item.providerKey) {
        roomOptions = this.removeOptions(roomOptions, ["reconnect-storage"]);
      }

      if (!canPinRoom) {
        roomOptions = this.removeOptions(roomOptions, [
          "unpin-room",
          "pin-room",
        ]);
      } else {
        item.pinned
          ? (roomOptions = this.removeOptions(roomOptions, ["pin-room"]))
          : (roomOptions = this.removeOptions(roomOptions, ["unpin-room"]));
      }

      if (!canMuteRoom) {
        roomOptions = this.removeOptions(roomOptions, [
          "unmute-room",
          "mute-room",
        ]);
      } else {
        item.mute
          ? (roomOptions = this.removeOptions(roomOptions, ["mute-room"]))
          : (roomOptions = this.removeOptions(roomOptions, ["unmute-room"]));
      }

      if (!canViewRoomInfo) {
        roomOptions = this.removeOptions(roomOptions, ["room-info"]);
      }

      if (isArchiveFolder || item.rootFolderType === FolderType.Archive) {
        roomOptions = this.removeOptions(roomOptions, ["archive-room"]);
      } else {
        roomOptions = this.removeOptions(roomOptions, ["unarchive-room"]);

        if (enablePlugins) {
          const pluginRoomsKeys = getContextMenuKeysByType(
            PluginContextMenuItemType.Rooms
          );

          pluginAllKeys &&
            pluginAllKeys.forEach((key) => roomOptions.push(key));
          pluginRoomsKeys &&
            pluginRoomsKeys.forEach((key) => roomOptions.push(key));
        }
      }

      roomOptions = this.removeSeparator(roomOptions);

      return roomOptions;
    } else {
      let folderOptions = [
        "select",
        "open",
        // "separator0",
        // "sharing-settings",
        "link-for-room-members",
        "owner-change",
        "show-info",
        // "link-for-portal-users",
        "separator1",
        "open-location",
        "download",
        "move", //category
        "move-to",
        "copy-to",
        "mark-read",
        "restore",
        "rename",
        // "change-thirdparty-info",
        "separator2",
        // "unsubscribe",
        "delete",
      ];

      if (!canRenameItem) {
        folderOptions = this.removeOptions(folderOptions, ["rename"]);
      }

      if (!canDelete) {
        folderOptions = this.removeOptions(folderOptions, ["delete"]);
      }
      if (!canMove) {
        folderOptions = this.removeOptions(folderOptions, ["move-to"]);
      }

      if (!canCopy) {
        folderOptions = this.removeOptions(folderOptions, ["copy-to"]);
      }

      if (!canDuplicate) {
        folderOptions = this.removeOptions(folderOptions, ["copy"]);
      }

      if (!canMove && !canCopy && !canDuplicate) {
        folderOptions = this.removeOptions(folderOptions, ["move"]);
      }

      // if (item.rootFolderType === FolderType.Archive) {
      //   folderOptions = this.removeOptions(folderOptions, [
      //     "change-thirdparty-info",
      //     "separator2",
      //   ]);
      // }

      // if (isPrivacyFolder) {
      //   folderOptions = this.removeOptions(folderOptions, [
      //     // "sharing-settings",
      //   ]);
      // }

      if (isRecycleBinFolder) {
        folderOptions = this.removeOptions(folderOptions, [
          "open",
          "link-for-room-members",
          // "link-for-portal-users",
          // "sharing-settings",
          "mark-read",
          "separator0",
          "separator1",
        ]);
      } else {
        folderOptions = this.removeOptions(folderOptions, ["restore"]);

        if (enablePlugins) {
          const pluginFoldersKeys = getContextMenuKeysByType(
            PluginContextMenuItemType.Folders
          );

          pluginAllKeys &&
            pluginAllKeys.forEach((key) => folderOptions.push(key));
          pluginFoldersKeys &&
            pluginFoldersKeys.forEach((key) => folderOptions.push(key));
        }
      }

      if (!this.canShareOwnerChange(item)) {
        folderOptions = this.removeOptions(folderOptions, ["owner-change"]);
      }

      if (!hasNew) {
        folderOptions = this.removeOptions(folderOptions, ["mark-read"]);
      }

      if (isThirdPartyFolder && isDesktopClient)
        folderOptions = this.removeOptions(folderOptions, ["separator2"]);

      // if (!isThirdPartyFolder)
      //   folderOptions = this.removeOptions(folderOptions, [
      //     "change-thirdparty-info",
      //   ]);

      // if (isThirdPartyItem) {
      //   folderOptions = this.removeOptions(folderOptions, ["owner-change"]);

      //   if (isShareFolder) {
      //     folderOptions = this.removeOptions(folderOptions, [
      //       "change-thirdparty-info",
      //     ]);
      //   } else {
      //     if (isDesktopClient) {
      //       folderOptions = this.removeOptions(folderOptions, [
      //         "change-thirdparty-info",
      //       ]);
      //     }

      //     folderOptions = this.removeOptions(folderOptions, ["remove"]);

      //     if (!item) {
      //       //For damaged items
      //       folderOptions = this.removeOptions(folderOptions, [
      //         "open",
      //         "download",
      //       ]);
      //     }
      //   }
      // } else {
      //   folderOptions = this.removeOptions(folderOptions, [
      //     "change-thirdparty-info",
      //   ]);
      // }

      if (!(isMyFolder && (this.filterType || this.filterSearch))) {
        folderOptions = this.removeOptions(folderOptions, ["open-location"]);
      }

      if (isMyFolder) {
        folderOptions = this.removeOptions(folderOptions, [
          "link-for-room-members",
        ]);
      }

      folderOptions = this.removeSeparator(folderOptions);

      return folderOptions;
    }
  };

  addFileToRecentlyViewed = (fileId) => {
    if (this.treeFoldersStore.isPrivacyFolder) return Promise.resolve();
    return api.files.addFileToRecentlyViewed(fileId);
  };

  createFile = (folderId, title, templateId, formId) => {
    return api.files
      .createFile(folderId, title, templateId, formId)
      .then((file) => {
        return Promise.resolve(file);
      });
  };

  createFolder(parentFolderId, title) {
    return api.files.createFolder(parentFolderId, title);
  }

  createRoom = (roomParams) => {
    this.roomCreated = true;
    return api.rooms.createRoom(roomParams);
  };

  createRoomInThirdpary(thirpartyFolderId, roomParams) {
    return api.rooms.createRoomInThirdpary(thirpartyFolderId, roomParams);
  }

  editRoom(id, roomParams) {
    return api.rooms.editRoom(id, roomParams);
  }

  addTagsToRoom(id, tagArray) {
    return api.rooms.addTagsToRoom(id, tagArray);
  }

  removeTagsFromRoom(id, tagArray) {
    return api.rooms.removeTagsFromRoom(id, tagArray);
  }

  calculateRoomLogoParams(img, x, y, zoom) {
    let imgWidth, imgHeight, dimensions;
    if (img.width > img.height) {
      imgWidth = Math.min(1280, img.width);
      imgHeight = Math.round(img.height / (img.width / imgWidth));
      dimensions = Math.round(imgHeight / zoom);
    } else {
      imgHeight = Math.min(1280, img.height);
      imgWidth = Math.round(img.width / (img.height / imgHeight));
      dimensions = Math.round(imgWidth / zoom);
    }

    const croppedX = Math.round(x * imgWidth - dimensions / 2);
    const croppedY = Math.round(y * imgHeight - dimensions / 2);

    return {
      x: croppedX,
      y: croppedY,
      width: dimensions,
      height: dimensions,
    };
  }

  uploadRoomLogo(formData) {
    return api.rooms.uploadRoomLogo(formData);
  }

  addLogoToRoom(id, icon) {
    return api.rooms.addLogoToRoom(id, icon);
  }

  removeLogoFromRoom(id) {
    return api.rooms.removeLogoFromRoom(id);
  }

  getRoomMembers(id) {
    return api.rooms.getRoomMembers(id);
  }

  updateRoomMemberRole(id, data) {
    return api.rooms.updateRoomMemberRole(id, data);
  }

  getHistory(module, id) {
    return api.rooms.getHistory(module, id);
  }

  getRoomHistory(id) {
    return api.rooms.getRoomHistory(id);
  }

  getFileHistory(id) {
    return api.rooms.getFileHistory(id);
  }

  // updateFolderBadge = (id, count) => {
  //   const folder = this.folders.find((x) => x.id === id);
  //   if (folder) folder.new -= count;
  // };

  // updateFileBadge = (id) => {
  //   const file = this.files.find((x) => x.id === id);
  //   if (file) file.fileStatus = file.fileStatus & ~FileStatus.IsEditing;
  // };

  // updateFilesBadge = () => {
  //   for (let file of this.files) {
  //     file.fileStatus = file.fileStatus & ~FileStatus.IsEditing;
  //   }
  // };

  // updateFoldersBadge = () => {
  //   for (let folder of this.folders) {
  //     folder.new = 0;
  //   }
  // };

  updateRoomPin = (item) => {
    const idx = this.folders.findIndex((folder) => folder.id === item);

    if (idx === -1) return;
    this.folders[idx].pinned = !this.folders[idx].pinned;
  };

  scrollToTop = () => {
    if (this.authStore.settingsStore.withPaging) return;

    const scrollElm = isMobileOnly
      ? document.querySelector("#customScrollBar > .scroll-body")
      : document.querySelector("#sectionScroll > .scroll-body");

    scrollElm && scrollElm.scrollTo(0, 0);
  };

  addItem = (item, isFolder) => {
    const { socketHelper } = this.authStore.settingsStore;

    if (isFolder) {
      const foundIndex = this.folders.findIndex((x) => x.id === item?.id);
      if (foundIndex > -1) return;

      this.folders.unshift(item);

      console.log("[WS] subscribe to folder changes", item.id, item.title);

      socketHelper.emit({
        command: "subscribe",
        data: {
          roomParts: `DIR-${item.id}`,
          individual: true,
        },
      });
    } else {
      const foundIndex = this.files.findIndex((x) => x.id === item?.id);
      if (foundIndex > -1) return;

      console.log("[WS] subscribe to file changes", item.id, item.title);

      socketHelper.emit({
        command: "subscribe",
        data: { roomParts: `FILE-${item.id}`, individual: true },
      });

      this.files.unshift(item);
    }
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    const filter = isRooms ? this.roomsFilter.clone() : this.filter.clone();

    filter.total += 1;

    if (isRooms) this.setRoomsFilter(filter);
    else this.setFilter(filter);

    this.scrollToTop();
  };

  removeFiles = (fileIds, folderIds, showToast) => {
    const newFilter = this.filter.clone();
    const deleteCount = (fileIds?.length ?? 0) + (folderIds?.length ?? 0);

    if (newFilter.total <= newFilter.pageCount) {
      const files = fileIds
        ? this.files.filter((x) => !fileIds.includes(x.id))
        : this.files;
      const folders = folderIds
        ? this.folders.filter((x) => !folderIds.includes(x.id))
        : this.folders;

      newFilter.total -= deleteCount;

      runInAction(() => {
        this.setFilter(newFilter);
        this.setFiles(files);
        this.setFolders(folders);
        this.setTempActionFilesIds([]);
      });

      return;
    }

    newFilter.startIndex =
      (newFilter.page + 1) * newFilter.pageCount - deleteCount;
    newFilter.pageCount = deleteCount;

    api.files
      .getFolder(newFilter.folder, newFilter)
      .then((res) => {
        const files = fileIds
          ? this.files.filter((x) => !fileIds.includes(x.id))
          : this.files;
        const folders = folderIds
          ? this.folders.filter((x) => !folderIds.includes(x.id))
          : this.folders;

        const newFiles = [...files, ...res.files];
        const newFolders = [...folders, ...res.folders];

        const filter = this.filter.clone();
        filter.total = res.total;

        runInAction(() => {
          this.setFilter(filter);
          this.setFiles(newFiles);
          this.setFolders(newFolders);
        });

        showToast && showToast();
      })
      .catch((err) => {
        toastr.error(err);
        console.log("Need page reload");
      })
      .finally(() => {
        this.setOperationAction(false);
        this.setTempActionFilesIds([]);
      });
  };

  updateFile = (fileId, title) => {
    return api.files
      .updateFile(fileId, title)
      .then((file) => this.setFile(file));
  };

  renameFolder = (folderId, title) => {
    return api.files.renameFolder(folderId, title).then((folder) => {
      this.setFolder(folder);
    });
  };

  getFilesCount = () => {
    const { filesCount, foldersCount } = this.selectedFolderStore;
    return filesCount + this.folders ? this.folders.length : foldersCount;
  };

  getServiceFilesCount = () => {
    const filesLength = this.files ? this.files.length : 0;
    const foldersLength = this.folders ? this.folders.length : 0;
    return filesLength + foldersLength;
  };

  canShareOwnerChange = (item) => {
    const userId =
      this.authStore.userStore.user && this.authStore.userStore.user.id;

    if (item.providerKey || !this.hasCommonFolder) {
      return false;
    } else if (this.authStore.isAdmin) {
      return true;
    } else if (item.createdBy.id === userId) {
      return true;
    } else {
      return false;
    }
  };

  get canShare() {
    const folderType = this.selectedFolderStore.rootFolderType;
    const isVisitor =
      (this.authStore.userStore.user &&
        this.authStore.userStore.user.isVisitor) ||
      false;

    if (isVisitor) {
      return false;
    }

    switch (folderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        return true;
      case FolderType.COMMON:
        return this.authStore.isAdmin;
      case FolderType.TRASH:
        return false;
      case FolderType.Favorites:
        return true; // false;
      case FolderType.Recent:
        return true; //false;
      case FolderType.Privacy:
        return true;
      default:
        return false;
    }
  }

  get currentFilesCount() {
    const serviceFilesCount = this.getServiceFilesCount();
    const filesCount = this.getFilesCount();
    return this.selectedFolderStore.providerItem
      ? serviceFilesCount
      : filesCount;
  }

  get iconOfDraggedFile() {
    const { getIcon } = this.filesSettingsStore;

    if (this.selection.length === 1) {
      return getIcon(
        24,
        this.selection[0].fileExst,
        this.selection[0].providerKey
      );
    }
    return null;
  }

  get isHeaderVisible() {
    return this.selection.length > 0;
  }

  get isHeaderIndeterminate() {
    const items = [...this.files, ...this.folders];
    return this.isHeaderVisible && this.selection.length
      ? this.selection.length < items.length
      : false;
  }

  get isHeaderChecked() {
    const items = [...this.files, ...this.folders];
    return this.isHeaderVisible && this.selection.length === items.length;
  }

  get hasCommonFolder() {
    return (
      this.treeFoldersStore.commonFolder &&
      this.selectedFolderStore.pathParts &&
      this.treeFoldersStore.commonFolder.id ===
        this.selectedFolderStore.pathParts[0]
    );
  }

  setFirsElemChecked = (checked) => {
    this.firstElemChecked = checked;
  };

  setHeaderBorder = (headerBorder) => {
    this.headerBorder = headerBorder;
  };

  get canCreate() {
    switch (this.selectedFolderStore.rootFolderType) {
      case FolderType.USER:
      case FolderType.Rooms:
        return true;
      case FolderType.SHARE:
        const canCreateInSharedFolder = this.selectedFolderStore.access === 1;
        return (
          !this.selectedFolderStore.isRootFolder && canCreateInSharedFolder
        );
      case FolderType.Privacy:
        return (
          this.authStore.settingsStore.isDesktopClient &&
          this.authStore.settingsStore.isEncryptionSupport
        );
      case FolderType.COMMON:
        return this.authStore.isAdmin;
      case FolderType.Archive:
      case FolderType.TRASH:
      default:
        return false;
    }
  }

  onCreateAddTempItem = (items) => {
    const { getFileIcon, getFolderIcon } = this.filesSettingsStore;
    const { extension, title } = this.fileActionStore;

    if (items.length && items[0].id === -1) return; //TODO: if change media collection from state remove this;

    const iconSize = this.viewAs === "table" ? 24 : 32;
    const icon = extension
      ? getFileIcon(`.${extension}`, iconSize)
      : getFolderIcon(null, iconSize);

    items.unshift({
      id: -1,
      title: title,
      parentId: this.selectedFolderStore.id,
      fileExst: extension,
      icon,
    });
  };

  get filterType() {
    return this.filter.filterType;
  }

  get filterSearch() {
    return this.filter.search;
  }

  getItemUrl = (id, isFolder, needConvert, canOpenPlayer) => {
    const proxyURL =
      window.DocSpaceConfig?.proxy?.url || window.location.origin;

    const url = getCategoryUrl(this.categoryType, id);

    if (canOpenPlayer) {
      return combineUrl(proxyURL, config.homepage, `${url}/#preview/${id}`);
    }

    if (isFolder) {
      const folderUrl = isFolder
        ? combineUrl(proxyURL, config.homepage, `${url}?folder=${id}`)
        : null;

      return folderUrl;
    } else {
      const url = combineUrl(
        proxyURL,
        config.homepage,
        `/doceditor?fileId=${id}${needConvert ? "&action=view" : ""}`
      );

      return url;
    }
  };

  get filesList() {
    const { getIcon } = this.filesSettingsStore;
    //return [...this.folders, ...this.files];

    const newFolders = [...this.folders];

    newFolders.sort((a, b) => {
      const firstValue = a.roomType ? 1 : 0;
      const secondValue = b.roomType ? 1 : 0;

      return secondValue - firstValue;
    });

    const items = [...newFolders, ...this.files];

    if (items.length > 0 && this.isEmptyPage) {
      this.setIsEmptyPage(false);
    }

    const newItem = items.map((item) => {
      const {
        access,
        autoDelete,
        originTitle,
        comment,
        contentLength,
        created,
        createdBy,
        encrypted,
        fileExst,
        filesCount,
        fileStatus,
        fileType,
        folderId,
        foldersCount,
        id,
        logo,
        locked,
        originId,
        originFolderId,
        originRoomId,
        originRoomTitle,
        parentId,
        pureContentLength,
        rootFolderType,
        rootFolderId,
        shared,
        title,
        updated,
        updatedBy,
        version,
        versionGroup,
        viewUrl,
        webUrl,
        providerKey,
        thumbnailUrl,
        thumbnailStatus,
        canShare,
        canEdit,
        roomType,
        isArchive,
        tags,
        pinned,
        security,
        viewAccessability,
        mute,
      } = item;

      const thirdPartyIcon = this.thirdPartyStore.getThirdPartyIcon(
        item.providerKey,
        "small"
      );

      const providerType =
        RoomsProviderType[
          Object.keys(RoomsProviderType).find((key) => key === item.providerKey)
        ];

      const canOpenPlayer =
        item.viewAccessability?.ImageView || item.viewAccessability?.MediaView;

      const previewUrl = canOpenPlayer
        ? this.getItemUrl(id, false, needConvert, canOpenPlayer)
        : null;
      const contextOptions = this.getFilesContextOptions(item);
      const isThirdPartyFolder = providerKey && id === rootFolderId;

      const iconSize = this.viewAs === "table" ? 24 : 32;

      let isFolder = false;
      this.folders.map((x) => {
        if (x.id === item.id && x.parentId === item.parentId) isFolder = true;
      });

      const { isRecycleBinFolder } = this.treeFoldersStore;

      const folderUrl = isFolder && this.getItemUrl(id, isFolder, false, false);

      const needConvert = item.viewAccessability?.Convert;
      const isEditing =
        (item.fileStatus & FileStatus.IsEditing) === FileStatus.IsEditing;

      const docUrl =
        !canOpenPlayer && !isFolder && this.getItemUrl(id, false, needConvert);

      const href = isRecycleBinFolder
        ? null
        : previewUrl
        ? previewUrl
        : !isFolder
        ? docUrl
        : folderUrl;

      const isRoom = !!roomType;

      const icon =
        isRoom && logo?.medium
          ? logo?.medium
          : getIcon(
              iconSize,
              fileExst,
              providerKey,
              contentLength,
              roomType,
              isArchive
            );

      const defaultRoomIcon = isRoom
        ? getIcon(
            iconSize,
            fileExst,
            providerKey,
            contentLength,
            roomType,
            isArchive
          )
        : undefined;

      return {
        access,
        daysRemaining: autoDelete && getDaysRemaining(autoDelete),
        originTitle,
        //checked,
        comment,
        contentLength,
        contextOptions,
        created,
        createdBy,
        encrypted,
        fileExst,
        filesCount,
        fileStatus,
        fileType,
        folderId,
        foldersCount,
        icon,
        defaultRoomIcon,
        id,
        isFolder,
        logo,
        locked,
        new: item.new,
        mute,
        parentId,
        pureContentLength,
        rootFolderType,
        rootFolderId,
        //selectedItem,
        shared,
        title,
        updated,
        updatedBy,
        version,
        versionGroup,
        viewUrl,
        webUrl,
        providerKey,
        canOpenPlayer,
        //canShare,
        canShare,
        canEdit,
        thumbnailUrl,
        thumbnailStatus,
        originId,
        originFolderId,
        originRoomId,
        originRoomTitle,
        previewUrl,
        folderUrl,
        href,
        isThirdPartyFolder,
        isEditing,
        roomType,
        isRoom,
        isArchive,
        tags,
        pinned,
        thirdPartyIcon,
        providerType,
        security,
        viewAccessability,
      };
    });

    return newItem;
  }

  get cbMenuItems() {
    const {
      isDocument,
      isPresentation,
      isSpreadsheet,
      isArchive,
    } = this.filesSettingsStore;

    let cbMenu = ["all"];
    const filesItems = [...this.files, ...this.folders];

    if (this.folders.length) {
      for (const item of this.folders) {
        if (item.roomType && RoomsTypeValues[item.roomType]) {
          cbMenu.push(`room-${RoomsTypeValues[item.roomType]}`);
        } else {
          cbMenu.push(FilterType.FoldersOnly);
        }
      }
    }

    for (let item of filesItems) {
      if (isDocument(item.fileExst)) cbMenu.push(FilterType.DocumentsOnly);
      else if (isPresentation(item.fileExst))
        cbMenu.push(FilterType.PresentationsOnly);
      else if (isSpreadsheet(item.fileExst))
        cbMenu.push(FilterType.SpreadsheetsOnly);
      else if (item.viewAccessability?.ImageView)
        cbMenu.push(FilterType.ImagesOnly);
      else if (item.viewAccessability?.MediaView)
        cbMenu.push(FilterType.MediaOnly);
      else if (isArchive(item.fileExst)) cbMenu.push(FilterType.ArchiveOnly);
    }

    const hasFiles = cbMenu.some(
      (elem) =>
        elem !== "all" &&
        elem !== `room-${FilterType.FoldersOnly}` &&
        elem !== `room-${RoomsType.FillingFormsRoom}` &&
        elem !== `room-${RoomsType.CustomRoom}` &&
        elem !== `room-${RoomsType.EditingRoom}` &&
        elem !== `room-${RoomsType.ReviewRoom}` &&
        elem !== `room-${RoomsType.ReadOnlyRoom}`
    );

    if (hasFiles) cbMenu.push(FilterType.FilesOnly);

    cbMenu = cbMenu.filter((item, index) => cbMenu.indexOf(item) === index);

    return cbMenu;
  }

  getCheckboxItemLabel = (t, key) => {
    switch (key) {
      case "all":
        return t("All");
      case FilterType.FoldersOnly:
        return t("Translations:Folders");
      case FilterType.DocumentsOnly:
        return t("Common:Documents");
      case FilterType.PresentationsOnly:
        return t("Translations:Presentations");
      case FilterType.SpreadsheetsOnly:
        return t("Translations:Spreadsheets");
      case FilterType.ImagesOnly:
        return t("Images");
      case FilterType.MediaOnly:
        return t("Media");
      case FilterType.ArchiveOnly:
        return t("Archives");
      case FilterType.FilesOnly:
        return t("AllFiles");
      case `room-${RoomsType.FillingFormsRoom}`:
        return t("FillingFormRooms");
      case `room-${RoomsType.CustomRoom}`:
        return t("CustomRooms");
      case `room-${RoomsType.EditingRoom}`:
        return t("CollaborationRooms");
      case `room-${RoomsType.ReviewRoom}`:
        return t("Common:Review");
      case `room-${RoomsType.ReadOnlyRoom}`:
        return t("ViewOnlyRooms");

      default:
        return "";
    }
  };

  getCheckboxItemId = (key) => {
    switch (key) {
      case "all":
        return "selected-all";
      case FilterType.FoldersOnly:
        return "selected-only-folders";
      case FilterType.DocumentsOnly:
        return "selected-only-documents";
      case FilterType.PresentationsOnly:
        return "selected-only-presentations";
      case FilterType.SpreadsheetsOnly:
        return "selected-only-spreadsheets";
      case FilterType.ImagesOnly:
        return "selected-only-images";
      case FilterType.MediaOnly:
        return "selected-only-media";
      case FilterType.ArchiveOnly:
        return "selected-only-archives";
      case FilterType.FilesOnly:
        return "selected-only-files";
      case `room-${RoomsType.FillingFormsRoom}`:
        return "selected-only-filling-form-rooms";
      case `room-${RoomsType.CustomRoom}`:
        return "selected-only-custom-room";
      case `room-${RoomsType.EditingRoom}`:
        return "selected-only-collaboration-rooms";
      case `room-${RoomsType.ReviewRoom}`:
        return "selected-only-review-rooms";
      case `room-${RoomsType.ReadOnlyRoom}`:
        return "selected-only-view-rooms";

      default:
        return "";
    }
  };

  get sortedFiles() {
    const {
      extsConvertible,
      isSpreadsheet,
      isPresentation,
      isDocument,
      isMasterFormExtension,
    } = this.filesSettingsStore;

    let sortedFiles = {
      documents: [],
      spreadsheets: [],
      presentations: [],
      masterForms: [],
      other: [],
    };

    let selection = this.selection.length
      ? this.selection
      : this.bufferSelection
      ? [this.bufferSelection]
      : [];

    selection = JSON.parse(JSON.stringify(selection));

    for (let item of selection) {
      item.checked = true;
      item.format = null;

      const canConvert = extsConvertible[item.fileExst];

      if (item.fileExst && canConvert) {
        if (isSpreadsheet(item.fileExst)) {
          sortedFiles.spreadsheets.push(item);
        } else if (isPresentation(item.fileExst)) {
          sortedFiles.presentations.push(item);
        } else if (isMasterFormExtension(item.fileExst)) {
          sortedFiles.masterForms.push(item);
        } else if (isDocument(item.fileExst)) {
          sortedFiles.documents.push(item);
        } else {
          sortedFiles.other.push(item);
        }
      } else {
        sortedFiles.other.push(item);
      }
    }

    return sortedFiles;
  }

  get userAccess() {
    switch (this.selectedFolderStore.rootFolderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        return false;
      case FolderType.COMMON:
        return (
          this.authStore.isAdmin ||
          this.selection.some((x) => x.access === 0 || x.access === 1)
        );
      case FolderType.Privacy:
        return true;
      case FolderType.TRASH:
        return true;
      default:
        return false;
    }
  }

  get isAccessedSelected() {
    return (
      this.selection.length &&
      this.selection.every((x) => x.access === 1 || x.access === 0)
    );
  }

  // get isThirdPartyRootSelection() {
  //   const withProvider = this.selection.find((x) => x.providerKey);
  //   return withProvider && withProvider.rootFolderId === withProvider.id;
  // }

  get isThirdPartySelection() {
    const withProvider = this.selection.find((x) => x.providerKey);
    return !!withProvider;
  }

  get canConvertSelected() {
    const { extsConvertible } = this.filesSettingsStore;

    const selection = this.selection.length
      ? this.selection
      : this.bufferSelection
      ? [this.bufferSelection]
      : [];

    return selection.some((selected) => {
      if (selected.isFolder === true || !selected.fileExst) return false;
      const array = extsConvertible[selected.fileExst];
      return array;
    });
  }

  get isViewedSelected() {
    return this.selection.some((selected) => {
      if (selected.isFolder === true || !selected.fileExst) return false;
      return selected.viewAccessability?.WebView;
    });
  }

  get isMediaSelected() {
    return this.selection.some((selected) => {
      if (selected.isFolder === true || !selected.fileExst) return false;
      return (
        selected.viewAccessability?.ImageView ||
        selected.viewAccessability?.MediaView
      );
    });
  }

  get selectionTitle() {
    if (this.selection.length === 0) {
      if (this.bufferSelection) {
        return this.bufferSelection.title;
      }
      return null;
    }
    return this.selection.find((el) => el.title).title;
  }

  get hasSelection() {
    return !!this.selection.length;
  }

  get isEmptyFilesList() {
    const filesList = [...this.files, ...this.folders];
    return filesList.length <= 0;
  }

  get hasNew() {
    const newFiles = [...this.files, ...this.folders].filter(
      (item) => (item.fileStatus & FileStatus.IsNew) === FileStatus.IsNew
    );
    return newFiles.length > 0;
  }

  get allFilesIsEditing() {
    const hasFolders = this.selection.find(
      (x) => !x.fileExst || !x.contentLength
    );
    if (!hasFolders) {
      return this.selection.every((x) => x.isEditing);
    }
    return false;
  }

  getOptions = (selection, externalAccess = false) => {
    if (selection[0].encrypted) {
      return ["FullAccess", "DenyAccess"];
    }

    let AccessOptions = [];

    AccessOptions.push("ReadOnly", "DenyAccess");

    const webEdit = selection.find((x) => x.viewAccessability?.WebEdit);

    const webComment = selection.find((x) => x.viewAccessability?.WebComment);

    const webReview = selection.find((x) => x.viewAccessability?.WebReview);

    const formFillingDocs = selection.find(
      (x) => x.viewAccessability?.WebRestrictedEditing
    );

    const webFilter = selection.find(
      (x) => x.viewAccessability?.WebCustomFilterEditing
    );

    const webNeedConvert = selection.find((x) => x.viewAccessability?.Convert);

    if ((webEdit && !webNeedConvert) || !externalAccess)
      AccessOptions.push("FullAccess");

    if (webComment) AccessOptions.push("Comment");
    if (webReview) AccessOptions.push("Review");
    if (formFillingDocs && !externalAccess) AccessOptions.push("FormFilling");
    if (webFilter) AccessOptions.push("FilterEditing");

    return AccessOptions;
  };

  getAccessOption = (selection) => {
    return this.getOptions(selection);
  };

  getExternalAccessOption = (selection) => {
    return this.getOptions(selection, true);
  };

  getShareUsers(folderIds, fileIds) {
    return api.files.getShareFiles(fileIds, folderIds);
  }

  setShareFiles = (
    folderIds,
    fileIds,
    share,
    notify,
    sharingMessage,
    externalAccess,
    ownerId
  ) => {
    let externalAccessRequest = [];
    if (fileIds.length === 1 && externalAccess !== null) {
      externalAccessRequest = fileIds.map((id) =>
        api.files.setExternalAccess(id, externalAccess)
      );
    }

    const ownerChangeRequest = ownerId
      ? [this.setFilesOwner(folderIds, fileIds, ownerId)]
      : [];

    const shareRequest = !!share.length
      ? [
          api.files.setShareFiles(
            fileIds,
            folderIds,
            share,
            notify,
            sharingMessage
          ),
        ]
      : [];

    const requests = [
      ...ownerChangeRequest,
      ...shareRequest,
      ...externalAccessRequest,
    ];

    return Promise.all(requests);
  };

  markItemAsFavorite = (id) => api.files.markAsFavorite(id);

  removeItemFromFavorite = (id) => api.files.removeFromFavorite(id);

  fetchFavoritesFolder = async (folderId) => {
    const favoritesFolder = await api.files.getFolder(folderId);
    this.setFolders(favoritesFolder.folders);
    this.setFiles(favoritesFolder.files);

    this.selectedFolderStore.setSelectedFolder({
      folders: favoritesFolder.folders,
      ...favoritesFolder.current,
      pathParts: favoritesFolder.pathParts,
    });
  };

  pinRoom = (id) => api.rooms.pinRoom(id);

  unpinRoom = (id) => api.rooms.unpinRoom(id);

  getFileInfo = async (id) => {
    const fileInfo = await api.files.getFileInfo(id);
    this.setFile(fileInfo);
    return fileInfo;
  };

  getFolderInfo = async (id) => {
    const folderInfo = await api.files.getFolderInfo(id);
    this.setFolder(folderInfo);
    return folderInfo;
  };

  openDocEditor = (
    id,
    providerKey = null,
    tab = null,
    url = null,
    preview = false
  ) => {
    const foundIndex = this.files.findIndex((x) => x.id === id);
    if (
      foundIndex !== -1 &&
      !preview &&
      this.files[foundIndex].rootFolderType !== FolderType.Archive
    ) {
      const newStatus =
        this.files[foundIndex].fileStatus | FileStatus.IsEditing;

      this.updateSelectionStatus(id, newStatus, true);
      this.updateFileStatus(foundIndex, newStatus);
    }

    const isPrivacy = this.treeFoldersStore.isPrivacyFolder;
    return openEditor(id, providerKey, tab, url, isPrivacy);
  };

  createThumbnails = async () => {
    if (this.viewAs !== "tile" || !this.files) return;

    const newFiles = this.files.filter((f) => {
      return (
        typeof f.id !== "string" &&
        f?.thumbnailStatus === thumbnailStatuses.WAITING &&
        !this.thumbnails.has(`${f.id}|${f.versionGroup}`)
      );
    });

    if (!newFiles.length) return;

    if (this.thumbnails.size > THUMBNAILS_CACHE) this.thumbnails.clear();

    newFiles.forEach((f) => this.thumbnails.add(`${f.id}|${f.versionGroup}`));

    console.log("thumbnails", this.thumbnails);

    const fileIds = newFiles.map((f) => f.id);

    const res = await api.files.createThumbnails(fileIds);

    return res;
  };

  createThumbnail = async (file) => {
    if (
      this.viewAs !== "tile" ||
      !file ||
      !file.id ||
      typeof file.id === "string" ||
      file.thumbnailStatus !== thumbnailStatuses.WAITING ||
      this.thumbnails.has(`${file.id}|${file.versionGroup}`)
    ) {
      return;
    }

    if (this.thumbnails.size > THUMBNAILS_CACHE) this.thumbnails.clear();

    this.thumbnails.add(`${file.id}|${file.versionGroup}`);

    console.log("thumbnails", this.thumbnails);

    const res = await api.files.createThumbnails([file.id]);

    return res;
  };

  setIsUpdatingRowItem = (updating) => {
    this.isUpdatingRowItem = updating;
  };

  setPasswordEntryProcess = (process) => {
    this.passwordEntryProcess = process;
  };

  setEnabledHotkeys = (enabledHotkeys) => {
    this.enabledHotkeys = enabledHotkeys;
  };

  setCreatedItem = (createdItem) => {
    this.createdItem = createdItem;

    // const { socketHelper } = this.authStore.settingsStore;
    // if (createdItem?.type == "file") {
    //   console.log(
    //     "[WS] subscribe to file's changes",
    //     createdItem.id,
    //     createdItem.title
    //   );

    //   socketHelper.emit({
    //     command: "subscribe",
    //     data: { roomParts: `FILE-${createdItem.id}`, individual: true },
    //   });
    // }
  };

  setScrollToItem = (item) => {
    this.scrollToItem = item;
  };

  getIsEmptyTrash = async () => {
    const res = await api.files.getTrashFolderList();
    const items = [...res.files, ...res.folders];
    this.setTrashIsEmpty(items.length === 0 ? true : false);
  };

  setTrashIsEmpty = (isEmpty) => {
    this.trashIsEmpty = isEmpty;
  };

  setMainButtonMobileVisible = (visible) => {
    this.mainButtonMobileVisible = visible;
  };

  get roomsFilterTotal() {
    return this.roomsFilter.total;
  }

  get filterTotal() {
    return this.filter.total;
  }

  get hasMoreFiles() {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;
    const filterTotal = isRooms ? this.roomsFilter.total : this.filter.total;

    if (this.isLoading) return false;
    return this.filesList.length < filterTotal;
  }

  setFilesIsLoading = (filesIsLoading) => {
    this.filesIsLoading = filesIsLoading;
  };

  fetchMoreFiles = async () => {
    if (!this.hasMoreFiles || this.filesIsLoading || this.isLoading) return;

    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    this.setFilesIsLoading(true);
    // console.log("fetchMoreFiles");

    const newFilter = isRooms ? this.roomsFilter.clone() : this.filter.clone();
    newFilter.page += 1;
    if (isRooms) this.setRoomsFilter(newFilter);
    else this.setFilter(newFilter);

    const newFiles = isRooms
      ? await api.rooms.getRooms(newFilter)
      : await api.files.getFolder(newFilter.folder, newFilter);

    runInAction(() => {
      this.setFiles([...this.files, ...newFiles.files]);
      this.setFolders([...this.folders, ...newFiles.folders]);
      this.setFilesIsLoading(false);
    });
  };

  //Duplicate of countTilesInRow, used to update the number of tiles in a row after the window is resized.
  getCountTilesInRow = () => {
    const isDesktopView = isDesktop();
    const tileGap = isDesktopView ? 16 : 14;
    const minTileWidth = 216 + tileGap;
    const sectionPadding = isDesktopView ? 24 : 16;

    const body = document.getElementById("section");
    const sectionWidth = body ? body.offsetWidth - sectionPadding : 0;

    return Math.floor(sectionWidth / minTileWidth);
  };

  setInvitationLinks = async (roomId, linkId, title, access) => {
    return await api.rooms.setInvitationLinks(roomId, linkId, title, access);
  };

  resendEmailInvitations = async (id, usersIds) => {
    return await api.rooms.resendEmailInvitations(id, usersIds);
  };

  getRoomSecurityInfo = async (id) => {
    return await api.rooms.getRoomSecurityInfo(id);
  };

  setRoomSecurity = async (id, data) => {
    return await api.rooms.setRoomSecurity(id, data);
  };

  withCtrlSelect = (item) => {
    this.setHotkeyCaret(item);
    this.setHotkeyCaretStart(item);

    const fileIndex = this.selection.findIndex(
      (f) => f.id === item.id && f.isFolder === item.isFolder
    );
    if (fileIndex === -1) {
      this.setSelection([...this.selection, item]);
    } else {
      this.deselectFile(item);
    }
  };

  withShiftSelect = (item) => {
    const caretStart = this.hotkeyCaretStart
      ? this.hotkeyCaretStart
      : this.filesList[0];
    const caret = this.hotkeyCaret ? this.hotkeyCaret : caretStart;

    if (!caret || !caretStart) return;

    const startCaretIndex = this.filesList.findIndex(
      (f) => f.id === caretStart.id && f.isFolder === caretStart.isFolder
    );

    const caretIndex = this.filesList.findIndex(
      (f) => f.id === caret.id && f.isFolder === caret.isFolder
    );

    const itemIndex = this.filesList.findIndex(
      (f) => f.id === item.id && f.isFolder === item.isFolder
    );

    const isMoveDown = caretIndex < itemIndex;

    let newSelection = JSON.parse(JSON.stringify(this.selection));
    let index = caretIndex;
    const newItemIndex = isMoveDown ? itemIndex + 1 : itemIndex - 1;

    while (index !== newItemIndex) {
      const filesItem = this.filesList[index];

      const selectionIndex = newSelection.findIndex(
        (f) => f.id === filesItem.id && f.isFolder === filesItem.isFolder
      );
      if (selectionIndex === -1) {
        newSelection.push(filesItem);
      } else {
        newSelection = newSelection.filter(
          (_, fIndex) => selectionIndex !== fIndex
        );
        newSelection.push(filesItem);
      }

      if (isMoveDown) {
        index++;
      } else {
        index--;
      }
    }

    const lastSelection = this.selection[this.selection.length - 1];
    const indexOfLast = this.filesList.findIndex(
      (f) =>
        f.id === lastSelection?.id && f.isFolder === lastSelection?.isFolder
    );

    newSelection = newSelection.filter((f) => {
      const listIndex = this.filesList.findIndex(
        (x) => x.id === f.id && x.isFolder === f.isFolder
      );

      if (isMoveDown) {
        const isSelect = listIndex < indexOfLast;
        if (isSelect) return true;

        if (listIndex >= startCaretIndex) {
          return true;
        } else {
          return listIndex >= itemIndex;
        }
      } else {
        const isSelect = listIndex > indexOfLast;
        if (isSelect) return true;

        if (listIndex <= startCaretIndex) {
          return true;
        } else {
          return listIndex <= itemIndex;
        }
      }
    });

    this.setSelection(newSelection);
    this.setHotkeyCaret(item);
  };

  get disableDrag() {
    const {
      isRecycleBinFolder,
      isRoomsFolder,
      isArchiveFolder,
      isFavoritesFolder,
      isRecentFolder,
    } = this.treeFoldersStore;

    return (
      isRecycleBinFolder ||
      isRoomsFolder ||
      isArchiveFolder ||
      isFavoritesFolder ||
      isRecentFolder
    );
  }

  get roomsForRestore() {
    return this.folders.filter((f) => f.security.Move);
  }

  get roomsForDelete() {
    return this.folders.filter((f) => f.security.Delete);
  }

  get isFiltered() {
    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;

    const {
      subjectId,
      filterValue,
      type,
      withSubfolders: withRoomsSubfolders,
      searchInContent: searchInContentRooms,
      tags,
      withoutTags,
    } = this.roomsFilter;

    const { authorType, search, withSubfolders, filterType, searchInContent } =
      this.filter;

    const isFiltered =
      isRoomsFolder || isArchiveFolder
        ? filterValue ||
          type ||
          withRoomsSubfolders ||
          searchInContentRooms ||
          subjectId ||
          tags ||
          withoutTags
        : authorType ||
          search ||
          !withSubfolders ||
          filterType ||
          searchInContent;

    return isFiltered;
  }
}

export default FilesStore;
