import {
  checkFileConflicts,
  deleteFile,
  deleteFolder,
  downloadFiles,
  emptyTrash,
  finalizeVersion,
  lockFile,
  markAsRead,
  removeFiles,
  removeShareFiles,
  createFolder,
} from "@docspace/common/api/files";
import { deleteRoom } from "@docspace/common/api/rooms";
import {
  ConflictResolveType,
  FileAction,
  FileStatus,
  FolderType,
} from "@docspace/common/constants";
import { makeAutoObservable } from "mobx";
import { isMobile } from "react-device-detect";
import toastr from "@docspace/components/toast/toastr";
import { TIMEOUT } from "@docspace/client/src/helpers/filesConstants";
import { checkProtocol } from "../helpers/files-helpers";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import FilesFilter from "@docspace/common/api/files/filter";
import api from "@docspace/common/api";
import { isTablet } from "@docspace/components/utils/device";

class FilesActionStore {
  authStore;
  uploadDataStore;
  treeFoldersStore;
  filesStore;
  selectedFolderStore;
  settingsStore;
  dialogsStore;
  mediaViewerDataStore;
  accessRightsStore;

  isBulkDownload = false;
  searchTitleOpenLocation = null;
  itemOpenLocation = null;
  isLoadedLocationFiles = false;
  isLoadedSearchFiles = false;

  constructor(
    authStore,
    uploadDataStore,
    treeFoldersStore,
    filesStore,
    selectedFolderStore,
    settingsStore,
    dialogsStore,
    mediaViewerDataStore,
    accessRightsStore
  ) {
    makeAutoObservable(this);
    this.authStore = authStore;
    this.uploadDataStore = uploadDataStore;
    this.treeFoldersStore = treeFoldersStore;
    this.filesStore = filesStore;
    this.selectedFolderStore = selectedFolderStore;
    this.settingsStore = settingsStore;
    this.dialogsStore = dialogsStore;
    this.mediaViewerDataStore = mediaViewerDataStore;
    this.accessRightsStore = accessRightsStore;
  }

  setIsBulkDownload = (isBulkDownload) => {
    this.isBulkDownload = isBulkDownload;
  };

  isMediaOpen = () => {
    const { visible, setMediaViewerData, playlist } = this.mediaViewerDataStore;
    if (visible && playlist.length === 1) {
      setMediaViewerData({ visible: false, id: null });
    }
  };

  updateCurrentFolder = (fileIds, folderIds, clearSelection) => {
    const {
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    const {
      filter,
      fetchFiles,
      roomsFilter,
      fetchRooms,
      isEmptyLastPageAfterOperation,
      resetFilterPage,
    } = this.filesStore;

    const { isRoomsFolder, isArchiveFolder } = this.treeFoldersStore;

    let newFilter;

    const selectionFilesLength =
      fileIds && folderIds
        ? fileIds.length + folderIds.length
        : fileIds?.length || folderIds?.length;

    if (
      selectionFilesLength &&
      isEmptyLastPageAfterOperation(selectionFilesLength)
    ) {
      newFilter = resetFilterPage();
    }

    let updatedFolder = this.selectedFolderStore.id;

    if (this.dialogsStore.isFolderActions) {
      updatedFolder = this.selectedFolderStore.parentId;
    }

    if (isRoomsFolder || isArchiveFolder) {
      fetchRooms(
        updatedFolder,
        newFilter ? newFilter : roomsFilter.clone()
      ).finally(() => {
        this.dialogsStore.setIsFolderActions(false);
        return setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
    } else {
      fetchFiles(
        updatedFolder,
        newFilter ? newFilter : filter,
        true,
        true,
        clearSelection
      ).finally(() => {
        this.dialogsStore.setIsFolderActions(false);
        return setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
    }
  };

  convertToTree = (folders) => {
    let result = [];
    let level = { result };
    try {
      folders.forEach((folder) => {
        folder.path
          .split("/")
          .filter((name) => name !== "")
          .reduce((r, name, i, a) => {
            if (!r[name]) {
              r[name] = { result: [] };
              r.result.push({ name, children: r[name].result });
            }

            return r[name];
          }, level);
      });
    } catch (e) {
      console.error("convertToTree", e);
    }
    return result;
  };

  createFolderTree = async (treeList, parentFolderId) => {
    if (!treeList || !treeList.length) return;

    for (let i = 0; i < treeList.length; i++) {
      const treeNode = treeList[i];

      // console.log(
      //   `createFolderTree parent id = ${parentFolderId} name '${treeNode.name}': `,
      //   treeNode.children
      // );

      const folder = await createFolder(parentFolderId, treeNode.name);
      const parentId = folder.id;

      if (treeNode.children.length == 0) continue;

      await this.createFolderTree(treeNode.children, parentId);
    }
  };

  uploadEmptyFolders = async (emptyFolders, folderId) => {
    //console.log("uploadEmptyFolders", emptyFolders, folderId);

    const { secondaryProgressDataStore } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    const toFolderId = folderId ? folderId : this.selectedFolderStore.id;

    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label: "",
      alert: false,
    });

    const tree = this.convertToTree(emptyFolders);
    await this.createFolderTree(tree, toFolderId);

    this.updateCurrentFolder(null, [folderId]);

    setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
  };

  updateFilesAfterDelete = () => {
    const { setSelected } = this.filesStore;
    const {
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    setSelected("close");

    this.dialogsStore.setIsFolderActions(false);
    setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
  };

  deleteAction = async (
    translations,
    newSelection = null,
    withoutDialog = false
  ) => {
    const { isRecycleBinFolder, isPrivacyFolder } = this.treeFoldersStore;
    const { addActiveItems, getIsEmptyTrash } = this.filesStore;
    const {
      secondaryProgressDataStore,
      clearActiveOperations,
    } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;
    const { withPaging } = this.authStore.settingsStore;

    const selection = newSelection ? newSelection : this.filesStore.selection;
    const isThirdPartyFile = selection.some((f) => f.providerKey);

    const currentFolderId = this.selectedFolderStore.id;

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
      filesCount: selection.length,
    });

    const deleteAfter = false; //Delete after finished TODO: get from settings
    const immediately = isRecycleBinFolder || isPrivacyFolder ? true : false; //Don't move to the Recycle Bin

    let folderIds = [];
    let fileIds = [];

    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst || selection[i].contentLength) {
        fileIds.push(selection[i].id);
      } else {
        folderIds.push(selection[i].id);
      }
      i++;
    }

    addActiveItems(fileIds);
    addActiveItems(null, folderIds);

    if (this.dialogsStore.isFolderActions && withoutDialog) {
      folderIds = [];
      fileIds = [];

      folderIds.push(selection[0]);
    }

    if (folderIds.length || fileIds.length) {
      this.isMediaOpen();

      try {
        this.filesStore.setOperationAction(true);
        await removeFiles(folderIds, fileIds, deleteAfter, immediately)
          .then(async (res) => {
            if (res[0]?.error) return Promise.reject(res[0].error);
            const data = res[0] ? res[0] : null;
            const pbData = {
              icon: "trash",
              label: translations.deleteOperation,
            };
            await this.uploadDataStore.loopFilesOperations(data, pbData);

            const showToast = () => {
              if (isRecycleBinFolder) {
                return toastr.success(translations.deleteFromTrash);
              }

              if (selection.length > 1 || isThirdPartyFile) {
                return toastr.success(translations.deleteSelectedElem);
              }
              if (selection[0].fileExst) {
                return toastr.success(translations.FileRemoved);
              }
              return toastr.success(translations.FolderRemoved);
            };

            if (withPaging || this.dialogsStore.isFolderActions) {
              this.updateCurrentFolder(fileIds, folderIds, false);
              showToast();
            } else {
              this.updateFilesAfterDelete(folderIds);
              this.filesStore.removeFiles(fileIds, folderIds, showToast);
            }

            if (currentFolderId) {
              const { socketHelper } = this.authStore.settingsStore;

              socketHelper.emit({
                command: "refresh-folder",
                data: currentFolderId,
              });
            }
          })
          .finally(() => {
            clearActiveOperations(fileIds, folderIds);
            getIsEmptyTrash();
          });
      } catch (err) {
        clearActiveOperations(fileIds, folderIds);
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        return toastr.error(err.message ? err.message : err);
      } finally {
        this.filesStore.setOperationAction(false);
      }
    }
  };

  emptyTrash = async (translations) => {
    const {
      secondaryProgressDataStore,
      loopFilesOperations,
      clearActiveOperations,
    } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;
    const { isRecycleBinFolder } = this.treeFoldersStore;
    const { addActiveItems, files, folders, getIsEmptyTrash } = this.filesStore;

    const fileIds = files.map((f) => f.id);
    const folderIds = folders.map((f) => f.id);
    if (isRecycleBinFolder) addActiveItems(fileIds, folderIds);

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
    });

    try {
      await emptyTrash().then(async (res) => {
        if (res[0]?.error) return Promise.reject(res[0].error);
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "trash",
          label: translations.deleteOperation,
        };
        await loopFilesOperations(data, pbData);
        toastr.success(translations.successOperation);
        this.updateCurrentFolder(fileIds, folderIds);
        getIsEmptyTrash();
        clearActiveOperations(fileIds, folderIds);
      });
    } catch (err) {
      clearActiveOperations(fileIds, folderIds);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
      });
      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      return toastr.error(err.message ? err.message : err);
    }
  };

  emptyArchive = async (translations) => {
    const {
      secondaryProgressDataStore,
      loopFilesOperations,
      clearActiveOperations,
    } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;
    const { isArchiveFolder } = this.treeFoldersStore;
    const { addActiveItems, roomsForDelete } = this.filesStore;

    const folderIds = roomsForDelete.map((f) => f.id);
    if (isArchiveFolder) addActiveItems(null, folderIds);

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
    });

    try {
      await removeFiles(folderIds, [], true, true).then(async (res) => {
        if (res[0]?.error) return Promise.reject(res[0].error);
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "trash",
          label: translations.deleteOperation,
        };
        await loopFilesOperations(data, pbData);
        toastr.success(translations.successOperation);
        this.updateCurrentFolder(null, folderIds);
        // getIsEmptyTrash();
        clearActiveOperations(null, folderIds);
      });
    } catch (err) {
      clearActiveOperations(null, folderIds);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
      });
      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);

      return toastr.error(err.message ? err.message : err);
    }
  };

  downloadFiles = async (fileConvertIds, folderIds, translations) => {
    const {
      clearActiveOperations,
      secondaryProgressDataStore,
    } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    const { addActiveItems } = this.filesStore;
    const { label } = translations;

    if (this.isBulkDownload) {
      //toastr.error(); TODO: new add cancel download operation and new translation "ErrorMassage_SecondDownload"
      return;
    }

    this.setIsBulkDownload(true);

    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label,
      alert: false,
    });

    const fileIds = fileConvertIds.map((f) => f.key || f);
    addActiveItems(fileIds, folderIds);

    try {
      await downloadFiles(fileConvertIds, folderIds).then(async (res) => {
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "file",
          label,
        };

        const item =
          data?.finished && data?.url
            ? data
            : await this.uploadDataStore.loopFilesOperations(
                data,
                pbData,
                true
              );

        clearActiveOperations(fileIds, folderIds);
        this.setIsBulkDownload(false);

        if (item.url) {
          window.location.href = item.url;
        } else {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
        }

        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        !item.url && toastr.error(translations.error, null, 0, true);
      });
    } catch (err) {
      this.setIsBulkDownload(false);
      clearActiveOperations(fileIds, folderIds);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
      });
      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      return toastr.error(err.message ? err.message : err);
    }
  };

  downloadAction = (label, folderId) => {
    const { bufferSelection } = this.filesStore;

    const selection = this.filesStore.selection.length
      ? this.filesStore.selection
      : [bufferSelection];

    let fileIds = [];
    let folderIds = [];
    const items = [];

    if (selection.length === 1 && selection[0].fileExst && !folderId) {
      window.open(selection[0].viewUrl, "_self");
      return Promise.resolve();
    }

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
        items.push({ id: item.id, fileExst: item.fileExst });
      } else {
        folderIds.push(item.id);
        items.push({ id: item.id });
      }
    }

    if (this.dialogsStore.isFolderActions) {
      fileIds = [];
      folderIds = [];

      folderIds.push(bufferSelection);
      this.dialogsStore.setIsFolderActions(false);
    }

    return this.downloadFiles(fileIds, folderIds, label);
  };

  editCompleteAction = async (selectedItem, type, isFolder = false) => {
    if (type === FileAction.Create) {
      this.filesStore.addFile(selectedItem, isFolder);
    }

    if (type === FileAction.Create || type === FileAction.Rename) {
      type === FileAction.Rename &&
        this.onSelectItem(
          {
            id: selectedItem.id,
            isFolder: selectedItem.isFolder,
          },
          false
        );
    }
  };

  onSelectItem = (
    { id, isFolder },
    withSelect = true,
    isContextItem = true,
    isSingleMenu = false
  ) => {
    const {
      setBufferSelection,
      setSelected,
      selection,
      setSelection,
      setHotkeyCaretStart,
      setHotkeyCaret,
      setEnabledHotkeys,
      filesList,
    } = this.filesStore;

    if (!id) return;

    const item = filesList.find(
      (elm) => elm.id === id && elm.isFolder === isFolder
    );

    if (item) {
      const isSelected =
        selection.findIndex((f) => f.id === id && f.isFolder === isFolder) !==
        -1;

      if (withSelect) {
        //TODO: fix double event on context-menu click
        if (isSelected && selection.length === 1 && !isContextItem) {
          setSelected("none");
        } else {
          setSelection([item]);
          setHotkeyCaret(null);
          setHotkeyCaretStart(null);
        }
      } else if (
        isSelected &&
        selection.length > 1 &&
        !isContextItem &&
        !isSingleMenu
      ) {
        setHotkeyCaret(null);
        setHotkeyCaretStart(null);
      } else {
        setSelected("none");
        setBufferSelection(item);
      }

      isContextItem && setEnabledHotkeys(false);
    }
  };

  deleteItemAction = async (
    itemId,
    translations,
    isFile,
    isThirdParty,
    isRoom
  ) => {
    const {
      secondaryProgressDataStore,
      clearActiveOperations,
    } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;
    if (
      this.settingsStore.confirmDelete ||
      this.treeFoldersStore.isPrivacyFolder ||
      isThirdParty
    ) {
      this.dialogsStore.setIsRoomDelete(isRoom);
      this.dialogsStore.setDeleteDialogVisible(true);
    } else {
      setSecondaryProgressBarData({
        icon: "trash",
        visible: true,
        percent: 0,
        label: translations?.deleteOperation,
        alert: false,
      });

      try {
        await this.deleteItemOperation(isFile, itemId, translations, isRoom);

        const id = Array.isArray(itemId) ? itemId : [itemId];

        clearActiveOperations(isFile && id, !isFile && id);
      } catch (err) {
        clearActiveOperations(isFile && [itemId], !isFile && [itemId]);
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        return toastr.error(err.message ? err.message : err);
      }
    }
  };

  deleteItemOperation = (isFile, itemId, translations, isRoom) => {
    const { addActiveItems, getIsEmptyTrash } = this.filesStore;
    const { withPaging } = this.authStore.settingsStore;

    const pbData = {
      icon: "trash",
      label: translations?.deleteOperation,
    };

    this.filesStore.setOperationAction(true);

    if (isFile) {
      addActiveItems([itemId]);
      this.isMediaOpen();
      return deleteFile(itemId)
        .then(async (res) => {
          if (res[0]?.error) return Promise.reject(res[0].error);
          const data = res[0] ? res[0] : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);

          if (withPaging) {
            this.updateCurrentFolder([itemId]);
            toastr.success(translations.successRemoveFile);
          } else {
            this.updateFilesAfterDelete();
            this.filesStore.removeFiles([itemId], null, () =>
              toastr.success(translations.successRemoveFile)
            );
          }
        })
        .finally(() => this.filesStore.setOperationAction(false));
    } else if (isRoom) {
      const items = Array.isArray(itemId) ? itemId : [itemId];
      addActiveItems(null, items);

      const actions = items.map((item) => deleteRoom(item));

      return Promise.all(actions)
        .then(async (res) => {
          if (res[0]?.error) return Promise.reject(res[0].error);
          const data = res ? res : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);
          this.updateCurrentFolder(null, [itemId]);
        })
        .then(() =>
          toastr.success(
            items.length > 1
              ? translations?.successRemoveRooms
              : translations?.successRemoveRoom
          )
        );
    } else {
      addActiveItems(null, [itemId]);
      return deleteFolder(itemId)
        .then(async (res) => {
          if (res[0]?.error) return Promise.reject(res[0].error);
          const data = res[0] ? res[0] : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);

          if (withPaging) {
            this.updateCurrentFolder(null, [itemId]);
            toastr.success(translations.successRemoveFolder);
          } else {
            this.updateFilesAfterDelete([itemId]);
            this.filesStore.removeFiles(null, [itemId], () =>
              toastr.success(translations.successRemoveFolder)
            );
          }

          getIsEmptyTrash();
        })
        .finally(() => this.filesStore.setOperationAction(false));
    }
  };

  unsubscribeAction = async (fileIds, folderIds) => {
    const { setUnsubscribe } = this.dialogsStore;
    const { filter, fetchFiles } = this.filesStore;

    return removeShareFiles(fileIds, folderIds)
      .then(() => setUnsubscribe(false))
      .then(() => fetchFiles(this.selectedFolderStore.id, filter, true, true));
  };

  lockFileAction = (id, locked) => {
    const { setFile } = this.filesStore;
    return lockFile(id, locked).then((res) => setFile(res));
  };

  finalizeVersionAction = (id) => {
    const { setFile } = this.filesStore;

    return finalizeVersion(id, 0, false).then((res) => {
      if (res && res[0]) {
        setFile(res[0]);
      }
    });
  };

  duplicateAction = (item, label) => {
    const {
      setSecondaryProgressBarData,
      filesCount,
    } = this.uploadDataStore.secondaryProgressDataStore;

    this.setSelectedItems();

    //TODO: duplicate for folders?
    const folderIds = [];
    const fileIds = [];
    item.fileExst ? fileIds.push(item.id) : folderIds.push(item.id);
    const conflictResolveType = ConflictResolveType.Duplicate;
    const deleteAfter = false; //TODO: get from settings

    setSecondaryProgressBarData({
      icon: "duplicate",
      visible: true,
      percent: 0,
      label,
      alert: false,
      filesCount: filesCount + fileIds.length,
    });

    this.filesStore.addActiveItems(fileIds, folderIds);

    return this.uploadDataStore.copyToAction(
      this.selectedFolderStore.id,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    );
  };

  getFilesInfo = (items) => {
    const requests = [];
    let i = items.length;
    while (i !== 0) {
      requests.push(this.filesStore.getFileInfo(items[i - 1]));
      i--;
    }
    return Promise.all(requests);
  };

  setFavoriteAction = (action, id) => {
    const {
      markItemAsFavorite,
      removeItemFromFavorite,
      fetchFavoritesFolder,
      setSelected,
    } = this.filesStore;

    const items = Array.isArray(id) ? id : [id];

    switch (action) {
      case "mark":
        return markItemAsFavorite(items)
          .then(() => {
            return this.getFilesInfo(items);
          })
          .then(() => setSelected("close"));

      case "remove":
        return removeItemFromFavorite(items)
          .then(() => {
            return this.treeFoldersStore.isFavoritesFolder
              ? fetchFavoritesFolder(this.selectedFolderStore.id)
              : this.getFilesInfo(items);
          })
          .then(() => setSelected("close"));
      default:
        return;
    }
  };

  setPinAction = (action, id, t) => {
    const { pinRoom, unpinRoom, updateRoomPin, setSelected } = this.filesStore;

    const { selection, setSelection } = this.authStore.infoPanelStore;

    const items = Array.isArray(id) ? id : [id];

    const actions = [];

    switch (action) {
      case "pin":
        items.forEach((item) => {
          updateRoomPin(item);
          actions.push(pinRoom(item));
        });

        return Promise.all(actions)
          .then(() => {
            this.updateCurrentFolder(null, items);
            if (selection) {
              setSelection({ ...selection, pinned: true });
            }
          })
          .then(() => setSelected("close"))
          .finally(() => toastr.success(t("RoomPinned")));
      case "unpin":
        items.forEach((item) => {
          updateRoomPin(item);
          actions.push(unpinRoom(item));
        });
        return Promise.all(actions)
          .then(() => {
            this.updateCurrentFolder(null, items);
            if (selection) {
              setSelection({ ...selection, pinned: false });
            }
          })
          .then(() => setSelected("close"))
          .finally(() => toastr.success(t("RoomUnpinned")));
      default:
        return;
    }
  };

  setArchiveAction = async (action, folders, t) => {
    const {
      addActiveItems,
      moveRoomToArchive,
      removeRoomFromArchive,
      setSelected,
    } = this.filesStore;

    const { setSelectedFolder } = this.selectedFolderStore;

    const { roomsFolder, isRoomsFolder } = this.treeFoldersStore;
    const { setPortalQuota } = this.authStore.currentQuotaStore;

    const {
      secondaryProgressDataStore,
      clearActiveOperations,
    } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    const items = Array.isArray(folders)
      ? folders.map((x) => (x?.id ? x.id : x))
      : [folders.id];

    setSecondaryProgressBarData({
      icon: "move",
      visible: true,
      percent: 0,
      label: "Archive room",
      alert: false,
    });

    addActiveItems(null, items);

    const actions = [];

    switch (action) {
      case "archive":
        items.forEach((item) => {
          actions.push(moveRoomToArchive(item));
        });

        return Promise.all(actions)
          .then(async (res) => {
            if (res[0]?.error) return Promise.reject(res[0].error);

            const pbData = {
              label: "Archive room operation",
            };
            const data = res ? res : null;
            await this.uploadDataStore.loopFilesOperations(data, pbData);

            if (!isRoomsFolder) {
              setSelectedFolder(roomsFolder);
            }

            this.updateCurrentFolder();
          })
          .then(() => setPortalQuota())
          .then(() => {
            const successTranslation =
              folders.length !== 1 && Array.isArray(folders)
                ? t("ArchivedRoomsAction")
                : Array.isArray(folders)
                ? t("ArchivedRoomAction", { name: folders[0].title })
                : t("ArchivedRoomAction", { name: folders.title });

            toastr.success(successTranslation);
          })
          .then(() => setSelected("close"))
          .catch((err) => {
            clearActiveOperations(null, items);
            setSecondaryProgressBarData({
              visible: true,
              alert: true,
            });
            setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
            return toastr.error(err.message ? err.message : err);
          })
          .finally(() => clearActiveOperations(null, items));
      case "unarchive":
        items.forEach((item) => {
          actions.push(removeRoomFromArchive(item));
        });
        return Promise.all(actions)
          .then(async (res) => {
            if (res[0]?.error) return Promise.reject(res[0].error);

            const pbData = {
              label: "Archive room operation",
            };
            const data = res ? res : null;
            await this.uploadDataStore.loopFilesOperations(data, pbData);
            this.updateCurrentFolder(null, [items]);
          })
          .then(() => setPortalQuota())
          .then(() => {
            const successTranslation =
              folders.length !== 1 && Array.isArray(folders)
                ? t("UnarchivedRoomsAction")
                : Array.isArray(folders)
                ? t("UnarchivedRoomAction", { name: folders[0].title })
                : t("UnarchivedRoomAction", { name: folders.title });

            toastr.success(successTranslation);
          })
          .then(() => setSelected("close"))
          .catch((err) => {
            clearActiveOperations(null, items);
            setSecondaryProgressBarData({
              visible: true,
              alert: true,
            });
            setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
            return toastr.error(err.message ? err.message : err);
          })
          .finally(() => clearActiveOperations(null, items));
      default:
        return;
    }
  };

  selectTag = (tag) => {
    const { roomsFilter, fetchRooms, setIsLoading } = this.filesStore;

    const { id } = this.selectedFolderStore;

    const newFilter = roomsFilter.clone();

    if (tag !== "no-tag") {
      const tags = newFilter.tags ? [...newFilter.tags] : [];

      if (tags.length > 0) {
        const idx = tags.findIndex((item) => item === tag);

        if (idx > -1) {
          //TODO: remove tag here if already selected
          return;
        }
      }
      tags.push(tag);

      newFilter.tags = [...tags];
      newFilter.withoutTags = false;
    } else {
      newFilter.withoutTags = true;
    }

    setIsLoading(true);

    fetchRooms(id, newFilter).finally(() => setIsLoading(false));
  };

  selectOption = ({ option, value }) => {
    const { roomsFilter, fetchRooms, setIsLoading } = this.filesStore;
    const { id } = this.selectedFolderStore;

    const newFilter = roomsFilter.clone();
    const tags = newFilter.tags ? [...newFilter.tags] : [];
    newFilter.tags = [...tags];

    if (option === "defaultTypeRoom") {
      newFilter.type = value;
    }

    if (option === "typeProvider") {
      newFilter.provider = value;
    }

    setIsLoading(true);
    fetchRooms(id, newFilter).finally(() => setIsLoading(false));
  };

  selectRowAction = (checked, file) => {
    const {
      // selected,
      // setSelected,
      selectFile,
      deselectFile,
      setBufferSelection,
    } = this.filesStore;
    //selected === "close" && setSelected("none");
    setBufferSelection(null);

    if (checked) {
      selectFile(file);
    } else {
      deselectFile(file);
    }
  };

  setSearchTitleOpenLocation = (searchTitleOpenLocation) => {
    this.searchTitleOpenLocation = searchTitleOpenLocation;
  };

  setItemOpenLocation = (itemOpenLocation) => {
    this.itemOpenLocation = itemOpenLocation;
  };

  setIsLoadedLocationFiles = (isLoadedLocationFiles) => {
    this.isLoadedLocationFiles = isLoadedLocationFiles;
  };

  setIsLoadedSearchFiles = (isLoadedSearchFiles) => {
    this.isLoadedSearchFiles = isLoadedSearchFiles;
  };

  openLocationAction = async (locationId) => {
    this.setIsLoadedLocationFiles(false);
    this.filesStore.setBufferSelection(null);

    const files = await this.filesStore.fetchFiles(locationId, null);
    this.setIsLoadedLocationFiles(true);
    return files;
  };

  checkAndOpenLocationAction = async (item) => {
    const filterData = FilesFilter.getDefault();

    this.setIsLoadedSearchFiles(false);

    if (this.itemOpenLocation?.title !== item.title) {
      this.setSearchTitleOpenLocation(null);
    }

    this.setItemOpenLocation(null);

    api.files
      .getFolder(item.ExtraLocation, filterData)
      .then(() => {
        this.openLocationAction(item.ExtraLocation);
        this.setSearchTitleOpenLocation(item.title);
        this.setItemOpenLocation(item);
      })
      .catch((err) => toastr.error(err));
  };

  setThirdpartyInfo = (providerKey) => {
    const { setConnectDialogVisible, setConnectItem } = this.dialogsStore;
    const { providers, capabilities } = this.settingsStore.thirdPartyStore;
    const provider = providers.find((x) => x.provider_key === providerKey);
    const capabilityItem = capabilities.find((x) => x[0] === providerKey);
    const capability = {
      title: capabilityItem ? capabilityItem[0] : provider.customer_title,
      link: capabilityItem ? capabilityItem[1] : " ",
    };

    setConnectDialogVisible(true);
    setConnectItem({ ...provider, ...capability });
  };

  // setNewBadgeCount = (item) => {
  //   const { getRootFolder, updateRootBadge } = this.treeFoldersStore;
  //   const { updateFileBadge, updateFolderBadge } = this.filesStore;
  //   const { rootFolderType, fileExst, id } = item;

  //   const count = item.new ? item.new : 1;
  //   const rootFolder = getRootFolder(rootFolderType);
  //   updateRootBadge(rootFolder.id, count);

  //   if (fileExst) updateFileBadge(id);
  //   else updateFolderBadge(id, item.new);
  // };

  markAsRead = (folderIds, fileIds, item) => {
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    setSecondaryProgressBarData({
      icon: "file",
      label: "", //TODO: add translation if need "MarkAsRead": "Mark all as read",
      percent: 0,
      visible: true,
    });

    return markAsRead(folderIds, fileIds)
      .then(async (res) => {
        const data = res[0] ? res[0] : null;
        const pbData = { icon: "file" };
        await this.uploadDataStore.loopFilesOperations(data, pbData);
      })
      .then(() => {
        if (!item) return;

        //this.setNewBadgeCount(item);

        const { getFileIndex, updateFileStatus } = this.filesStore;

        const index = getFileIndex(item.id);
        updateFileStatus(index, item.fileStatus & ~FileStatus.IsNew);
      })
      .catch((err) => toastr.error(err))
      .finally(() => setTimeout(() => clearSecondaryProgressData(), TIMEOUT));
  };

  moveDragItems = (destFolderId, folderTitle, providerKey, translations) => {
    const folderIds = [];
    const fileIds = [];
    const deleteAfter = false;

    const { selection } = this.filesStore;
    const { isRootFolder } = this.selectedFolderStore;
    const {
      isShareFolder,
      isCommonFolder,
      isFavoritesFolder,
      isRecentFolder,
    } = this.treeFoldersStore;
    const isCopy =
      isShareFolder ||
      isFavoritesFolder ||
      isRecentFolder ||
      (!this.authStore.isAdmin && isCommonFolder);

    const operationData = {
      destFolderId,
      folderIds,
      fileIds,
      deleteAfter,
      translations,
      folderTitle,
      isCopy,
    };

    const {
      setThirdPartyMoveDialogVisible,
      setDestFolderId,
    } = this.dialogsStore;

    const provider = selection.find((x) => x.providerKey);

    if (provider && providerKey !== provider.providerKey) {
      setDestFolderId(destFolderId);
      return setThirdPartyMoveDialogVisible(true);
    }

    for (let item of selection) {
      if (!item.isFolder) {
        fileIds.push(item.id);
      } else {
        if (item.providerKey && isRootFolder) continue;
        folderIds.push(item.id);
      }
    }

    if (!folderIds.length && !fileIds.length) return;
    this.checkOperationConflict(operationData);
  };

  checkFileConflicts = (destFolderId, folderIds, fileIds) => {
    this.filesStore.addActiveItems(fileIds);
    this.filesStore.addActiveItems(null, folderIds);
    return checkFileConflicts(destFolderId, folderIds, fileIds);
  };

  setConflictDialogData = (conflicts, operationData) => {
    this.dialogsStore.setConflictResolveDialogItems(conflicts);
    this.dialogsStore.setConflictResolveDialogData(operationData);
    this.dialogsStore.setConflictResolveDialogVisible(true);
  };

  setSelectedItems = () => {
    const selectionLength = this.filesStore.selection.length;
    const selectionTitle = this.filesStore.selectionTitle;

    if (selectionLength !== undefined && selectionTitle) {
      this.uploadDataStore.secondaryProgressDataStore.setItemsSelectionLength(
        selectionLength
      );
      this.uploadDataStore.secondaryProgressDataStore.setItemsSelectionTitle(
        selectionTitle
      );
    }
  };

  checkOperationConflict = async (operationData) => {
    const { destFolderId, folderIds, fileIds } = operationData;
    const { setBufferSelection } = this.filesStore;

    this.setSelectedItems();

    this.filesStore.setSelected("none");
    let conflicts;

    try {
      conflicts = await this.checkFileConflicts(
        destFolderId,
        folderIds,
        fileIds
      );
    } catch (err) {
      setBufferSelection(null);
      return toastr.error(err.message ? err.message : err);
    }

    if (conflicts.length) {
      this.setConflictDialogData(conflicts, operationData);
    } else {
      try {
        await this.uploadDataStore.itemOperationToFolder(operationData);
      } catch (err) {
        setBufferSelection(null);
        return toastr.error(err.message ? err.message : err);
      }
    }
  };

  isAvailableOption = (option) => {
    const {
      canConvertSelected,
      hasSelection,
      allFilesIsEditing,
      selection,
    } = this.filesStore;

    const { rootFolderType } = this.selectedFolderStore;

    switch (option) {
      case "copy":
        const canCopy = selection.map((s) => s.security?.Copy).filter((s) => s);

        return hasSelection && canCopy;
      case "showInfo":
      case "download":
        return hasSelection;
      case "downloadAs":
        return canConvertSelected;
      case "moveTo":
        const canMove = selection.every((s) => s.security?.Move);

        return (
          hasSelection &&
          !allFilesIsEditing &&
          canMove &&
          rootFolderType !== FolderType.TRASH
        );

      case "archive":
      case "unarchive":
        const canArchive = selection
          .map((s) => s.security?.Move)
          .filter((s) => s);

        return canArchive.length > 0;
      case "delete-room":
        const canRemove = selection
          .map((s) => s.security?.Delete)
          .filter((r) => r);

        return canRemove.length > 0;

      case "delete":
        const canDelete = selection.every((s) => s.security?.Delete);

        return !allFilesIsEditing && canDelete && hasSelection;
    }
  };

  convertToArray = (itemsCollection) => {
    const result = Array.from(itemsCollection.values()).filter((item) => {
      return item != null;
    });

    itemsCollection.clear();

    return result;
  };

  pinRooms = (t) => {
    const { selection } = this.filesStore;

    const items = [];

    selection.forEach((item) => {
      if (!item.pinned) items.push(item.id);
    });

    this.setPinAction("pin", items, t);
  };

  unpinRooms = (t) => {
    const { selection } = this.filesStore;

    const items = [];

    selection.forEach((item) => {
      if (item.pinned) items.push(item.id);
    });

    this.setPinAction("unpin", items, t);
  };

  archiveRooms = (action) => {
    const { setArchiveAction, setArchiveDialogVisible } = this.dialogsStore;

    setArchiveAction(action);
    setArchiveDialogVisible(true);
  };

  deleteRooms = (t) => {
    const { selection } = this.filesStore;

    const items = [];

    selection.forEach((item) => {
      items.push(item.id);
    });

    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      successRemoveFile: t("Files:FileRemoved"),
      successRemoveFolder: t("Files:FolderRemoved"),
      successRemoveRoom: t("Files:RoomRemoved"),
      successRemoveRooms: t("Files:RoomsRemoved"),
    };

    this.deleteItemAction(items, translations, null, null, true);
  };

  deleteRoomsAction = async (itemId, translations) => {
    const {
      secondaryProgressDataStore,
      clearActiveOperations,
    } = this.uploadDataStore;

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations?.deleteOperation,
      alert: false,
    });

    try {
      await this.deleteItemOperation(false, itemId, translations, true);

      const id = Array.isArray(itemId) ? itemId : [itemId];

      clearActiveOperations(null, id);
    } catch (err) {
      console.log(err);
      clearActiveOperations(null, [itemId]);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
      });
      setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      onClose();
      return toastr.error(err.message ? err.message : err);
    }
  };

  onShowInfoPanel = () => {
    const { selection } = this.filesStore;
    const { setSelection, setIsVisible } = this.authStore.infoPanelStore;

    setSelection([selection]);
    setIsVisible(true);
  };

  getOption = (option, t) => {
    const {
      setSharingPanelVisible,
      setDownloadDialogVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setDeleteDialogVisible,
    } = this.dialogsStore;

    switch (option) {
      case "show-info":
        if (!isTablet() && !isMobile) return null;
        else
          return {
            id: "menu-show-info",
            key: "show-info",
            label: t("Common:Info"),
            iconUrl: "/static/images/info.outline.react.svg",
            onClick: this.onShowInfoPanel,
          };
      case "copy":
        if (!this.isAvailableOption("copy")) return null;
        else
          return {
            id: "menu-copy",
            label: t("Translations:Copy"),
            onClick: () => setCopyPanelVisible(true),
            iconUrl: "/static/images/copyTo.react.svg",
          };

      case "download":
        if (!this.isAvailableOption("download")) return null;
        else
          return {
            id: "menu-download",
            label: t("Common:Download"),
            onClick: () =>
              this.downloadAction(
                t("Translations:ArchivingData")
              ).catch((err) => toastr.error(err)),
            iconUrl: "/static/images/download.react.svg",
          };

      case "downloadAs":
        if (!this.isAvailableOption("downloadAs")) return null;
        else
          return {
            id: "menu-download-as",
            label: t("Translations:DownloadAs"),
            onClick: () => setDownloadDialogVisible(true),
            iconUrl: "/static/images/downloadAs.react.svg",
          };

      case "moveTo":
        if (!this.isAvailableOption("moveTo")) return null;
        else
          return {
            id: "menu-move-to",
            label: t("MoveTo"),
            onClick: () => setMoveToPanelVisible(true),
            iconUrl: "/static/images/move.react.svg",
          };
      case "pin":
        return {
          id: "menu-pin",
          key: "pin",
          label: t("Pin"),
          iconUrl: "/static/images/pin.react.svg",
          onClick: () => this.pinRooms(t),
          disabled: false,
        };
      case "unpin":
        return {
          id: "menu-unpin",
          key: "unpin",
          label: t("Unpin"),
          iconUrl: "/static/images/unpin.react.svg",
          onClick: () => this.unpinRooms(t),
          disabled: false,
        };
      case "archive":
        if (!this.isAvailableOption("archive")) return null;
        else
          return {
            id: "menu-archive",
            key: "archive",
            label: t("Archived"),
            iconUrl: "/static/images/room.archive.svg",
            onClick: () => this.archiveRooms("archive"),
            disabled: false,
          };
      case "unarchive":
        if (!this.isAvailableOption("unarchive")) return null;
        else
          return {
            id: "menu-unarchive",
            key: "unarchive",
            label: t("Common:Restore"),
            iconUrl: "images/subtract.react.svg",
            onClick: () => this.archiveRooms("unarchive"),
            disabled: false,
          };
      case "delete-room":
        if (!this.isAvailableOption("delete-room")) return null;
        else
          return {
            id: "menu-delete-room",
            label: t("Common:Delete"),
            onClick: () => this.deleteRooms(t),
            iconUrl: "/static/images/delete.react.svg",
          };

      case "delete":
        if (!this.isAvailableOption("delete")) return null;
        else
          return {
            id: "menu-delete",
            label: t("Common:Delete"),
            onClick: () => {
              if (this.settingsStore.confirmDelete) {
                setDeleteDialogVisible(true);
              } else {
                const translations = {
                  deleteOperation: t("Translations:DeleteOperation"),
                  deleteFromTrash: t("Translations:DeleteFromTrash"),
                  deleteSelectedElem: t("Translations:DeleteSelectedElem"),
                  FileRemoved: t("Files:FileRemoved"),
                  FolderRemoved: t("Files:FolderRemoved"),
                };

                this.deleteAction(translations).catch((err) =>
                  toastr.error(err)
                );
              }
            },
            iconUrl: "/static/images/delete.react.svg",
          };
    }
  };

  getRoomsFolderOptions = (itemsCollection, t) => {
    let pinName = "unpin";
    const { selection } = this.filesStore;

    selection.forEach((item) => {
      if (!item.pinned) pinName = "pin";
    });

    const pin = this.getOption(pinName, t);
    const archive = this.getOption("archive", t);

    itemsCollection.set(pinName, pin).set("archive", archive);
    return this.convertToArray(itemsCollection);
  };

  getArchiveRoomsFolderOptions = (itemsCollection, t) => {
    const archive = this.getOption("unarchive", t);
    const deleteOption = this.getOption("delete-room", t);
    const showOption = this.getOption("show-info", t);

    itemsCollection
      .set("unarchive", archive)
      .set("show-info", showOption)
      .set("delete", deleteOption);

    return this.convertToArray(itemsCollection);
  };

  getAnotherFolderOptions = (itemsCollection, t) => {
    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const moveTo = this.getOption("moveTo", t);
    const copy = this.getOption("copy", t);
    const deleteOption = this.getOption("delete", t);
    const showInfo = this.getOption("showInfo", t);

    itemsCollection
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("moveTo", moveTo)
      .set("copy", copy)
      .set("delete", deleteOption)
      .set("showInfo", showInfo);

    return this.convertToArray(itemsCollection);
  };

  getRecentFolderOptions = (itemsCollection, t) => {
    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const copy = this.getOption("copy", t);
    const showInfo = this.getOption("showInfo", t);

    itemsCollection

      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("copy", copy)
      .set("showInfo", showInfo);

    return this.convertToArray(itemsCollection);
  };

  getShareFolderOptions = (itemsCollection, t) => {
    const { setDeleteDialogVisible, setUnsubscribe } = this.dialogsStore;

    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const copy = this.getOption("copy", t);
    const showInfo = this.getOption("showInfo", t);

    itemsCollection

      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("copy", copy)
      .set("delete", {
        label: t("RemoveFromList"),
        onClick: () => {
          setUnsubscribe(true);
          setDeleteDialogVisible(true);
        },
      })
      .set("showInfo", showInfo);

    return this.convertToArray(itemsCollection);
  };

  getPrivacyFolderOption = (itemsCollection, t) => {
    const moveTo = this.getOption("moveTo", t);
    const deleteOption = this.getOption("delete", t);
    const download = this.getOption("download", t);
    const showInfo = this.getOption("showInfo", t);

    itemsCollection
      .set("download", download)
      .set("moveTo", moveTo)

      .set("delete", deleteOption)
      .set("showInfo", showInfo);

    return this.convertToArray(itemsCollection);
  };

  getFavoritesFolderOptions = (itemsCollection, t) => {
    const { selection } = this.filesStore;
    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const copy = this.getOption("copy", t);
    const showInfo = this.getOption("showInfo", t);

    itemsCollection
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("copy", copy)
      .set("delete", {
        label: t("RemoveFromFavorites"),
        alt: t("RemoveFromFavorites"),
        iconUrl: "images/favorites.react.svg",
        onClick: () => {
          const items = selection.map((item) => item.id);
          this.setFavoriteAction("remove", items)
            .then(() => toastr.success(t("RemovedFromFavorites")))
            .catch((err) => toastr.error(err));
        },
      })
      .set("showInfo", showInfo);

    return this.convertToArray(itemsCollection);
  };

  getRecycleBinFolderOptions = (itemsCollection, t) => {
    const {
      setEmptyTrashDialogVisible,
      setMoveToPanelVisible,
    } = this.dialogsStore;

    const download = this.getOption("download", t);
    const downloadAs = this.getOption("downloadAs", t);
    const deleteOption = this.getOption("delete", t);
    const showInfo = this.getOption("showInfo", t);

    itemsCollection
      .set("download", download)
      .set("downloadAs", downloadAs)
      .set("restore", {
        id: "menu-restore",
        label: t("Common:Restore"),
        onClick: () => setMoveToPanelVisible(true),
        iconUrl: "/static/images/move.react.svg",
      })
      .set("delete", deleteOption)
      .set("showInfo", showInfo);

    return this.convertToArray(itemsCollection);
  };

  getHeaderMenu = (t) => {
    const {
      isFavoritesFolder,
      isRecentFolder,
      isRecycleBinFolder,
      isPrivacyFolder,
      isShareFolder,
      isRoomsFolder,
      isArchiveFolder,
    } = this.treeFoldersStore;

    let itemsCollection = new Map();

    if (isRecycleBinFolder)
      return this.getRecycleBinFolderOptions(itemsCollection, t);

    if (isFavoritesFolder)
      return this.getFavoritesFolderOptions(itemsCollection, t);

    if (isPrivacyFolder) return this.getPrivacyFolderOption(itemsCollection, t);

    if (isShareFolder) return this.getShareFolderOptions(itemsCollection, t);

    if (isRecentFolder) return this.getRecentFolderOptions(itemsCollection, t);

    if (isArchiveFolder)
      return this.getArchiveRoomsFolderOptions(itemsCollection, t);

    if (isRoomsFolder) return this.getRoomsFolderOptions(itemsCollection, t);

    return this.getAnotherFolderOptions(itemsCollection, t);
  };

  onMarkAsRead = (item) => this.markAsRead([], [`${item.id}`], item);

  openFileAction = (item) => {
    const {
      isLoading,
      setIsLoading,
      fetchFiles,
      openDocEditor,
      isPrivacyFolder,
    } = this.filesStore;
    const { isRecycleBinFolder } = this.treeFoldersStore;
    const { setMediaViewerData } = this.mediaViewerDataStore;
    const { setConvertDialogVisible, setConvertItem } = this.dialogsStore;

    const isMediaOrImage =
      item.viewAccessability?.ImageView || item.viewAccessability?.MediaView;
    const canConvert = item.viewAccessability?.Convert;
    const canWebEdit = item.viewAccessability?.WebEdit;
    const canViewedDocs = item.viewAccessability?.WebView;

    const { id, viewUrl, providerKey, fileStatus, encrypted, isFolder } = item;
    if (encrypted && isPrivacyFolder) return checkProtocol(item.id, true);

    if (isRecycleBinFolder || isLoading) return;

    if (isFolder) {
      setIsLoading(true);

      fetchFiles(id, null, true, false)
        .catch((err) => {
          toastr.error(err);
          setIsLoading(false);
        })
        .finally(() => setIsLoading(false));
    } else {
      if (canConvert) {
        setConvertItem(item);
        setConvertDialogVisible(true);
        return;
      }

      if ((fileStatus & FileStatus.IsNew) === FileStatus.IsNew)
        this.onMarkAsRead(item);

      if (canWebEdit || canViewedDocs) {
        let tab =
          !this.authStore.settingsStore.isDesktopClient && !isFolder
            ? window.open(
                combineUrl(
                  AppServerConfig.proxyURL,
                  config.homepage,
                  `/doceditor?fileId=${id}`
                ),
                "_blank"
              )
            : null;

        return openDocEditor(id, providerKey, tab);
      }

      if (isMediaOrImage) {
        localStorage.setItem("isFirstUrl", window.location.href);
        setMediaViewerData({ visible: true, id });

        const url = "/products/files/#preview/" + id;
        history.pushState(null, null, url);
        return;
      }

      return window.open(viewUrl, "_self");
    }
  };

  backToParentFolder = () => {
    const { setIsLoading, fetchFiles } = this.filesStore;

    if (!this.selectedFolderStore.parentId) return;

    setIsLoading(true);

    fetchFiles(
      this.selectedFolderStore.parentId,
      null,
      true,
      false
    ).finally(() => setIsLoading(false));
  };
}

export default FilesActionStore;
