import { makeObservable, action, observable, computed } from "mobx";
import { api, constants, store } from "asc-web-common";
import FileActionStore from "./FileActionStore";
import SelectedFolderStore from "./SelectedFolderStore";
import TreeFoldersStore from "./TreeFoldersStore";
import FormatsStore from "./FormatsStore";
import MediaViewersFormatsStore from "./MediaViewersFormatsStore";
import DocserviceStore from "./DocserviceStore";
import { createTreeFolders } from "./files/selectors";

const { FilesFilter } = api;
const { FolderType } = constants;
const { authStore } = store;

class FilesStore {
  fileActionStore = null;
  selectedFolderStore = null;
  treeFoldersStore = null;
  formatsStore = null;
  mediaViewersFormatsStore = null;
  docserviceStore = null;

  firstLoad = true;
  files = [];
  folders = [];
  treeFolders = [];
  selection = [];
  selected = "close";
  filter = FilesFilter.getDefault(); //TODO: FILTER

  constructor() {
    makeObservable(this, {
      fileActionStore: observable,
      selectedFolderStore: observable,
      treeFoldersStore: observable,
      formatsStore: observable,
      mediaViewersFormatsStore: observable,
      docserviceStore: observable,

      firstLoad: observable,
      files: observable,
      folders: observable,
      selected: observable,
      filter: observable, //TODO: FILTER
      selection: observable,

      filesList: computed,

      setFirstLoad: action,
      setFiles: action,
      setFolders: action,
      setSelected: action,
      setFilesFilter: action, //TODO: FILTER
      setSelection: action,
      fetchFiles: action,
      selectFile: action,
      deselectFile: action,
    });

    this.fileActionStore = new FileActionStore();
    this.selectedFolderStore = new SelectedFolderStore();
    this.treeFoldersStore = new TreeFoldersStore();
    this.formatsStore = new FormatsStore();
    this.mediaViewersFormatsStore = new MediaViewersFormatsStore();
    this.docserviceStore = new DocserviceStore();
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

  setSelected = (selected) => {
    this.selected = selected;
  };

  //TODO: FILTER
  setFilesFilter = (filter) => {
    this.filter = filter;
  };

  setSelection = (selection) => {
    this.selection = selection;
  };

  fetchFiles = (folderId, filter, clearFilter = true) => {
    const filterData = filter ? filter.clone() : FilesFilter.getDefault();
    filterData.folder = folderId;

    const { privacyFolder } = this.treeFoldersStore;

    if (privacyFolder && privacyFolder.id === +folderId) {
      const isEncryptionSupported = authStore.settingsStore.isEncryptionSupport;

      if (!isEncryptionSupported) {
        filterData.treeFolders = createTreeFolders(
          privacyFolder.pathParts,
          filterData
        );
        filterData.total = 0;
        this.setFilesFilter(filterData); //TODO: FILTER
        if (clearFilter) {
          this.setFolders([]);
          this.setFiles([]);
          this.fileActionStore.setAction({ type: null });
          this.setSelected("close");

          this.selectedFolderStore.setSelectedFolder({
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
      const isEncryptionSupport = authStore.settingsStore.isEncryptionSupport;
      const isPrivacyFolder =
        data.current.rootFolderType === FolderType.Privacy;

      filterData.treeFolders = createTreeFolders(data.pathParts, filterData);
      filterData.total = data.total;
      this.setFilesFilter(filterData); //TODO: FILTER
      this.setFolders(
        isPrivacyFolder && !isEncryptionSupport ? [] : data.folders
      );
      this.setFiles(isPrivacyFolder && !isEncryptionSupport ? [] : data.files);
      if (clearFilter) {
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
    const folderType = this.selectedFolderStore.rootFolderType;
    const isAdmin = authStore.isAdmin;
    const isVisitor =
      (authStore.userStore.user && authStore.userStore.user.isVisitor) || false;

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
    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      isRecentFolder,
      //isFavoritesFolder
    } = this.treeFoldersStore;

    const options = [];
    const isVisitor =
      (authStore.userStore.user && authStore.userStore.user.isVisitor) || false;
    const canChangeOwner = this.getCanShareOwnerChange();
    const haveAccess = this.getUserAccess();
    const isRootFolder =
      this.selectedFolderStore.pathParts &&
      this.selectedFolderStore.pathParts.length <= 1;

    const isFile = !!item.fileExst;
    const isFavorite = item.fileStatus === 32;
    const isFullAccess = item.access < 2;
    const isThirdPartyFolder = item.providerKey && isRootFolder;

    if (item.id <= 0) return [];

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

      canChangeOwner && options.push("owner-change");
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
        !isThirdPartyFolder && haveAccess && options.push("move");
        options.push("copy");

        if (isFile) {
          options.push("duplicate");
        }

        haveAccess && options.push("rename");
        isThirdPartyFolder &&
          haveAccess &&
          options.push("change-thirdparty-info");
        options.push("separator3");
        haveAccess && options.push("delete");
      } else {
        options.push("copy");
      }
    }

    if (isFavorite && !isRecycleBinFolder) {
      options.push("remove-from-favorites");
    }

    return options;
  };

  getUserAccess = () => {
    switch (this.selectedFolderStore.rootFolderType) {
      case FolderType.USER:
        return true;
      case FolderType.SHARE:
        return false;
      case FolderType.COMMON:
        return (
          authStore.isAdmin ||
          this.selection.some((x) => x.access === 0 || x.access === 1)
        );
      case FolderType.Privacy:
        return true;
      case FolderType.TRASH:
        return true;
      default:
        return false;
    }
  };

  getCanShareOwnerChange = () => {
    const { commonFolder } = this.treeFoldersStore;

    const pathParts = this.selectedFolderStore.pathParts;
    const userId = authStore.userStore.user.id;
    return (
      (authStore.isAdmin ||
        (this.selection.length && this.selection[0].createdBy.id === userId)) &&
      pathParts &&
      commonFolder &&
      commonFolder.id === pathParts[0] &&
      this.selection.length &&
      !this.selection[0].providerKey
    );
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

    return items.map((item) => {
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

      const canOpenPlayer = this.mediaViewersFormatsStore.isMediaOrImage(
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

      const draggable =
        selectedItem &&
        !this.treeFoldersStore.isRecycleBinFolder &&
        selectedItem.id !== this.fileActionStore.id;

      let value = fileExst ? `file_${id}` : `folder_${id}`;
      value += draggable ? "_draggable" : "";

      const isCanWebEdit = this.docserviceStore.canWebEdit(item.fileExst);
      const icon = this.formatsStore.getIcon(24, fileExst, providerKey);

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
  }
}

export default FilesStore;
