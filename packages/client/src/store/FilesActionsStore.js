import FavoritesReactSvgUrl from "PUBLIC_DIR/images/favorites.react.svg?url";
import InfoOutlineReactSvgUrl from "PUBLIC_DIR/images/info.outline.react.svg?url";
import CopyToReactSvgUrl from "PUBLIC_DIR/images/copyTo.react.svg?url";
import DownloadReactSvgUrl from "PUBLIC_DIR/images/download.react.svg?url";
import DownloadAsReactSvgUrl from "PUBLIC_DIR/images/downloadAs.react.svg?url";
import MoveReactSvgUrl from "PUBLIC_DIR/images/move.react.svg?url";
import PinReactSvgUrl from "PUBLIC_DIR/images/pin.react.svg?url";
import UnpinReactSvgUrl from "PUBLIC_DIR/images/unpin.react.svg?url";
import RoomArchiveSvgUrl from "PUBLIC_DIR/images/room.archive.svg?url";
import DeleteReactSvgUrl from "PUBLIC_DIR/images/delete.react.svg?url";
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
  moveToFolder,
} from "@docspace/common/api/files";
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
import config from "PACKAGE_FILE";
import { isTablet } from "@docspace/components/utils/device";
import { getCategoryType } from "SRC_DIR/helpers/utils";
import { muteRoomNotification } from "@docspace/common/api/settings";
import { CategoryType } from "SRC_DIR/helpers/constants";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { RoomSearchArea } from "@docspace/common/constants";
import { getObjectByLocation } from "@docspace/common/utils";
import uniqueid from "lodash/uniqueId";

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
  isLoadedSearchFiles = false;
  isGroupMenuBlocked = false;
  emptyTrashInProgress = false;

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

  updateCurrentFolder = (fileIds, folderIds, clearSelection, operationId) => {
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

    const {
      isRoomsFolder,
      isArchiveFolder,
      isArchiveFolderRoot,
    } = this.treeFoldersStore;

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

    if (isRoomsFolder || isArchiveFolder || isArchiveFolderRoot) {
      fetchRooms(
        updatedFolder,
        newFilter ? newFilter : roomsFilter.clone()
      ).finally(() => {
        this.dialogsStore.setIsFolderActions(false);
        return setTimeout(
          () => clearSecondaryProgressData(operationId),
          TIMEOUT
        );
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
        return setTimeout(
          () => clearSecondaryProgressData(operationId),
          TIMEOUT
        );
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

    const operationId = uniqueid("operation_");

    const toFolderId = folderId ? folderId : this.selectedFolderStore.id;

    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label: "",
      alert: false,
      operationId,
    });

    const tree = this.convertToTree(emptyFolders);
    await this.createFolderTree(tree, toFolderId);

    this.updateCurrentFolder(null, [folderId], null, operationId);

    setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
  };

  updateFilesAfterDelete = (operationId) => {
    const { setSelected } = this.filesStore;
    const {
      clearSecondaryProgressData,
    } = this.uploadDataStore.secondaryProgressDataStore;

    setSelected("close");

    this.dialogsStore.setIsFolderActions(false);
    setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
  };

  deleteAction = async (
    translations,
    newSelection = null,
    withoutDialog = false
  ) => {
    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      recycleBinFolderId,
    } = this.treeFoldersStore;
    const {
      addActiveItems,
      getIsEmptyTrash,
      bufferSelection,
      activeFiles,
      activeFolders,
    } = this.filesStore;
    const {
      secondaryProgressDataStore,
      clearActiveOperations,
    } = this.uploadDataStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;
    const { withPaging } = this.authStore.settingsStore;

    const selection = newSelection
      ? newSelection
      : this.filesStore.selection.length
      ? this.filesStore.selection
      : [bufferSelection];
    const isThirdPartyFile = selection.some((f) => f.providerKey);

    const currentFolderId = this.selectedFolderStore.id;

    const operationId = uniqueid("operation_");

    const deleteAfter = false; //Delete after finished TODO: get from settings
    const immediately = isRecycleBinFolder || isPrivacyFolder ? true : false; //Don't move to the Recycle Bin

    let folderIds = [];
    let fileIds = [];

    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst || selection[i].contentLength) {
        // try to fix with one check later (see onDeleteMediaFile)
        const isActiveFile = activeFiles.find(
          (elem) => elem.id === selection[i].id
        );
        !isActiveFile && fileIds.push(selection[i].id);
      } else {
        // try to fix with one check later (see onDeleteMediaFile)
        const isActiveFolder = activeFolders.find(
          (elem) => elem.id === selection[i].id
        );
        !isActiveFolder && folderIds.push(selection[i].id);
      }
      i++;
    }

    if (!folderIds.length && !fileIds.length) return;
    const filesCount = folderIds.length + fileIds.length;

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
      filesCount,
      operationId,
    });

    const destFolderId = immediately ? null : recycleBinFolderId;

    addActiveItems(fileIds, null, destFolderId);
    addActiveItems(null, folderIds, destFolderId);

    if (this.dialogsStore.isFolderActions && withoutDialog) {
      folderIds = [];
      fileIds = [];

      folderIds.push(selection[0]);
    }

    if (folderIds.length || fileIds.length) {
      this.isMediaOpen();

      try {
        this.filesStore.setOperationAction(true);
        this.setGroupMenuBlocked(true);
        await removeFiles(folderIds, fileIds, deleteAfter, immediately)
          .then(async (res) => {
            if (res[0]?.error) return Promise.reject(res[0].error);
            const data = res[0] ? res[0] : null;
            const pbData = {
              icon: "trash",
              label: translations.deleteOperation,
              operationId,
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
              this.updateCurrentFolder(fileIds, folderIds, false, operationId);
              showToast();
            } else {
              this.updateFilesAfterDelete(operationId);

              this.filesStore.removeFiles(
                fileIds,
                folderIds,
                showToast,
                destFolderId
              );

              this.uploadDataStore.removeFiles(fileIds);
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
          operationId,
        });
        setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
        return toastr.error(err.message ? err.message : err);
      } finally {
        this.filesStore.setOperationAction(false);
        this.setGroupMenuBlocked(false);
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

    if (isRecycleBinFolder) {
      addActiveItems(fileIds, folderIds);
    }

    const operationId = uniqueid("operation_");

    this.emptyTrashInProgress = true;

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
      operationId,
    });

    try {
      await emptyTrash().then(async (res) => {
        if (res[0]?.error) return Promise.reject(res[0].error);
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "trash",
          label: translations.deleteOperation,
          operationId,
        };
        await loopFilesOperations(data, pbData);
        toastr.success(translations.successOperation);
        this.updateCurrentFolder(fileIds, folderIds, null, operationId);
        getIsEmptyTrash();
        clearActiveOperations(fileIds, folderIds);
      });
    } catch (err) {
      clearActiveOperations(fileIds, folderIds);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
        operationId,
      });
      setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
      return toastr.error(err.message ? err.message : err);
    } finally {
      this.emptyTrashInProgress = false;
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

    const operationId = uniqueid("operation_");

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations.deleteOperation,
      alert: false,
      operationId,
    });

    try {
      await removeFiles(folderIds, [], true, true).then(async (res) => {
        if (res[0]?.error) return Promise.reject(res[0].error);
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "trash",
          label: translations.deleteOperation,
          operationId,
        };
        await loopFilesOperations(data, pbData);
        toastr.success(translations.successOperation);
        this.updateCurrentFolder(null, folderIds, null, operationId);
        // getIsEmptyTrash();
        clearActiveOperations(null, folderIds);
      });
    } catch (err) {
      clearActiveOperations(null, folderIds);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
        operationId,
      });
      setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);

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

    const operationId = uniqueid("operation_");

    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label,
      alert: false,
      operationId,
    });

    const fileIds = fileConvertIds.map((f) => f.key || f);
    addActiveItems(fileIds, folderIds);

    try {
      await downloadFiles(fileConvertIds, folderIds).then(async (res) => {
        const data = res[0] ? res[0] : null;
        const pbData = {
          icon: "file",
          label,
          operationId,
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
            operationId,
          });
        }

        setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
        !item.url && toastr.error(translations.error, null, 0, true);
      });
    } catch (err) {
      this.setIsBulkDownload(false);
      clearActiveOperations(fileIds, folderIds);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
        operationId,
      });
      setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
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

    this.setGroupMenuBlocked(true);
    return this.downloadFiles(fileIds, folderIds, label).finally(() =>
      this.setGroupMenuBlocked(false)
    );
  };

  completeAction = async (selectedItem, type, isFolder = false) => {
    switch (type) {
      case FileAction.Create:
        this.filesStore.addItem(selectedItem, isFolder);
        break;
      case FileAction.Rename:
        this.onSelectItem(
          {
            id: selectedItem.id,
            isFolder: selectedItem.isFolder,
          },
          false,
          false
        );
        break;
      default:
        break;
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
          setHotkeyCaretStart(item);
        }
      } else if (
        isSelected &&
        selection.length > 1 &&
        !isContextItem &&
        !isSingleMenu
      ) {
        setHotkeyCaret(null);
        setHotkeyCaretStart(item);
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
      isThirdParty ||
      isRoom
    ) {
      this.dialogsStore.setIsRoomDelete(isRoom);
      this.dialogsStore.setDeleteDialogVisible(true);
    } else {
      const operationId = uniqueid("operation_");

      setSecondaryProgressBarData({
        icon: "trash",
        visible: true,
        percent: 0,
        label: translations?.deleteOperation,
        alert: false,
        operationId,
      });

      try {
        await this.deleteItemOperation(
          isFile,
          itemId,
          translations,
          isRoom,
          operationId
        );

        const id = Array.isArray(itemId) ? itemId : [itemId];

        clearActiveOperations(isFile && id, !isFile && id);
      } catch (err) {
        clearActiveOperations(isFile && [itemId], !isFile && [itemId]);
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
          operationId,
        });
        setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
        return toastr.error(err.message ? err.message : err);
      }
    }
  };

  deleteItemOperation = (isFile, itemId, translations, isRoom, operationId) => {
    const { addActiveItems, getIsEmptyTrash } = this.filesStore;
    const { withPaging } = this.authStore.settingsStore;
    const { isRecycleBinFolder, recycleBinFolderId } = this.treeFoldersStore;

    const pbData = {
      icon: "trash",
      label: translations?.deleteOperation,
      operationId,
    };

    this.filesStore.setOperationAction(true);

    const destFolderId = isRecycleBinFolder ? null : recycleBinFolderId;

    if (isFile) {
      addActiveItems([itemId], null, destFolderId);
      this.isMediaOpen();
      return deleteFile(itemId)
        .then(async (res) => {
          if (res[0]?.error) return Promise.reject(res[0].error);
          const data = res[0] ? res[0] : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);

          if (withPaging) {
            this.updateCurrentFolder([itemId], null, null, operationId);
            toastr.success(translations.successRemoveFile);
          } else {
            this.updateFilesAfterDelete(operationId);
            this.filesStore.removeFiles(
              [itemId],
              null,
              () => toastr.success(translations.successRemoveFile),
              destFolderId
            );
          }
        })
        .finally(() => this.filesStore.setOperationAction(false));
    } else if (isRoom) {
      const items = Array.isArray(itemId) ? itemId : [itemId];
      addActiveItems(null, items);

      this.setGroupMenuBlocked(true);
      return removeFiles(items, [], false, true)
        .then(async (res) => {
          if (res[0]?.error) return Promise.reject(res[0].error);
          const data = res ? res : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);
          this.updateCurrentFolder(null, [itemId], null, operationId);
        })
        .then(() =>
          toastr.success(
            items.length > 1
              ? translations?.successRemoveRooms
              : translations?.successRemoveRoom
          )
        )
        .finally(() => {
          this.setGroupMenuBlocked(false);
        });
    } else {
      addActiveItems(null, [itemId], destFolderId);
      return deleteFolder(itemId)
        .then(async (res) => {
          if (res[0]?.error) return Promise.reject(res[0].error);
          const data = res[0] ? res[0] : null;
          await this.uploadDataStore.loopFilesOperations(data, pbData);

          if (withPaging) {
            this.updateCurrentFolder(null, [itemId], null, operationId);
            toastr.success(translations.successRemoveFolder);
          } else {
            this.updateFilesAfterDelete(operationId);
            this.filesStore.removeFiles(
              null,
              [itemId],
              () => toastr.success(translations.successRemoveFolder),
              destFolderId
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

  lockFileAction = async (id, locked) => {
    let timer = null;
    const { setFile } = this.filesStore;
    try {
      timer = setTimeout(() => {
        this.filesStore.setActiveFiles([id]);
      }, 200);
      await lockFile(id, locked).then((res) => {
        setFile(res), this.filesStore.setActiveFiles([]);
      });
    } catch (err) {
      toastr.error(err);
    } finally {
      clearTimeout(timer);
    }
  };

  finalizeVersionAction = async (id) => {
    let timer = null;
    const { setFile } = this.filesStore;
    try {
      timer = setTimeout(() => {
        this.filesStore.setActiveFiles([id]);
      }, 200);
      await finalizeVersion(id, 0, false).then((res) => {
        if (res && res[0]) {
          setFile(res[0]);
          this.filesStore.setActiveFiles([]);
        }
      });
    } catch (err) {
      toastr.error(err);
    } finally {
      clearTimeout(timer);
    }
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
    const operationId = uniqueid("operation_");

    setSecondaryProgressBarData({
      icon: "duplicate",
      visible: true,
      percent: 0,
      label,
      alert: false,
      filesCount: filesCount + fileIds.length,
      operationId,
    });

    this.filesStore.addActiveItems(fileIds, folderIds);

    return this.uploadDataStore.copyToAction(
      this.selectedFolderStore.id,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter,
      operationId
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
    const operationId = uniqueid("operation_");

    switch (action) {
      case "pin":
        items.forEach((item) => {
          updateRoomPin(item);
          actions.push(pinRoom(item));
        });

        return Promise.all(actions)
          .then(() => {
            this.updateCurrentFolder(null, items, null, operationId);
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
            this.updateCurrentFolder(null, items, null, operationId);
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

  setMuteAction = (action, item, t) => {
    const { id, new: newCount, rootFolderId } = item;
    const { treeFolders } = this.treeFoldersStore;
    const { folders, updateRoomMute } = this.filesStore;

    const muteStatus = action === "mute" ? true : false;

    const folderIndex = id && folders.findIndex((x) => x.id === id);
    if (folderIndex) updateRoomMute(folderIndex, muteStatus);

    const treeIndex = treeFolders.findIndex((x) => x.id === rootFolderId);
    const count = treeFolders[treeIndex].newItems;
    if (treeIndex) {
      if (muteStatus) {
        treeFolders[treeIndex].newItems = newCount >= 0 ? count - newCount : 0;
      } else treeFolders[treeIndex].newItems = count + newCount;
    }

    const operationId = uniqueid("operation_");

    muteRoomNotification(id, muteStatus)
      .then(() =>
        toastr.success(
          muteStatus
            ? t("RoomNotificationsDisabled")
            : t("RoomNotificationsEnabled")
        )
      )
      .catch((e) => toastr.error(e))
      .finally(() => {
        Promise.all([
          this.updateCurrentFolder(null, [id], null, operationId),
          this.treeFoldersStore.fetchTreeFolders(),
        ]);
      });
  };

  setArchiveAction = async (action, folders, t) => {
    const { addActiveItems, setSelected } = this.filesStore;

    const { setSelectedFolder } = this.selectedFolderStore;

    const {
      roomsFolder,
      isRoomsFolder,
      archiveRoomsId,
      myRoomsId,
    } = this.treeFoldersStore;

    const {
      secondaryProgressDataStore,
      clearActiveOperations,
    } = this.uploadDataStore;

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    if (!myRoomsId || !archiveRoomsId) {
      console.error("Default categories not found");
      return;
    }

    const operationId = uniqueid("operation_");

    const items = Array.isArray(folders)
      ? folders.map((x) => (x?.id ? x.id : x))
      : [folders.id];

    setSecondaryProgressBarData({
      icon: "move",
      visible: true,
      percent: 0,
      label: "Archive room",
      alert: false,
      operationId,
    });

    const destFolder = action === "archive" ? archiveRoomsId : myRoomsId;

    addActiveItems(null, items, destFolder);

    switch (action) {
      case "archive":
        this.setGroupMenuBlocked(true);
        return moveToFolder(archiveRoomsId, items)
          .then(async (res) => {
            const lastResult = res && res[res.length - 1];

            if (lastResult?.error) return Promise.reject(lastResult.error);

            const pbData = {
              icon: "move",
              label: "Archive rooms operation",
              operationId,
            };
            const data = lastResult || null;

            console.log(pbData.label, { data, res });

            const operationData = await this.uploadDataStore.loopFilesOperations(
              data,
              pbData
            );

            if (
              !operationData ||
              operationData.error ||
              !operationData.finished
            ) {
              return Promise.reject(
                operationData?.error ? operationData.error : ""
              );
            }

            if (!isRoomsFolder) {
              setSelectedFolder(roomsFolder);
            }

            this.updateCurrentFolder(null, null, null, operationId);
          })

          .then(() => {
            const successTranslation =
              folders.length !== 1 && Array.isArray(folders)
                ? t("ArchivedRoomsAction")
                : Array.isArray(folders)
                ? t("ArchivedRoomAction", { name: folders[0].title })
                : t("ArchivedRoomAction", { name: folders.title });

            toastr.success(successTranslation);
          })
          .then(() => {
            const clearBuffer =
              !this.dialogsStore.archiveDialogVisible &&
              !this.dialogsStore.restoreRoomDialogVisible;
            setSelected("close", clearBuffer);
          })
          .catch((err) => {
            clearActiveOperations(null, items);
            setSecondaryProgressBarData({
              icon: "move",
              visible: true,
              alert: true,
              operationId,
            });
            setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
            return toastr.error(err.message ? err.message : err);
          })
          .finally(() => {
            clearActiveOperations(null, items);
            this.setGroupMenuBlocked(false);
          });
      case "unarchive":
        this.setGroupMenuBlocked(true);
        return moveToFolder(myRoomsId, items)
          .then(async (res) => {
            const lastResult = res && res[res.length - 1];

            if (lastResult?.error) return Promise.reject(lastResult.error);

            const pbData = {
              icon: "move",
              label: "Restore rooms from archive operation",
              operationId,
            };
            const data = lastResult || null;

            console.log(pbData.label, { data, res });

            await this.uploadDataStore.loopFilesOperations(data, pbData);

            this.updateCurrentFolder(null, [items], null, operationId);
          })

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
              operationId,
            });
            setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
            return toastr.error(err.message ? err.message : err);
          })
          .finally(() => {
            clearActiveOperations(null, items);
            this.setGroupMenuBlocked(false);
          });
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
      setHotkeyCaret,
      setHotkeyCaretStart,
    } = this.filesStore;
    //selected === "close" && setSelected("none");
    setBufferSelection(null);
    setHotkeyCaret(null);
    setHotkeyCaretStart(file);

    if (checked) {
      selectFile(file);
    } else {
      deselectFile(file);
    }
  };

  openLocationAction = async (locationId) => {
    const { setBufferSelection, setIsLoading, fetchFiles } = this.filesStore;

    setBufferSelection(null);
    setIsLoading(true);
    const files = await fetchFiles(locationId, null);
    setIsLoading(false);
    return files;
  };

  checkAndOpenLocationAction = async (item) => {
    const { filter, setHighlightFile, fetchFiles } = this.filesStore;
    const newFilter = filter.clone();

    newFilter.page = 0;
    newFilter.search = item.title;

    fetchFiles(item.ExtraLocation, newFilter)
      .then(() => {
        setHighlightFile({
          highlightFileId: item.id,
          isFileHasExst: !item.fileExst,
        });
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

    const operationId = uniqueid("operation_");

    setSecondaryProgressBarData({
      icon: "file",
      label: "", //TODO: add translation if need "MarkAsRead": "Mark all as read",
      percent: 0,
      visible: true,
      operationId,
    });

    return markAsRead(folderIds, fileIds)
      .then(async (res) => {
        const data = res[0] ? res[0] : null;
        const pbData = { icon: "file", operationId };
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
      .finally(() =>
        setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT)
      );
  };

  moveDragItems = (destFolderId, folderTitle, translations) => {
    const folderIds = [];
    const fileIds = [];
    const deleteAfter = false;

    const { bufferSelection } = this.filesStore;
    const { isRootFolder } = this.selectedFolderStore;

    const selection = bufferSelection
      ? [bufferSelection]
      : this.filesStore.selection;

    const isCopy = selection.findIndex((f) => f.security.Move) === -1;

    const operationData = {
      destFolderId,
      folderIds,
      fileIds,
      deleteAfter,
      translations,
      folderTitle,
      isCopy,
    };

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
    this.filesStore.addActiveItems(fileIds, null, destFolderId);
    this.filesStore.addActiveItems(null, folderIds, destFolderId);
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
        const canCopy = selection.every((s) => s.security?.Copy);

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
        const canArchive = selection.every((s) => s.security?.Move);

        return canArchive;
      case "unarchive":
        const canUnArchive = selection.some((s) => s.security?.Move);

        return canUnArchive;
      case "delete-room":
        const canRemove = selection.some((s) => s.security?.Delete);

        return canRemove;
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
    const {
      setArchiveDialogVisible,
      setInviteUsersWarningDialogVisible,
      setRestoreRoomDialogVisible,
    } = this.dialogsStore;
    const { isGracePeriod } = this.authStore.currentTariffStatusStore;

    if (action === "unarchive" && isGracePeriod) {
      setInviteUsersWarningDialogVisible(true);
      return;
    }

    if (action === "archive") {
      setArchiveDialogVisible(true);
    } else {
      setRestoreRoomDialogVisible(true);
    }
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

    const operationId = uniqueid("operation_");

    setSecondaryProgressBarData({
      icon: "trash",
      visible: true,
      percent: 0,
      label: translations?.deleteOperation,
      alert: false,
      operationId,
    });

    try {
      this.setGroupMenuBlocked(true);
      await this.deleteItemOperation(
        false,
        itemId,
        translations,
        true,
        operationId
      );

      const id = Array.isArray(itemId) ? itemId : [itemId];

      clearActiveOperations(null, id);
    } catch (err) {
      console.log(err);
      clearActiveOperations(null, [itemId]);
      setSecondaryProgressBarData({
        visible: true,
        alert: true,
        operationId,
      });
      setTimeout(() => clearSecondaryProgressData(operationId), TIMEOUT);
      return toastr.error(err.message ? err.message : err);
    } finally {
      this.setGroupMenuBlocked(false);
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
            iconUrl: InfoOutlineReactSvgUrl,
            onClick: this.onShowInfoPanel,
          };
      case "copy":
        if (!this.isAvailableOption("copy")) return null;
        else
          return {
            id: "menu-copy",
            label: t("Common:Copy"),
            onClick: () => setCopyPanelVisible(true),
            iconUrl: CopyToReactSvgUrl,
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
            iconUrl: DownloadReactSvgUrl,
          };

      case "downloadAs":
        if (!this.isAvailableOption("downloadAs")) return null;
        else
          return {
            id: "menu-download-as",
            label: t("Translations:DownloadAs"),
            onClick: () => setDownloadDialogVisible(true),
            iconUrl: DownloadAsReactSvgUrl,
          };

      case "moveTo":
        if (!this.isAvailableOption("moveTo")) return null;
        else
          return {
            id: "menu-move-to",
            label: t("Common:MoveTo"),
            onClick: () => setMoveToPanelVisible(true),
            iconUrl: MoveReactSvgUrl,
          };
      case "pin":
        return {
          id: "menu-pin",
          key: "pin",
          label: t("Pin"),
          iconUrl: PinReactSvgUrl,
          onClick: () => this.pinRooms(t),
          disabled: false,
        };
      case "unpin":
        return {
          id: "menu-unpin",
          key: "unpin",
          label: t("Unpin"),
          iconUrl: UnpinReactSvgUrl,
          onClick: () => this.unpinRooms(t),
          disabled: false,
        };
      case "archive":
        if (!this.isAvailableOption("archive")) return null;
        else
          return {
            id: "menu-archive",
            key: "archive",
            label: t("MoveToArchive"),
            iconUrl: RoomArchiveSvgUrl,
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
            iconUrl: MoveReactSvgUrl,
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
            iconUrl: DeleteReactSvgUrl,
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
            iconUrl: DeleteReactSvgUrl,
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
        iconUrl: FavoritesReactSvgUrl,
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
        iconUrl: MoveReactSvgUrl,
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
    const canConvert =
      item.viewAccessability?.Convert && item.security?.Convert;
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
        setConvertItem({ ...item, isOpen: true });
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
                  window.DocSpaceConfig?.proxy?.url,
                  config.homepage,
                  `/doceditor?fileId=${id}`
                ),
                "_blank"
              )
            : null;

        return openDocEditor(id, providerKey, tab, null, !canWebEdit);
      }

      if (isMediaOrImage) {
        // localStorage.setItem("isFirstUrl", window.location.href);
        this.mediaViewerDataStore.saveFirstUrl(window.location.href);
        setMediaViewerData({ visible: true, id });

        const url = "/products/files/#preview/" + id;
        history.pushState(null, null, url);
        return;
      }

      return window.open(viewUrl, "_self");
    }
  };

  onClickBack = () => {
    const { roomType, parentId, setSelectedFolder } = this.selectedFolderStore;
    const { setSelectedNode } = this.treeFoldersStore;

    const categoryType = getCategoryType(location);
    const isRoom = !!roomType;

    const urlFilter = getObjectByLocation(location);

    const isArchivedRoom = !!(CategoryType.Archive && urlFilter?.folder);

    if (categoryType === CategoryType.SharedRoom || isArchivedRoom) {
      if (isRoom) {
        return this.moveToRoomsPage();
      }

      return this.backToParentFolder();
    }

    if (
      categoryType === CategoryType.Shared ||
      categoryType === CategoryType.Archive
    ) {
      return this.moveToRoomsPage();
    }

    if (
      categoryType === CategoryType.Personal ||
      categoryType === CategoryType.Trash
    ) {
      return this.backToParentFolder();
    }

    if (categoryType === CategoryType.Settings) {
      setSelectedFolder(null);
      setSelectedNode(["common"]);
    }

    if (categoryType === CategoryType.Accounts) {
      setSelectedFolder(null);
      setSelectedNode(["accounts", "filter"]);
    }
  };

  moveToRoomsPage = () => {
    const {
      setIsLoading,
      fetchRooms,
      setAlreadyFetchingRooms,
    } = this.filesStore;

    const categoryType = getCategoryType(location);

    setIsLoading(true);
    setAlreadyFetchingRooms(true);

    const filter = RoomsFilter.getDefault();

    if (categoryType == CategoryType.Archive) {
      filter.searchArea = RoomSearchArea.Archive;
    }

    fetchRooms(null, filter).finally(() => {
      setIsLoading(false);
    });
  };

  backToParentFolder = () => {
    const {
      archiveFolderId,
      myFolderId,
      myRoomsId,
      isTrashFolder,
    } = this.treeFoldersStore;
    const currentFolderId = this.selectedFolderStore?.id;

    if (
      currentFolderId === archiveFolderId ||
      currentFolderId === myFolderId ||
      currentFolderId === myRoomsId ||
      isTrashFolder
    )
      return;

    const { setIsLoading, fetchFiles } = this.filesStore;

    let id = this.selectedFolderStore.parentId;

    if (!id) {
      const urlFilter = getObjectByLocation(location);
      id = urlFilter.folder;
    }

    setIsLoading(true);

    fetchFiles(id, null, true, false).finally(() => setIsLoading(false));
  };

  setGroupMenuBlocked = (blocked) => {
    this.isGroupMenuBlocked = blocked;
  };
}

export default FilesActionStore;
