import { makeAutoObservable } from "mobx";
import api from "@appserver/common/api";
import {
  FolderType,
  FilterType,
  FileType,
  FileAction,
  AppServerConfig,
  FileStatus,
} from "@appserver/common/constants";
import history from "@appserver/common/history";
import { loopTreeFolders } from "../helpers/files-helpers";
import config from "../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { updateTempContent } from "@appserver/common/utils";
import { thumbnailStatuses } from "../helpers/constants";
import { isMobile } from "react-device-detect";
import { openDocEditor as openEditor } from "../helpers/utils";
import toastr from "studio/toastr";

const { FilesFilter } = api;
const storageViewAs = localStorage.getItem("viewAs");

class FilesStore {
  authStore;
  settingsStore;
  userStore;
  fileActionStore;
  selectedFolderStore;
  treeFoldersStore;
  filesSettingsStore;

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
  files = [];
  folders = [];
  selection = [];
  bufferSelection = null;
  selected = "close";
  filter = FilesFilter.getDefault(); //TODO: FILTER
  loadTimeout = null;
  activeFiles = [];
  activeFolders = [];

  firstElemChecked = false;

  isPrevSettingsModule = false;

  constructor(
    authStore,
    settingsStore,
    userStore,
    fileActionStore,
    selectedFolderStore,
    treeFoldersStore,
    filesSettingsStore
  ) {
    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;

    makeAutoObservable(this);
    this.authStore = authStore;
    this.settingsStore = settingsStore;
    this.userStore = userStore;
    this.fileActionStore = fileActionStore;
    this.selectedFolderStore = selectedFolderStore;
    this.treeFoldersStore = treeFoldersStore;
    this.filesSettingsStore = filesSettingsStore;

    const { socketHelper } = authStore.settingsStore;

    socketHelper.on("s:modify-folder", async (opt) => {
      //console.log("Call s:modify-folder", opt);

      if (this.isLoading) return;

      switch (opt?.cmd) {
        case "create":
          if (opt?.type == "file" && opt?.id) {
            const foundIndex = this.files.findIndex((x) => x.id === opt?.id);
            if (foundIndex > -1) return;

            const file = JSON.parse(opt?.data);

            const newFiles = [file, ...this.files];

            if (newFiles.length > this.filter.pageCount) {
              newFiles.pop(); // Remove last
            }

            this.setFiles(newFiles);
          }
          break;
        case "delete":
          if (opt?.type == "file" && opt?.id) {
            const foundIndex = this.files.findIndex((x) => x.id === opt?.id);
            if (foundIndex == -1) return;

            this.setFiles(
              this.files.filter((_, index) => {
                return index !== foundIndex;
              })
            );
          }
          break;
      }
    });

    socketHelper.on("refresh-folder", (id) => {
      if (!id || this.isLoading) return;

      //console.log(
      //  `selected folder id ${this.selectedFolderStore.id} an changed folder id ${id}`
      //);

      if (this.selectedFolderStore.id == id) {
        this.fetchFiles(id, this.filter);
      }
    });

    //WAIT FOR RESPONSES OF EDITING FILE
    socketHelper.on("s:start-edit-file", (id) => {
      //console.log(`Call s:start-edit-file (id=${id})`);
      const foundIndex = this.files.findIndex((x) => x.id === id);
      if (foundIndex == -1) return;

      this.updateFileStatus(
        foundIndex,
        this.files[foundIndex].fileStatus | FileStatus.IsEditing
      );
    });

    socketHelper.on("s:stop-edit-file", (id, data) => {
      //console.log(`Call s:stop-edit-file (id=${id})`);
      const foundIndex = this.files.findIndex((x) => x.id === id);
      if (foundIndex == -1) return;

      let file;

      if (data) {
        file = JSON.parse(data);
        console.log(`socket stop-edit-file (id=${id}`, file);
      }

      this.updateFileStatus(
        foundIndex,
        this.files[foundIndex].fileStatus & ~FileStatus.IsEditing,
        file
      );
    });
  }

  setIsPrevSettingsModule = (isSettings) => {
    this.isPrevSettingsModule = isSettings;
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
      setModuleInfo,
    } = this.settingsStore;
    const { isDesktopClient } = settingsStore;

    setModuleInfo(config.homepage, config.id);

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
        this.treeFoldersStore.fetchTreeFolders()
      );

      if (isDesktopClient) {
        requests.push(getIsEncryptionSupport(), getEncryptionKeys());
      }
    }
    requests.push(getFilesSettings());

    return Promise.all(requests).then(() => (this.isInit = true));
  };

  setFirstLoad = (firstLoad) => {
    this.firstLoad = firstLoad;
  };

  setFiles = (files) => {
    const { socketHelper } = this.settingsStore;
    if (files.length === 0 && this.files.length === 0) return;

    if (this.files?.length > 0) {
      socketHelper.emit({
        command: "unsubscribe",
        data: this.files.map((f) => `FILE-${f.id}`),
      });
    }

    this.files = files;

    if (this.files?.length > 0) {
      socketHelper.emit({
        command: "subscribe",
        data: this.files.map((f) => `FILE-${f.id}`),
      });
    }
  };

  setFolders = (folders) => {
    if (folders.length === 0 && this.folders.length === 0) return;
    this.folders = folders;
  };

  updateFileStatus = (index, status, file) => {
    if (index < 0) return;

    if (file) {
      this.files[index] = file;
    }

    this.files[index].fileStatus = status;
  };

  setFile = (file) => {
    const index = this.files.findIndex((x) => x.id === file.id);
    if (index !== -1) this.files[index] = file;
  };

  setFolder = (folder) => {
    const index = this.folders.findIndex((x) => x.id === folder.id);
    if (index !== -1) this.folders[index] = folder;
  };

  getFilesChecked = (file, selected) => {
    if (!file.parentId) {
      if (this.activeFiles.includes(file.id)) return false;
    } else {
      if (this.activeFolders.includes(file.id)) return false;
    }

    const type = file.fileType;
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
    if (selected === "close" || selected === "none")
      this.setBufferSelection(null);

    this.selected = selected;
    const files = this.files.concat(this.folders);
    this.selection = this.getFilesBySelected(files, selected);
  };

  setSelection = (selection) => {
    this.selection = selection;
  };

  setBufferSelection = (bufferSelection) => {
    this.bufferSelection = bufferSelection;
  };

  //TODO: FILTER
  setFilesFilter = (filter, isPrefSettings) => {
    const key = `UserFilter=${this.userStore.user.id}`;
    const value = `${filter.sortBy},${filter.pageCount},${filter.sortOrder}`;
    localStorage.setItem(key, value);

    !isPrefSettings && this.setFilterUrl(filter);
    this.filter = filter;
  };

  setFilter = (filter) => {
    this.filter = filter;
  };

  setFilesOwner = (folderIds, fileIds, ownerId) => {
    return api.files.setFileOwner(folderIds, fileIds, ownerId);
  };

  setFilterUrl = (filter) => {
    const urlFilter = filter.toUrlParams();
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/filter?${urlFilter}`
      )
    );
  };

  isEmptyLastPageAfterOperation = (newSelection) => {
    const selection =
      newSelection || this.selection?.length || [this.bufferSelection].length;

    return (
      selection &&
      this.filter.page > 0 &&
      !this.filter.hasNext() &&
      selection === this.files.length + this.folders.length
    );
  };

  resetFilterPage = () => {
    let newFilter;
    newFilter = this.filter.clone();
    newFilter.page--;

    return newFilter;
  };

  fetchFiles = (
    folderId,
    filter,
    clearFilter = true,
    withSubfolders = false
  ) => {
    const {
      treeFolders,
      setSelectedNode,
      getSubfolders,
      selectedTreeNode,
    } = this.treeFoldersStore;
    const { id } = this.selectedFolderStore;

    const isPrefSettings = isNaN(+selectedTreeNode) && !id;
    isPrefSettings && this.setIsPrevSettingsModule(true);

    const filterData = filter ? filter.clone() : FilesFilter.getDefault();
    filterData.folder = folderId;

    const filterStorageItem = localStorage.getItem(
      `UserFilter=${this.userStore.user.id}`
    );

    if (filterStorageItem && !filter) {
      const splitFilter = filterStorageItem.split(",");

      filterData.sortBy = splitFilter[0];
      filterData.pageCount = +splitFilter[1];
      filterData.sortOrder = splitFilter[2];
    }

    setSelectedNode([folderId + ""]);

    //TODO: fix @my
    let requestCounter = 1;
    const request = () =>
      api.files
        .getFolder(folderId, filterData)
        .then(async (data) => {
          const isRecycleBinFolder =
            data.current.rootFolderType === FolderType.TRASH;

          !isRecycleBinFolder && this.checkUpdateNode(data, folderId);

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

          if (!isRecycleBinFolder && withSubfolders) {
            const path = data.pathParts.slice(0);
            const foldersCount = data.current.foldersCount;
            const subfolders = await getSubfolders(folderId);
            loopTreeFolders(path, treeFolders, subfolders, foldersCount);
          }

          const isPrivacyFolder =
            data.current.rootFolderType === FolderType.Privacy;

          this.setFilesFilter(filterData, isPrefSettings); //TODO: FILTER
          this.setFolders(isPrivacyFolder && isMobile ? [] : data.folders);
          this.setFiles(isPrivacyFolder && isMobile ? [] : data.files);

          if (clearFilter) {
            this.fileActionStore.setAction({ type: null });
            this.setSelected("close");
          }

          this.selectedFolderStore.setSelectedFolder({
            folders: data.folders,
            ...data.current,
            pathParts: data.pathParts,
            ...{ new: data.new },
          });

          const selectedFolder = {
            selectedFolder: { ...this.selectedFolderStore },
          };
          this.viewAs === "tile" && this.createThumbnails();
          return Promise.resolve(selectedFolder);
        })
        .catch((err) => {
          toastr.error(err);
          if (!requestCounter) return;
          requestCounter--;

          if (folderId === "@my" /*  && !this.isInit */) {
            setTimeout(() => {
              return request();
            }, 5000);
          } else {
            this.treeFoldersStore.fetchTreeFolders();
            return this.fetchFiles(
              this.userStore.user.isVisitor ? "@common" : "@my"
            );
          }
        });

    return request();
  };

  checkUpdateNode = async (data, folderId) => {
    const { treeFolders, getSubfolders } = this.treeFoldersStore;
    const { pathParts, current } = data;

    if (current.parentId === 0) return;

    const somePath = pathParts.slice(0);
    const path = pathParts.slice(0);
    let newItems = treeFolders;

    while (somePath.length !== 1) {
      const folderItem = newItems.find((x) => x.id === somePath[0]);
      newItems = folderItem?.folders
        ? folderItem.folders
        : somePath.length > 1
        ? []
        : null;
      if (!newItems) {
        return;
      }

      somePath.shift();
    }

    if (!newItems.find((x) => x.id == folderId)) {
      path.splice(pathParts.length - 1, 1);
      const subfolders = await getSubfolders(current.parentId);
      loopTreeFolders(path, treeFolders, subfolders, 0);
    }
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
    if (isFileSelected)
      this.selection = this.selection.filter((x) => x.id !== id);
  };

  removeOptions = (options, toRemoveArray) =>
    options.filter((o) => !toRemoveArray.includes(o));

  getFilesContextOptions = (item, canOpenPlayer) => {
    const isVisitor =
      (this.userStore.user && this.userStore.user.isVisitor) || false;
    const isFile = !!item.fileExst || item.contentLength;
    const isFavorite =
      (item.fileStatus & FileStatus.IsFavorite) === FileStatus.IsFavorite;
    const isFullAccess = item.access < 2;
    const withoutShare = false; //TODO: need this prop
    const isThirdPartyItem = !!item.providerKey;
    const hasNew =
      item.new > 0 || (item.fileStatus & FileStatus.IsNew) === FileStatus.IsNew;
    const canConvert = false; //TODO: fix of added convert check;
    const isEncrypted = item.encrypted;
    const isDocuSign = false; //TODO: need this prop;
    const isEditing =
      (item.fileStatus & FileStatus.IsEditing) === FileStatus.IsEditing;
    const isFileOwner = item.createdBy.id === this.userStore.user.id;

    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      isRecentFolder,
      isCommon,
      isShare,
      isFavoritesFolder,
      isShareFolder,
      isMy,
    } = this.treeFoldersStore;

    const {
      canWebEdit,
      canViewedDocs,
      canFormFillingDocs,
    } = this.filesSettingsStore;

    const isThirdPartyFolder =
      item.providerKey && item.id === item.rootFolderId;
    const isShareItem = isShare(item.rootFolderType);
    const isCommonFolder = isCommon(item.rootFolderType);
    const isMyFolder = isMy(item.rootFolderType);

    const { personal } = this.settingsStore;
    const { isDesktopClient } = this.authStore.settingsStore;

    if (isFile) {
      const shouldFillForm = canFormFillingDocs(item.fileExst);
      const shouldEdit = !shouldFillForm && canWebEdit(item.fileExst);
      const shouldView = canViewedDocs(item.fileExst);
      const isMasterForm = item.fileExst === ".docxf";

      let fileOptions = [
        //"open",
        "fill-form",
        "edit",
        "preview",
        "view",
        "make-form",
        "separator0",
        "sharing-settings",
        "external-link",
        "owner-change",
        "link-for-portal-users",
        "send-by-email",
        "docu-sign",
        "version", //category
        "finalize-version",
        "show-version-history",
        "block-unblock-version", //need split
        "separator1",
        "open-location",
        "mark-read",
        "mark-as-favorite",
        "remove-from-favorites",
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
        "unsubscribe",
        "delete",
      ];

      if (!isMasterForm)
        fileOptions = this.removeOptions(fileOptions, ["make-form"]);

      if (!shouldFillForm)
        fileOptions = this.removeOptions(fileOptions, ["fill-form"]);

      if (personal) {
        fileOptions = this.removeOptions(fileOptions, [
          "owner-change",
          "link-for-portal-users",
          "docu-sign",
          "mark-read",
          "unsubscribe",
        ]);

        if (!shouldEdit && !shouldView) {
          fileOptions = this.removeOptions(fileOptions, ["sharing-settings"]);
        }
      }

      if (!this.canConvertSelected) {
        fileOptions = this.removeOptions(fileOptions, ["download-as"]);
      }

      if (!canConvert || isEncrypted) {
        fileOptions = this.removeOptions(fileOptions, ["convert"]);
      }

      if (!canOpenPlayer) {
        fileOptions = this.removeOptions(fileOptions, ["view"]);
      } else {
        fileOptions = this.removeOptions(fileOptions, ["edit", "preview"]);
      }

      if (!isDocuSign) {
        fileOptions = this.removeOptions(fileOptions, ["docu-sign"]);
      }

      if (isEditing) {
        fileOptions = this.removeOptions(fileOptions, [
          "finalize-version",
          "move-to",
          "separator2",
          "delete",
        ]);
        if (isThirdPartyFolder) {
          fileOptions = this.removeOptions(fileOptions, ["rename"]);
        }
      }

      if (isFavorite) {
        fileOptions = this.removeOptions(fileOptions, ["mark-as-favorite"]);
      } else {
        fileOptions = this.removeOptions(fileOptions, [
          "remove-from-favorites",
        ]);

        if (isFavoritesFolder) {
          fileOptions = this.removeOptions(fileOptions, ["mark-as-favorite"]);
        }
      }

      if (isFavoritesFolder) {
        fileOptions = this.removeOptions(fileOptions, [
          "move-to",
          "delete",
          "copy",
        ]);

        if (!isFavorite) {
          fileOptions = this.removeOptions(fileOptions, ["separator2"]);
        }
      }

      if (isEncrypted) {
        fileOptions = this.removeOptions(fileOptions, [
          "open",
          "edit",
          "make-form",
          "link-for-portal-users",
          "external-link",
          "send-by-email",
          "block-unblock-version", //need split
          "version", //category
          "finalize-version",
          "copy-to",
          "copy",
          "mark-as-favorite",
        ]);
      }

      if (isRecentFolder) {
        fileOptions = this.removeOptions(fileOptions, ["delete"]);

        if (!isFavorite) {
          fileOptions = this.removeOptions(fileOptions, ["separator2"]);
        }
      }

      if (isFavoritesFolder || isRecentFolder) {
        fileOptions = this.removeOptions(fileOptions, [
          "make-form",
          "copy",
          "move-to",
          //"sharing-settings",
          "unsubscribe",
          "separator2",
        ]);
      }

      if (isRecycleBinFolder) {
        fileOptions = this.removeOptions(fileOptions, [
          "fill-form",
          "open",
          "open-location",
          "view",
          "preview",
          "edit",
          "make-form",
          "link-for-portal-users",
          "sharing-settings",
          "external-link",
          "send-by-email",
          "block-unblock-version", //need split
          "version", //category
          "finalize-version",
          "show-version-history",
          "move", //category
          "move-to",
          "copy-to",
          "copy",
          "mark-read",
          "mark-as-favorite",
          "remove-from-favorites",
          "rename",
          "separator0",
          "separator1",
        ]);
      } else {
        fileOptions = this.removeOptions(fileOptions, ["restore"]);
      }

      if (!isFullAccess) {
        fileOptions = this.removeOptions(fileOptions, [
          "finalize-version",
          "rename",
          "block-unblock-version",
          "copy",
          "sharing-settings",
        ]);
      }

      if (isVisitor) {
        fileOptions = this.removeOptions(fileOptions, [
          "block-unblock-version",
          "finalize-version",
          "mark-as-favorite",
          "remove-from-favorites",
        ]);

        if (!isFullAccess) {
          fileOptions = this.removeOptions(fileOptions, ["rename"]);
        }
      }

      if (!this.canShareOwnerChange(item)) {
        fileOptions = this.removeOptions(fileOptions, ["owner-change"]);
      }

      if (isThirdPartyItem) {
        fileOptions = this.removeOptions(fileOptions, [
          "owner-change",
          "finalize-version",
          "copy",
        ]);
      }

      if (isCommonFolder) {
        if (!this.userAccess) {
          fileOptions = this.removeOptions(fileOptions, [
            "owner-change",
            "move-to",
            "delete",
            "copy",
            "separator2",
          ]);
          if (!isFavorite) {
            fileOptions = this.removeOptions(fileOptions, ["separator2"]);
          }
        }
      }

      if (withoutShare) {
        fileOptions = this.removeOptions(fileOptions, [
          "sharing-settings",
          "external-link",
        ]);
      }

      if (!hasNew) {
        fileOptions = this.removeOptions(fileOptions, ["mark-read"]);
      }

      if (
        !(
          isRecentFolder ||
          isFavoritesFolder ||
          (isMyFolder && (this.filterType || this.filterSearch))
        )
      ) {
        fileOptions = this.removeOptions(fileOptions, ["open-location"]);
      }

      if (isShareItem) {
        if (!isFullAccess) {
          fileOptions = this.removeOptions(fileOptions, ["edit"]);
        }

        if (isShareFolder) {
          fileOptions = this.removeOptions(fileOptions, [
            "copy",
            "move-to",
            "delete",
          ]);
        }
      } else if (!isEncrypted) {
        fileOptions = this.removeOptions(fileOptions, ["unsubscribe"]);
      }

      if (isPrivacyFolder) {
        fileOptions = this.removeOptions(fileOptions, [
          "preview",
          "view",
          "separator0",
          "copy",
          "download-as",
        ]);

        if (!isDesktopClient) {
          fileOptions = this.removeOptions(fileOptions, ["sharing-settings"]);
        }

        fileOptions = this.removeOptions(
          fileOptions,
          isFileOwner ? ["unsubscribe"] : ["move-to", "delete"]
        );
      }

      if (!shouldEdit && !shouldView && !fileOptions.includes("view")) {
        fileOptions = this.removeOptions(fileOptions, [
          "edit",
          "preview",
          "separator0",
        ]);
      }

      if (!shouldEdit && shouldView) {
        fileOptions = this.removeOptions(fileOptions, ["edit"]);
      }

      return fileOptions;
    } else {
      let folderOptions = [
        "open",
        "separator0",
        "sharing-settings",
        "owner-change",
        "link-for-portal-users",
        "separator1",
        "open-location",
        "download",
        "move", //category
        "move-to",
        "copy-to",
        "mark-read",
        "restore",
        "rename",
        "change-thirdparty-info",
        "separator2",
        "unsubscribe",
        "delete",
      ];

      if (personal) {
        folderOptions = this.removeOptions(folderOptions, [
          "sharing-settings",
          "owner-change",
          "link-for-portal-users",
          "separator1",
          "docu-sign",
          "mark-read",
          "unsubscribe",
        ]);
      }

      if (isPrivacyFolder) {
        folderOptions = this.removeOptions(folderOptions, [
          "sharing-settings",
          "copy",
          "copy-to",
        ]);

        if (!isDesktopClient) {
          folderOptions = this.removeOptions(folderOptions, ["rename"]);
        }
      }

      if (isShareItem) {
        if (isShareFolder) {
          folderOptions = this.removeOptions(folderOptions, [
            "move-to",
            "delete",
          ]);
        }
      } else {
        folderOptions = this.removeOptions(folderOptions, ["unsubscribe"]);
      }

      if (isRecycleBinFolder) {
        folderOptions = this.removeOptions(folderOptions, [
          "open",
          "link-for-portal-users",
          "sharing-settings",
          "move",
          "move-to",
          "copy-to",
          "mark-read",
          "rename",
          "separator0",
          "separator1",
        ]);
      } else {
        folderOptions = this.removeOptions(folderOptions, ["restore"]);
      }

      if (!isFullAccess) {
        //TODO: if added Projects, add project folder check
        folderOptions = this.removeOptions(folderOptions, [
          "rename",
          "change-thirdparty-info",
        ]);
      }

      if (!this.canShareOwnerChange(item)) {
        folderOptions = this.removeOptions(folderOptions, ["owner-change"]);
      }

      if (!isFullAccess) {
        folderOptions = this.removeOptions(folderOptions, [
          "owner-change",
          "move-to",
          "delete",
          "change-thirdparty-info",
        ]);

        if (!isShareItem) {
          folderOptions = this.removeOptions(folderOptions, ["separator2"]);
        }

        if (isVisitor) {
          folderOptions = this.removeOptions(folderOptions, ["rename"]);
        }
      }

      if (withoutShare) {
        folderOptions = this.removeOptions(folderOptions, ["sharing-settings"]);
      }

      if (!hasNew) {
        folderOptions = this.removeOptions(folderOptions, ["mark-read"]);
      }

      if (isThirdPartyFolder) {
        folderOptions = this.removeOptions(folderOptions, ["move-to"]);

        if (isDesktopClient) {
          folderOptions = this.removeOptions(folderOptions, [
            "separator2",
            "delete",
          ]);
        }
      } else {
        folderOptions = this.removeOptions(folderOptions, [
          "change-thirdparty-info",
        ]);
      }

      if (isThirdPartyItem) {
        folderOptions = this.removeOptions(folderOptions, ["owner-change"]);

        if (isShareFolder) {
          folderOptions = this.removeOptions(folderOptions, [
            "change-thirdparty-info",
          ]);
        } else {
          if (isDesktopClient) {
            folderOptions = this.removeOptions(folderOptions, [
              "change-thirdparty-info",
            ]);
          }

          folderOptions = this.removeOptions(folderOptions, ["remove"]);

          if (!item) {
            //For damaged items
            folderOptions = this.removeOptions(folderOptions, [
              "open",
              "download",
              "copy-to",
              "rename",
            ]);
          }
        }
      } else {
        folderOptions = this.removeOptions(folderOptions, [
          "change-thirdparty-info",
        ]);
      }

      if (!(isMyFolder && (this.filterType || this.filterSearch))) {
        folderOptions = this.removeOptions(folderOptions, ["open-location"]);
      }

      return folderOptions;
    }
  };

  addFileToRecentlyViewed = (fileId) => {
    if (this.treeFoldersStore.isPrivacyFolder) return Promise.resolve();
    return api.files.addFileToRecentlyViewed(fileId);
  };

  createFile = (folderId, title, templateId) => {
    return api.files.createFile(folderId, title, templateId).then((file) => {
      return Promise.resolve(file);
    });
  };

  createFolder(parentFolderId, title) {
    return api.files.createFolder(parentFolderId, title);
  }

  setFile = (file) => {
    const fileIndex = this.files.findIndex((f) => f.id === file.id);
    if (fileIndex !== -1) this.files[fileIndex] = file;
  };

  setFolder = (folder) => {
    const folderIndex = this.folders.findIndex((f) => f.id === folder.id);
    if (folderIndex !== -1) this.folders[folderIndex] = folder;
  };

  updateFolderBadge = (id, count) => {
    const folder = this.folders.find((x) => x.id === id);
    if (folder) folder.new -= count;
  };

  updateFileBadge = (id) => {
    const file = this.files.find((x) => x.id === id);
    if (file) file.fileStatus = file.fileStatus & ~FileStatus.IsEditing;
  };

  updateFilesBadge = () => {
    for (let file of this.files) {
      file.fileStatus = file.fileStatus & ~FileStatus.IsEditing;
    }
  };

  updateFoldersBadge = () => {
    for (let folder of this.folders) {
      folder.new = 0;
    }
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
    const userId = this.userStore.user && this.userStore.user.id;

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
      (this.userStore.user && this.userStore.user.isVisitor) || false;

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

  get canCreate() {
    switch (this.selectedFolderStore.rootFolderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        const canCreateInSharedFolder = this.selectedFolderStore.access === 1;
        return (
          !this.selectedFolderStore.isRootFolder && canCreateInSharedFolder
        );
      case FolderType.Privacy:
        return (
          this.authStore.settingsStore.isDesktopClient &&
          this.settingsStore.isEncryptionSupport
        );
      case FolderType.COMMON:
        return this.authStore.isAdmin;
      case FolderType.TRASH:
      default:
        return false;
    }
  }

  onCreateAddTempItem = (items) => {
    const { getFileIcon, getFolderIcon } = this.filesSettingsStore;
    const { extension, title } = this.fileActionStore;

    if (items.length && items[0].id === -1) return; //TODO: if change media collection from state remove this;

    const iconSize = this.viewAs === "tile" && isMobile ? 32 : 24;
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

  get filesList() {
    const { getIcon } = this.filesSettingsStore;
    //return [...this.folders, ...this.files];

    const items = [...this.folders, ...this.files];
    const newItem = items.map((item) => {
      const {
        access,
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
        locked,
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
      } = item;

      const { canConvert, isMediaOrImage } = this.filesSettingsStore;

      const canOpenPlayer = isMediaOrImage(item.fileExst);

      const previewUrl = canOpenPlayer
        ? combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            `/#preview/${id}`
          )
        : null;
      const contextOptions = this.getFilesContextOptions(item, canOpenPlayer);
      const isThirdPartyFolder = providerKey && id === rootFolderId;

      const iconSize = this.viewAs === "table" ? 24 : 32;
      const icon = getIcon(iconSize, fileExst, providerKey, contentLength);

      let isFolder = false;
      this.folders.map((x) => {
        if (x.id === item.id) isFolder = true;
      });

      const { isRecycleBinFolder } = this.treeFoldersStore;

      const folderUrl = isFolder
        ? combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            `/filter?folder=${id}`
          )
        : null;

      const needConvert = canConvert(fileExst);
      const isEditing =
        (item.fileStatus & FileStatus.IsEditing) === FileStatus.IsEditing;

      const docUrl = combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/doceditor?fileId=${id}${needConvert ? "&action=view" : ""}`
      );

      const href = isRecycleBinFolder
        ? null
        : previewUrl
        ? previewUrl
        : !isFolder
        ? docUrl
        : folderUrl;

      return {
        access,
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
        id,
        isFolder,
        locked,
        new: item.new,
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
        previewUrl,
        folderUrl,
        href,
        isThirdPartyFolder,
        isEditing,
      };
    });

    if (this.fileActionStore.type === FileAction.Create) {
      this.onCreateAddTempItem(newItem);
    }

    return newItem;
  }

  get cbMenuItems() {
    const {
      isImage,
      isVideo,
      isDocument,
      isPresentation,
      isSpreadsheet,
      isArchive,
    } = this.filesSettingsStore;

    let cbMenu = ["all"];
    const filesItems = [...this.files, ...this.folders];

    if (this.folders.length) cbMenu.push(FilterType.FoldersOnly);
    for (let item of filesItems) {
      if (isDocument(item.fileExst)) cbMenu.push(FilterType.DocumentsOnly);
      else if (isPresentation(item.fileExst))
        cbMenu.push(FilterType.PresentationsOnly);
      else if (isSpreadsheet(item.fileExst))
        cbMenu.push(FilterType.SpreadsheetsOnly);
      else if (isImage(item.fileExst)) cbMenu.push(FilterType.ImagesOnly);
      else if (isVideo(item.fileExst)) cbMenu.push(FilterType.MediaOnly);
      else if (isArchive(item.fileExst)) cbMenu.push(FilterType.ArchiveOnly);
    }

    const hasFiles = cbMenu.some(
      (elem) => elem !== "all" && elem !== FilterType.FoldersOnly
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
    } = this.filesSettingsStore;

    let sortedFiles = {
      documents: [],
      spreadsheets: [],
      presentations: [],
      other: [],
    };

    const selection = this.selection.length
      ? this.selection
      : this.bufferSelection
      ? [this.bufferSelection]
      : [];

    for (let item of selection) {
      item.checked = true;
      item.format = null;

      const canConvert = extsConvertible[item.fileExst];

      if (item.fileExst && canConvert) {
        if (isSpreadsheet(item.fileExst)) {
          sortedFiles.spreadsheets.push(item);
        } else if (isPresentation(item.fileExst)) {
          sortedFiles.presentations.push(item);
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

  get isThirdPartyRootSelection() {
    const withProvider = this.selection.find((x) => x.providerKey);
    return withProvider && withProvider.rootFolderId === withProvider.id;
  }

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
    const { canViewedDocs } = this.filesSettingsStore;

    return this.selection.some((selected) => {
      if (selected.isFolder === true || !selected.fileExst) return false;
      return canViewedDocs(selected.fileExst);
    });
  }

  get isMediaSelected() {
    const { isMediaOrImage } = this.filesSettingsStore;

    return this.selection.some((selected) => {
      if (selected.isFolder === true || !selected.fileExst) return false;
      return isMediaOrImage(selected.fileExst);
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
    const {
      canWebEdit,
      canWebComment,
      canWebReview,
      canFormFillingDocs,
      canWebFilterEditing,
      canConvert,
    } = this.filesSettingsStore;

    if (selection[0].encrypted) {
      return ["FullAccess", "DenyAccess"];
    }

    let AccessOptions = [];

    AccessOptions.push("ReadOnly", "DenyAccess");

    const webEdit = selection.find((x) => canWebEdit(x.fileExst));

    const webComment = selection.find((x) => canWebComment(x.fileExst));

    const webReview = selection.find((x) => canWebReview(x.fileExst));

    const formFillingDocs = selection.find((x) =>
      canFormFillingDocs(x.fileExst)
    );

    const webFilter = selection.find((x) => canWebFilterEditing(x.fileExst));

    const webNeedConvert = selection.find((x) => canConvert(x.fileExst));

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

  setSelections = (items) => {
    if (!items.length && !this.selection.length) return;

    //if (items.length !== this.selection.length) {
    const newSelection = [];

    for (let item of items) {
      const value = item.getAttribute("value");
      const splitValue = value && value.split("_");

      const fileType = splitValue[0];
      // const id =
      //   splitValue[splitValue.length - 1] === "draggable"
      //     ? splitValue.slice(1, -1).join("_")
      //     : splitValue.slice(1, -1).join("_");

      const id = splitValue.slice(1, -1).join("_");

      if (fileType === "file") {
        this.activeFiles.findIndex((f) => f == id) === -1 &&
          //newSelection.push(this.files.find((f) => f.id == id));
          newSelection.push(
            this.filesList.find((f) => f.id == id && !f.isFolder)
          );
      } else if (this.activeFolders.findIndex((f) => f == id) === -1) {
        //const selectableFolder = this.folders.find((f) => f.id == id);
        const selectableFolder = this.filesList.find(
          (f) => f.id == id && f.isFolder
        );
        selectableFolder.isFolder = true;
        newSelection.push(selectableFolder);
      }
    }

    //this.selected === "close" && this.setSelected("none");

    //need fo table view
    const clearSelection = Object.values(
      newSelection.reduce((item, n) => ((item[n.id] = n), item), {})
    );

    this.setSelection(clearSelection);
    //}
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

  getFileInfo = async (id) => {
    const fileInfo = await api.files.getFileInfo(id);
    this.setFile(fileInfo);
    return fileInfo;
  };

  openDocEditor = (id, providerKey = null, tab = null, url = null) => {
    return openEditor(id, providerKey, tab, url);
  };

  getFolderInfo = async (id) => {
    const folderInfo = await api.files.getFolderInfo(id);
    this.setFolder(folderInfo);
    return folderInfo;
  };

  openDocEditor = (id, providerKey = null, tab = null, url = null) => {
    const isPrivacy = this.treeFoldersStore.isPrivacyFolder;
    return openEditor(id, providerKey, tab, url, isPrivacy);
  };

  createThumbnails = () => {
    const filesList = [...this.files, this.folders];
    const fileIds = [];

    filesList.map((file) => {
      const { thumbnailStatus } = file;

      if (thumbnailStatus === thumbnailStatuses.WAITING) fileIds.push(file.id);
    });

    if (fileIds.length) return api.files.createThumbnails(fileIds);
  };

  setIsUpdatingRowItem = (updating) => {
    this.isUpdatingRowItem = updating;
  };

  setPasswordEntryProcess = (process) => {
    this.passwordEntryProcess = process;
  };
}

export default FilesStore;
