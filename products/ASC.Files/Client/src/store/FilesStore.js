import { makeObservable, action, observable } from "mobx";
import { api, constants, store } from "asc-web-common";
import FileActionStore from "./FileActionStore";
import SelectedFolderStore from "./SelectedFolderStore";
import { createTreeFolders } from "./files/selectors";

const { FilesFilter } = api;
const { FolderType } = constants;
const { authStore } = store;

class FilesStore {
  fileActionStore = null;
  selectedFolderStore = null;

  firstLoad = true;
  files = [];
  folders = [];
  selected = "close";
  filter = FilesFilter.getDefault(); //TODO: FILTER

  constructor() {
    makeObservable(this, {
      fileActionStore: observable,
      selectedFolderStore: observable,

      firstLoad: observable,
      files: observable,
      folders: observable,
      selected: observable,
      filter: observable, //TODO: FILTER

      setFirstLoad: action,
      setFiles: action,
      setFolders: action,
      setSelected: action,
      setFilesFilter: action, //TODO: FILTER
      fetchFiles: action,
      getFilesList: action,
    });

    this.fileActionStore = new FileActionStore();
    this.selectedFolderStore = new SelectedFolderStore();
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

  fetchFiles = (folderId, filter, clearFilter = true) => {
    const filterData = filter ? filter.clone() : FilesFilter.getDefault();
    filterData.folder = folderId;

    //const state = getState();
    //const privacyFolder = getPrivacyFolder(state);

    /*if (privacyFolder && privacyFolder.id === +folderId) {
      const isEncryptionSupported = isEncryptionSupport(state);

      if (!isEncryptionSupported) {
        filterData.treeFolders = createTreeFolders(
          privacyFolder.pathParts,
          filterData
        );
        filterData.total = 0;
        dispatch(setFilesFilter(filterData));
        if (clearFilter) {
          dispatch(setFolders([]));
          dispatch(setFiles([]));
          dispatch(setAction({ type: null }));
          dispatch(setSelected("close")); //this.setSelected("close");
          dispatch(
            setSelectedFolder({
              folders: [],
              ...privacyFolder,
              pathParts: privacyFolder.pathParts,
              ...{ new: 0 },
            })
          );
        }
        return Promise.resolve();
      }
    }*/

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

  getFilesList = () => {
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

      //const canOpenPlayer = isMediaOrImage(item.fileExst)(state);
      //const contextOptions = getFilesContextOptions(item,isRecycleBin,isRecent,isFavorites,isVisitor,canOpenPlayer,canChangeOwner,haveAccess,canShare,isPrivacy,isRootFolder);
      //const checked = isFileSelected(selection, id, parentId);

      //const selectedItem = selection.find((x) => x.id === id && x.fileExst === fileExst);

      //const isFolder = selectedItem ? false : fileExst ? false : true;

      //const draggable = selectedItem && !isRecycleBin && selectedItem.id !== actionId;

      let value = fileExst ? `file_${id}` : `folder_${id}`;

      //const isCanWebEdit = canWebEdit(item.fileExst)(state);

      //const icon = getIcon(state, 24, fileExst, providerKey);

      //value += draggable ? "_draggable" : "";

      return {
        access,
        //checked,
        comment,
        contentLength,
        //contextOptions,
        created,
        createdBy,
        fileExst,
        filesCount,
        fileStatus,
        fileType,
        folderId,
        foldersCount,
        //icon,
        id,
        //isFolder,
        locked,
        new: item.new,
        parentId,
        pureContentLength,
        rootFolderType,
        //selectedItem,
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
        //draggable,
        //canOpenPlayer,
        //canWebEdit: isCanWebEdit,
        //canShare,
      };
    });
  };
}

export default FilesStore;
