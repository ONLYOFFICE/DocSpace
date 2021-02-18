import { makeObservable, action, observable, computed } from "mobx";
import { api, constants, store, history } from "asc-web-common";
import axios from "axios";
import queryString from "query-string";
import FileActionStore from "./FileActionStore";
import selectedFolderStore from "./SelectedFolderStore";
import formatsStore from "./FormatsStore";
import treeFoldersStore from "./TreeFoldersStore";
import { createTreeFolders } from "../helpers/files-helpers";
import {
  AUTHOR_TYPE,
  FILTER_TYPE,
  PAGE_COUNT,
  PAGE,
  SEARCH_TYPE,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  FOLDER,
  PREVIEW,
} from "../helpers/constants";
import config from "../../package.json";

const { FilesFilter } = api;
const { FolderType, FilterType, FileType, FileAction } = constants;
const { authStore } = store;
const { settingsStore, userStore, isAdmin } = authStore;
const { isEncryptionSupport, isDesktopClient } = settingsStore;

const {
  iconFormatsStore,
  mediaViewersFormatsStore,
  docserviceStore,
} = formatsStore;
const {
  isSpreadsheet,
  isPresentation,
  getFileIcon,
  getFolderIcon,
} = iconFormatsStore;
const { getIcon } = iconFormatsStore;
const {
  canWebEdit,
  canWebComment,
  canWebReview,
  canFormFillingDocs,
  canWebFilterEditing,
} = docserviceStore;

class FilesStore {
  fileActionStore = null;

  firstLoad = true;
  files = [];
  folders = [];
  selection = [];
  selected = "close";
  filter = FilesFilter.getDefault(); //TODO: FILTER
  newRowItems = [];

  constructor() {
    makeObservable(this, {
      fileActionStore: observable,

      firstLoad: observable,
      files: observable,
      folders: observable,
      selected: observable,
      filter: observable, //TODO: FILTER
      selection: observable,
      newRowItems: observable,

      filesList: computed,
      sortedFiles: computed,
      canCreate: computed,
      isHeaderVisible: computed,
      isHeaderIndeterminate: computed,
      isHeaderChecked: computed,
      userAccess: computed,
      isAccessedSelected: computed,
      isOnlyFoldersSelected: computed,
      isThirdPartySelection: computed,
      isWebEditSelected: computed,
      canShareOwnerChange: computed,
      selectionTitle: computed,
      currentFilesCount: computed,

      setFirstLoad: action,
      setFiles: action,
      setFolders: action,
      setSelected: action,
      setFilesFilter: action, //TODO: FILTER
      setSelection: action,
      setNewRowItems: action,
      setFilesOwner: action,
      fetchFiles: action,
      selectFile: action,
      deselectFile: action,
      addFileToRecentlyViewed: action,
      createFile: action,
      createFolder: action,
      updateFile: action,
      getAccessOption: action,
      getExternalAccessOption: action,
      setSelections: action,
      getShareUsers: action,
      setShareFiles: action,
      markItemAsFavorite: action,
      removeItemFromFavorite: action,
      fetchFavoritesFolder: action,
      getFileInfo: action,
    });

    this.fileActionStore = new FileActionStore();
  }

  setFirstLoad = (firstLoad) => {
    this.firstLoad = firstLoad;
  };

  setFiles = (files) => {
    this.files = files;
  };

  setFolders = (folders) => {
    this.folders = folders;
  };

  getFilesChecked = (file, selected) => {
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
    this.selected = selected;
    const files = this.files.concat(this.folders);
    this.selection = this.getFilesBySelected(files, selected);
  };

  setSelection = (selection) => {
    this.selection = selection;
  };

  //TODO: FILTER
  setFilesFilter = (filter) => {
    this.setFilterUrl(filter);
    this.filter = filter;
  };

  setFilter = (filter) => {
    this.filter = filter;
  };

  setNewRowItems = (newRowItems) => {
    this.newRowItems = newRowItems;
  };

  setFilesOwner = (folderIds, fileIds, ownerId) => {
    return api.files.setFileOwner(folderIds, fileIds, ownerId);
  };

  setFilterUrl = (filter) => {
    const defaultFilter = FilesFilter.getDefault();
    const params = [];
    const URLParams = queryString.parse(window.location.href);

    if (filter.filterType) {
      params.push(`${FILTER_TYPE}=${filter.filterType}`);
    }

    if (filter.withSubfolders === "false") {
      params.push(`${SEARCH_TYPE}=${filter.withSubfolders}`);
    }

    if (filter.search) {
      params.push(`${SEARCH}=${filter.search.trim()}`);
    }
    if (filter.authorType) {
      params.push(`${AUTHOR_TYPE}=${filter.authorType}`);
    }
    if (filter.folder) {
      params.push(`${FOLDER}=${filter.folder}`);
    }

    if (filter.pageCount !== defaultFilter.pageCount) {
      params.push(`${PAGE_COUNT}=${filter.pageCount}`);
    }

    if (URLParams.preview) {
      params.push(`${PREVIEW}=${URLParams.preview}`);
    }

    params.push(`${PAGE}=${filter.page + 1}`);
    params.push(`${SORT_BY}=${filter.sortBy}`);
    params.push(`${SORT_ORDER}=${filter.sortOrder}`);

    history.push(`${config.homepage}/filter?${params.join("&")}`);
  };

  fetchFiles = (folderId, filter, clearFilter = true) => {
    const filterData = filter ? filter.clone() : FilesFilter.getDefault();
    filterData.folder = folderId;
    const { privacyFolder, expandedKeys, setExpandedKeys } = treeFoldersStore;

    if (privacyFolder && privacyFolder.id === +folderId) {
      if (!isEncryptionSupport) {
        const newExpandedKeys = createTreeFolders(
          privacyFolder.pathParts,
          expandedKeys
        );
        setExpandedKeys(newExpandedKeys);
        filterData.total = 0;
        this.setFilesFilter(filterData); //TODO: FILTER
        if (clearFilter) {
          this.setFolders([]);
          this.setFiles([]);
          this.fileActionStore.setAction({ type: null });
          this.setSelected("close");

          selectedFolderStore.setSelectedFolder({
            folders: [],
            ...privacyFolder,
            pathParts: privacyFolder.pathParts,
            ...{ new: 0 },
          });
        }
        return Promise.resolve();
      }
    }

    return api.files.getFolder(folderId, filter).then((data) => {
      const isPrivacyFolder =
        data.current.rootFolderType === FolderType.Privacy;

      const newExpandedKeys = createTreeFolders(
        data.pathParts,
        treeFoldersStore.expandedKeys
      );
      treeFoldersStore.setExpandedKeys(newExpandedKeys);
      filterData.total = data.total;
      this.setFilesFilter(filterData); //TODO: FILTER
      this.setFolders(
        isPrivacyFolder && !isEncryptionSupport ? [] : data.folders
      );
      this.setFiles(isPrivacyFolder && !isEncryptionSupport ? [] : data.files);
      if (clearFilter) {
        this.setSelected("close");
      }

      selectedFolderStore.setSelectedFolder({
        folders: data.folders,
        ...data.current,
        pathParts: data.pathParts,
        ...{ new: data.new },
      });

      const selectedFolder = {
        selectedFolder: { ...selectedFolderStore },
      };
      return Promise.resolve(selectedFolder);
    });
  };

  isFileSelected = (selection, fileId, parentId) => {
    const item = selection.find(
      (x) => x.id === fileId && x.parentId === parentId
    );

    return item !== undefined;
  };

  selectFile = (file) => {
    const { id, parentId } = file;
    const isFileSelected = this.isFileSelected(this.selection, id, parentId);
    if (!isFileSelected) this.selection.push(file);
  };

  deselectFile = (file) => {
    const { id, parentId } = file;
    const isFileSelected = this.isFileSelected(this.selection, id, parentId);
    if (isFileSelected)
      this.selection = this.selection.filter((x) => x.id !== id);
  };

  isCanShare = () => {
    const folderType = selectedFolderStore.rootFolderType;
    const isVisitor = (userStore.user && userStore.user.isVisitor) || false;

    if (isVisitor) {
      return false;
    }

    switch (folderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        return false;
      case FolderType.COMMON:
        return isAdmin;
      case FolderType.TRASH:
        return false;
      case FolderType.Favorites:
        return false;
      case FolderType.Recent:
        return false;
      case FolderType.Privacy:
        return true;
      default:
        return false;
    }
  };

  getFilesContextOptions = (item, canOpenPlayer, canShare) => {
    const options = [];
    const isVisitor = (userStore.user && userStore.user.isVisitor) || false;

    const isFile = !!item.fileExst;
    const isFavorite = item.fileStatus === 32;
    const isFullAccess = item.access < 2;
    const isThirdPartyFolder =
      item.providerKey && selectedFolderStore.isRootFolder;

    if (item.id <= 0) return [];

    const isRecycleBinFolder = treeFoldersStore.isRecycleBinFolder;
    const isPrivacyFolder = treeFoldersStore.isPrivacyFolder;
    const isRecentFolder = treeFoldersStore.isRecentFolder;

    if (isRecycleBinFolder) {
      options.push("download");
      options.push("download-as");
      options.push("restore");
      options.push("separator0");
      options.push("delete");
    } else if (isPrivacyFolder) {
      if (isFile) {
        options.push("sharing-settings");
        options.push("separator0");
        options.push("show-version-history");
        options.push("separator1");
      }
      options.push("download");
      options.push("move");
      options.push("rename");
      options.push("separator2");
      options.push("delete");
    } else {
      if (!isFile) {
        options.push("open");
        options.push("separator0");
      }

      //TODO: use canShare selector
      if (
        /*!(isRecentFolder || isFavoritesFolder || isVisitor) && */ canShare
      ) {
        options.push("sharing-settings");
      }

      if (isFile && !isVisitor) {
        options.push("send-by-email");
      }

      this.canShareOwnerChange && options.push("owner-change");
      options.push("link-for-portal-users");

      if (!isVisitor) {
        options.push("separator1");
      }

      if (isFile) {
        options.push("show-version-history");
        if (!isVisitor) {
          if (isFullAccess && !item.providerKey && !canOpenPlayer) {
            options.push("finalize-version");
            options.push("block-unblock-version");
          }
          options.push("separator2");

          if (isRecentFolder) {
            options.push("open-location");
          }
          if (!isFavorite) {
            options.push("mark-as-favorite");
          }
        } else {
          options.push("separator3");
        }

        if (canOpenPlayer) {
          options.push("view");
        } else {
          options.push("edit");
          options.push("preview");
        }

        options.push("download");
      }

      if (!isVisitor) {
        !isThirdPartyFolder && this.userAccess && options.push("move");
        options.push("copy");

        if (isFile) {
          options.push("duplicate");
        }

        this.userAccess && options.push("rename");
        isThirdPartyFolder &&
          this.userAccess &&
          options.push("change-thirdparty-info");
        options.push("separator3");
        this.userAccess && options.push("delete");
      } else {
        options.push("copy");
      }
    }

    if (isFavorite && !isRecycleBinFolder) {
      options.push("remove-from-favorites");
    }

    return options;
  };

  addFileToRecentlyViewed = (fileId, isPrivacy) => {
    if (isPrivacy) return Promise.resolve();
    return api.files.addFileToRecentlyViewed(fileId);
  };

  createFile = (folderId, title) => {
    return api.files.createFile(folderId, title).then((file) => {
      return Promise.resolve(file);
    });
  };

  createFolder(parentFolderId, title) {
    return api.files.createFolder(parentFolderId, title);
  }

  //TODO: action?
  setFile = (file) => {
    const fileIndex = this.files.findIndex((f) => f.id === file.id);
    if (fileIndex !== -1) this.files[fileIndex] = file;
  };

  //TODO: action?
  setFolder = (folder) => {
    const folderIndex = this.folders.findIndex((f) => f.id === folder.id);
    if (folderIndex !== -1) this.folders[folderIndex] = folder;
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
    const { filesCount, foldersCount } = selectedFolderStore;
    return filesCount + this.folders ? this.folders.length : foldersCount;
  };

  getServiceFilesCount = () => {
    const filesLength = this.files ? this.files.length : 0;
    const foldersLength = this.folders ? this.folders.length : 0;
    return filesLength + foldersLength;
  };

  get currentFilesCount() {
    const serviceFilesCount = this.getServiceFilesCount();
    const filesCount = this.getFilesCount();
    return selectedFolderStore.providerItem ? serviceFilesCount : filesCount;
  }

  get iconOfDraggedFile() {
    if (this.selection.length === 1) {
      const icon = getIcon(
        24,
        this.selection[0].fileExst,
        this.selection[0].providerKey
      );

      return icon;
    }
    return null;
  }

  get canShareOwnerChange() {
    const pathParts = selectedFolderStore.pathParts;
    const userId = userStore.user.id;
    const commonFolder = treeFoldersStore.commonFolder;
    return (
      (isAdmin ||
        (this.selection.length && this.selection[0].createdBy.id === userId)) &&
      pathParts &&
      commonFolder &&
      commonFolder.id === pathParts[0] &&
      this.selection.length &&
      !this.selection[0].providerKey
    );
  }

  get isHeaderVisible() {
    return this.selection.length > 0 || this.selected !== "close";
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

  get canCreate() {
    switch (selectedFolderStore.rootFolderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        const canCreateInSharedFolder = selectedFolderStore.access === 1;
        return !selectedFolderStore.isRootFolder && canCreateInSharedFolder;
      case FolderType.Privacy:
        return isDesktopClient && isEncryptionSupport;
      case FolderType.COMMON:
        return isAdmin;
      case FolderType.TRASH:
      default:
        return false;
    }
  }

  onCreateAddTempItem = (items) => {
    if (items.length && items[0].id === -1) return; //TODO: if change media collection from state remove this;
    const icon = this.fileActionStore.extension
      ? getFileIcon(`.${this.fileActionStore.extension}`, 24)
      : getFolderIcon(null, 24);

    items.unshift({
      id: -1,
      title: "",
      parentId: selectedFolderStore.id,
      fileExst: this.fileActionStore.extension,
      icon,
    });
  };

  get filesList() {
    const items =
      this.folders && this.files
        ? [...this.folders, ...this.files]
        : this.folders
        ? this.folders
        : this.files
        ? this.files
        : [];

    const newItem = items.map((item) => {
      const {
        access,
        comment,
        contentLength,
        created,
        createdBy,
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
        shared,
        title,
        updated,
        updatedBy,
        version,
        versionGroup,
        viewUrl,
        webUrl,
        providerKey,
      } = item;

      const canOpenPlayer = mediaViewersFormatsStore.isMediaOrImage(
        item.fileExst
      );

      const canShare = this.isCanShare();

      const contextOptions = this.getFilesContextOptions(
        item,
        canOpenPlayer,
        canShare
      );
      const checked = this.isFileSelected(this.selection, id, parentId);

      const selectedItem = this.selection.find(
        (x) => x.id === id && x.fileExst === fileExst
      );

      const isFolder = selectedItem ? false : fileExst ? false : true;
      const isRecycleBinFolder = treeFoldersStore.isRecycleBinFolder;

      const draggable =
        selectedItem &&
        isRecycleBinFolder &&
        selectedItem.id !== this.fileActionStore.id;

      let value = fileExst ? `file_${id}` : `folder_${id}`;
      value += draggable ? "_draggable" : "";

      const isCanWebEdit = canWebEdit(item.fileExst);
      const icon = getIcon(24, fileExst, providerKey);

      return {
        access,
        checked,
        comment,
        contentLength,
        contextOptions,
        created,
        createdBy,
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
        selectedItem,
        shared,
        title,
        updated,
        updatedBy,
        value,
        version,
        versionGroup,
        viewUrl,
        webUrl,
        providerKey,
        draggable,
        canOpenPlayer,
        canWebEdit: isCanWebEdit,
        canShare,
      };
    });

    if (this.fileActionStore.type === FileAction.Create) {
      this.onCreateAddTempItem(newItem);
    }

    return newItem;
  }

  get sortedFiles() {
    const formatKeys = Object.freeze({
      OriginalFormat: 0,
    });

    let sortedFiles = {
      documents: [],
      spreadsheets: [],
      presentations: [],
      other: [],
    };

    for (let item of this.selection) {
      item.checked = true;
      item.format = formatKeys.OriginalFormat;

      if (item.fileExst) {
        if (isSpreadsheet(item.fileExst)) {
          sortedFiles.spreadsheets.push(item);
        } else if (isPresentation(item.fileExst)) {
          sortedFiles.presentations.push(item);
        } else if (item.fileExst !== ".pdf" && canWebEdit(item.fileExst)) {
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
    switch (selectedFolderStore.rootFolderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        return false;
      case FolderType.COMMON:
        return (
          isAdmin ||
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
      isAdmin &&
      this.selection.every((x) => x.access === 1 || x.access === 0)
    );
  }

  get isOnlyFoldersSelected() {
    return this.selection.every((selected) => selected.isFolder === true);
  }

  get isThirdPartySelection() {
    const withProvider = this.selection.find((x) => !x.providerKey);
    return !withProvider && selectedFolderStore.isRootFolder;
  }

  get isWebEditSelected() {
    return this.selection.some((selected) => {
      if (selected.isFolder === true || !selected.fileExst) return false;
      return docserviceStore.editedDocs.find(
        (format) => selected.fileExst === format
      );
    });
  }

  get selectionTitle() {
    if (this.selection.length === 0) return null;
    return this.selection.find((el) => el.title).title;
  }

  getOptions = (selection, externalAccess = false) => {
    const webEdit = selection.find((x) => canWebEdit(x.fileExst));
    const webComment = selection.find((x) => canWebComment(x.fileExst));
    const webReview = selection.find((x) => canWebReview(x.fileExst));
    const formFillingDocs = selection.find((x) =>
      canFormFillingDocs(x.fileExst)
    );
    const webFilter = selection.find((x) => canWebFilterEditing(x.fileExst));

    let AccessOptions = [];

    if (webEdit || !externalAccess) AccessOptions.push("FullAccess");

    AccessOptions.push("ReadOnly", "DenyAccess");

    if (webComment) AccessOptions.push("Comment");
    if (webReview) AccessOptions.push("Review");
    if (formFillingDocs) AccessOptions.push("FormFilling");
    if (webFilter) AccessOptions.push("FilterEditing");
    return AccessOptions;
  };

  getAccessOption = (selection) => {
    return this.getOptions(selection);
  };

  getExternalAccessOption = (selection) => {
    return this.getOptions(selection, true);
  };

  convertSplitItem = (item) => {
    let splitItem = item.split("_");
    const fileExst = splitItem[0];
    splitItem.splice(0, 1);
    if (splitItem[splitItem.length - 1] === "draggable") {
      splitItem.splice(-1, 1);
    }
    splitItem = splitItem.join("_");
    return [fileExst, splitItem];
  };

  setSelections = (items) => {
    if (this.selection.length > items.length) {
      //Delete selection
      const newSelection = [];
      let newFile = null;
      for (let item of items) {
        if (!item) break; // temporary fall protection selection tile

        item = this.convertSplitItem(item);
        if (item[0] === "folder") {
          newFile = this.selection.find(
            (x) => x.id + "" === item[1] && !x.fileExst
          );
        } else if (item[0] === "file") {
          newFile = this.selection.find(
            (x) => x.id + "" === item[1] && x.fileExst
          );
        }
        if (newFile) {
          newSelection.push(newFile);
        }
      }

      for (let item of this.selection) {
        const element = newSelection.find(
          (x) => x.id === item.id && x.fileExst === item.fileExst
        );
        if (!element) {
          this.deselectFile(item);
        }
      }
    } else if (this.selection.length < items.length) {
      //Add selection
      for (let item of items) {
        if (!item) break; // temporary fall protection selection tile

        let newFile = null;
        item = this.convertSplitItem(item);
        if (item[0] === "folder") {
          newFile = this.folders.find(
            (x) => x.id + "" === item[1] && !x.fileExst
          );
        } else if (item[0] === "file") {
          newFile = this.files.find((x) => x.id + "" === item[1] && x.fileExst);
        }
        if (newFile && this.fileActionStore.id !== newFile.id) {
          const existItem = this.selection.find(
            (x) => x.id === newFile.id && x.fileExst === newFile.fileExst
          );
          if (!existItem) {
            this.selectFile(newFile);
            this.selected !== "none" && this.setSelected("none");
          }
        }
      }
    } else if (this.selection.length === items.length && items.length === 1) {
      const item = this.convertSplitItem(items[0]);

      if (item[1] !== this.selection[0].id) {
        let addFile = null;
        let delFile = null;
        const newSelection = [];
        if (item[0] === "folder") {
          delFile = this.selection.find(
            (x) => x.id + "" === item[1] && !x.fileExst
          );
          addFile = this.folders.find(
            (x) => x.id + "" === item[1] && !x.fileExst
          );
        } else if (item[0] === "file") {
          delFile = this.selection.find(
            (x) => x.id + "" === item[1] && x.fileExst
          );
          addFile = this.files.find((x) => x.id + "" === item[1] && x.fileExst);
        }

        const existItem = this.selection.find(
          (x) => x.id === addFile.id && x.fileExst === addFile.fileExst
        );
        if (!existItem) {
          this.selectFile(addFile);
          this.selected !== "none" && this.setSelected("none");
        }

        if (delFile) {
          newSelection.push(delFile);
        }

        for (let item of this.selection) {
          const element = newSelection.find(
            (x) => x.id === item.id && x.fileExst === item.fileExst
          );
          if (!element) {
            this.deselectFile(item);
          }
        }
      } else {
        return;
      }
    } else {
      return;
    }
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

    return axios.all(requests);
  };

  markItemAsFavorite = (id) => api.files.markAsFavorite(id);

  removeItemFromFavorite = (id) => api.files.removeFromFavorite(id);

  fetchFavoritesFolder = async (folderId) => {
    const favoritesFolder = await api.files.getFolder(folderId);
    this.setFolders(favoritesFolder.folders);
    this.setFiles(favoritesFolder.files);

    selectedFolderStore.setSelectedFolder({
      folders: favoritesFolder.folders,
      ...favoritesFolder.current,
      pathParts: favoritesFolder.pathParts,
    });
  };

  getFileInfo = async (id) => {
    const fileInfo = await api.files.getFileInfo(id);
    this.setFile(fileInfo);
  };
}

export default new FilesStore();
