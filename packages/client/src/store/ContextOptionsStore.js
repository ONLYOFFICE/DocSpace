import { makeAutoObservable } from "mobx";
import copy from "copy-to-clipboard";
import saveAs from "file-saver";
import { isMobile } from "react-device-detect";
import config from "PACKAGE_FILE";
import toastr from "@docspace/components/toast/toastr";
import { AppServerConfig, ShareAccessRights } from "@docspace/common/constants";
import combineUrl from "@docspace/common/utils/combineUrl";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";
import { Events } from "@docspace/common/constants";
import { getContextMenuItems } from "SRC_DIR/helpers/plugins";

class ContextOptionsStore {
  authStore;
  dialogsStore;
  filesActionsStore;
  filesStore;
  mediaViewerDataStore;
  treeFoldersStore;
  uploadDataStore;
  versionHistoryStore;
  settingsStore;
  filesSettingsStore;

  constructor(
    authStore,
    dialogsStore,
    filesActionsStore,
    filesStore,
    mediaViewerDataStore,
    treeFoldersStore,
    uploadDataStore,
    versionHistoryStore,
    settingsStore
  ) {
    makeAutoObservable(this);
    this.authStore = authStore;
    this.dialogsStore = dialogsStore;
    this.filesActionsStore = filesActionsStore;
    this.filesStore = filesStore;
    this.mediaViewerDataStore = mediaViewerDataStore;
    this.treeFoldersStore = treeFoldersStore;
    this.uploadDataStore = uploadDataStore;
    this.versionHistoryStore = versionHistoryStore;
    this.settingsStore = settingsStore;
  }

  onOpenFolder = (item) => {
    const { id, folderId, fileExst } = item;
    const locationId = !fileExst ? id : folderId;
    this.filesActionsStore.openLocationAction(locationId);
  };

  onClickLinkFillForm = (item) => {
    return this.gotoDocEditor(false, item);
  };

  onClickMakeForm = (item, t) => {
    const {
      setConvertPasswordDialogVisible,
      setFormCreationInfo,
    } = this.dialogsStore;
    const { title, id, folderId, fileExst } = item;

    const newTitle =
      title.substring(0, title.length - fileExst.length) +
      this.settingsStore.extsWebRestrictedEditing[0];

    this.uploadDataStore.copyAsAction(id, newTitle, folderId).catch((err) => {
      if (err.indexOf("password") == -1) {
        toastr.error(err, t("Common:Warning"));
        return;
      }

      toastr.error(t("Translations:FileProtected"), t("Common:Warning"));
      setFormCreationInfo({
        newTitle,
        fromExst: fileExst,
        toExst: this.settingsStore.extsWebRestrictedEditing[0],
        fileInfo: item,
      });
      setConvertPasswordDialogVisible(true);
    });
  };

  onOpenLocation = (item) => {
    const { parentId, folderId, fileExst } = item;
    const locationId = !fileExst ? parentId : folderId;
    this.filesActionsStore.openLocationAction(locationId);
  };

  onOwnerChange = () => {
    this.dialogsStore.setChangeOwnerPanelVisible(true);
  };

  onMoveAction = () => {
    this.dialogsStore.setMoveToPanelVisible(true);
  };

  onCopyAction = () => {
    this.dialogsStore.setCopyPanelVisible(true);
  };

  showVersionHistory = (id) => {
    const {
      fetchFileVersions,
      setIsVerHistoryPanel,
    } = this.versionHistoryStore;

    if (this.treeFoldersStore.isRecycleBinFolder) return;

    fetchFileVersions(id + "");
    setIsVerHistoryPanel(true);
  };

  finalizeVersion = (id) => {
    this.filesActionsStore
      .finalizeVersionAction(id)
      .catch((err) => toastr.error(err));
  };

  onClickFavorite = (e, id, t) => {
    const data = (e.currentTarget && e.currentTarget.dataset) || e;
    const { action } = data;

    this.filesActionsStore
      .setFavoriteAction(action, id)
      .then(() =>
        action === "mark"
          ? toastr.success(t("MarkedAsFavorite"))
          : toastr.success(t("RemovedFromFavorites"))
      )
      .catch((err) => toastr.error(err));
  };

  lockFile = (item, t) => {
    const { id, locked } = item;
    const {
      setSelection: setInfoPanelSelection,
    } = this.authStore.infoPanelStore;

    this.filesActionsStore
      .lockFileAction(id, !locked)
      .then(() =>
        locked
          ? toastr.success(t("Translations:FileUnlocked"))
          : toastr.success(t("Translations:FileLocked"))
      )
      .then(() => setInfoPanelSelection({ ...item, locked: !locked }))
      .catch((err) => toastr.error(err));
  };

  onClickLinkForPortal = (item, t) => {
    const { fileExst, canOpenPlayer, webUrl, id } = item;

    const isFile = !!fileExst;
    copy(
      isFile
        ? canOpenPlayer
          ? `${window.location.href}&preview=${id}`
          : webUrl
        : `${window.location.origin + config.homepage}/filter?folder=${id}` //TODO: Change url by category
    );

    toastr.success(t("Translations:LinkCopySuccess"));
  };

  onClickLinkEdit = (item) => {
    const { setConvertItem, setConvertDialogVisible } = this.dialogsStore;
    const canConvert = this.settingsStore.canConvert(item.fileExst);

    if (canConvert) {
      setConvertItem(item);
      setConvertDialogVisible(true);
    } else {
      this.gotoDocEditor(false, item);
    }
  };

  onPreviewClick = (item) => {
    this.gotoDocEditor(true, item);
  };

  gotoDocEditor = (preview = false, item) => {
    const { id, providerKey, fileExst } = item;

    const urlFormation = preview
      ? combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/doceditor?fileId=${encodeURIComponent(id)}&action=view`
        )
      : null;

    let tab =
      !this.authStore.isDesktopClient && fileExst
        ? window.open(
            combineUrl(AppServerConfig.proxyURL, config.homepage, "/doceditor"),
            "_blank"
          )
        : null;

    this.filesStore.openDocEditor(id, providerKey, tab, urlFormation);
  };

  isPwa = () => {
    return ["fullscreen", "standalone", "minimal-ui"].some(
      (displayMode) =>
        window.matchMedia("(display-mode: " + displayMode + ")").matches
    );
  };

  onClickDownload = (item, t) => {
    const { fileExst, contentLength, viewUrl } = item;
    const isFile = !!fileExst && contentLength;

    if (this.isPwa()) {
      const xhr = new XMLHttpRequest();
      xhr.open("GET", viewUrl);
      xhr.responseType = "blob";

      xhr.onload = () => {
        saveAs(xhr.response, item.title);
      };

      xhr.onerror = () => {
        console.error("download failed", viewUrl);
      };

      xhr.send();
      return;
    }

    isFile
      ? window.open(viewUrl, "_self")
      : this.filesActionsStore
          .downloadAction(t("Translations:ArchivingData"))
          .catch((err) => toastr.error(err));
  };

  onClickDownloadAs = () => {
    this.dialogsStore.setDownloadDialogVisible(true);
  };

  onDuplicate = (item, t) => {
    this.filesActionsStore
      .duplicateAction(item, t("Common:CopyOperation"))
      .catch((err) => toastr.error(err));
  };

  onClickRename = (item) => {
    const event = new Event(Events.RENAME);

    event.item = item;

    window.dispatchEvent(event);
  };

  onChangeThirdPartyInfo = (providerKey) => {
    this.filesActionsStore.setThirdpartyInfo(providerKey);
  };

  onMediaFileClick = (fileId, item) => {
    const itemId = typeof fileId !== "object" ? fileId : item.id;
    this.mediaViewerDataStore.setMediaViewerData({ visible: true, id: itemId });
  };

  onClickDelete = (item, t) => {
    const {
      setRemoveItem,
      setDeleteThirdPartyDialogVisible,
    } = this.dialogsStore;

    const { id, title, providerKey, rootFolderId, isFolder, isRoom } = item;

    const isRootThirdPartyFolder = providerKey && id === rootFolderId;

    if (isRootThirdPartyFolder) {
      const splitItem = id.split("-");
      setRemoveItem({ id: splitItem[splitItem.length - 1], title });
      setDeleteThirdPartyDialogVisible(true);
      return;
    }

    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      successRemoveFile: t("Files:FileRemoved"),
      successRemoveFolder: t("Files:FolderRemoved"),
      successRemoveRoom: t("Files:RoomRemoved"),
      successRemoveRooms: t("Files:RoomsRemoved"),
    };

    this.filesActionsStore.deleteItemAction(
      id,
      translations,
      !isFolder,
      providerKey,
      isRoom
    );
  };

  onClickShare = () => {
    setTimeout(() => {
      this.dialogsStore.setSharingPanelVisible(true);
    }, 10); //TODO: remove delay after fix context menu callback
  };

  onClickMarkRead = (item) => {
    const { markAsRead } = this.filesActionsStore;

    item.fileExst
      ? markAsRead([], [item.id], item)
      : markAsRead([item.id], [], item);
  };

  onClickUnsubscribe = () => {
    const { setDeleteDialogVisible, setUnsubscribe } = this.dialogsStore;

    setUnsubscribe(true);
    setDeleteDialogVisible(true);
  };

  filterModel = (model, filter) => {
    let options = [];
    let index = 0;
    const last = model.length;

    for (index; index < last; index++) {
      if (filter.includes(model[index].key)) {
        options[index] = model[index];
        if (model[index].items) {
          options[index].items = model[index].items.filter((item) =>
            filter.includes(item.key)
          );
        }
      }
    }

    return options;
  };

  onShowInfoPanel = (item) => {
    const { setSelection, setIsVisible } = this.authStore.infoPanelStore;
    setSelection({ ...item, isContextMenuSelection: true });
    setIsVisible(true);
  };

  onClickEditRoom = (item) => {
    const event = new Event(Events.ROOM_EDIT);
    event.item = item;
    window.dispatchEvent(event);
  };

  onClickInviteUsers = (e) => {
    const data = (e.currentTarget && e.currentTarget.dataset) || e;

    const { action } = data;

    this.dialogsStore.setInvitePanelOptions({
      visible: true,
      roomId: action ? action : e,
      hideSelector: false,
      defaultAccess: ShareAccessRights.ReadOnly,
    });
  };

  onClickPin = (e, id, t) => {
    const data = (e.currentTarget && e.currentTarget.dataset) || e;
    const { action } = data;

    this.filesActionsStore.setPinAction(action, id);
  };

  onClickArchive = (e, item, t) => {
    const data = (e.currentTarget && e.currentTarget.dataset) || e;
    const { action } = data;

    this.filesActionsStore
      .setArchiveAction(action, item, t)
      .catch((err) => toastr.error(err));
  };

  onSelect = (item) => {
    const { onSelectItem } = this.filesActionsStore;

    onSelectItem({ id: item.id, isFolder: item.isFolder }, true, false);
  };

  getFilesContextOptions = (item, t) => {
    const { contextOptions } = item;

    const { enablePlugins } = this.authStore.settingsStore;

    const isRootThirdPartyFolder =
      item.providerKey && item.id === item.rootFolderId;

    const isShareable = item.canShare;

    const isMedia = this.settingsStore.isMediaOrImage(item.fileExst);
    const isCanWebEdit = this.settingsStore.canWebEdit(item.fileExst);

    const blockAction = isCanWebEdit
      ? {
          key: "block-unblock-version",
          label: t("UnblockVersion"),
          icon: "/static/images/locked.react.svg",
          onClick: () => this.lockFile(item, t),
          disabled: false,
        }
      : false;

    const versionActions = !isMedia
      ? !isMobile && !isMobileUtils() && !isTabletUtils()
        ? [
            {
              key: "version",
              label: t("VersionHistory"),
              icon: "images/history-finalized.react.svg",
              items: [
                {
                  key: "finalize-version",
                  label: t("FinalizeVersion"),
                  icon: "images/history-finalized.react.svg",
                  onClick: () => this.finalizeVersion(item.id),
                  disabled: false,
                },
                {
                  key: "show-version-history",
                  label: t("ShowVersionHistory"),
                  icon: "images/history.react.svg",
                  onClick: () => this.showVersionHistory(item.id),
                  disabled: false,
                },
              ],
            },
          ]
        : [
            {
              key: "finalize-version",
              label: t("FinalizeVersion"),
              icon: "images/history-finalized.react.svg",
              onClick: () => this.finalizeVersion(item.id),
              disabled: false,
            },
            {
              key: "show-version-history",
              label: t("ShowVersionHistory"),
              icon: "images/history.react.svg",
              onClick: () => this.showVersionHistory(item.id),
              disabled: false,
            },
          ]
      : [];

    const moveActions =
      !isMobile && !isMobileUtils() && !isTabletUtils()
        ? [
            {
              key: "move",
              label: t("MoveOrCopy"),
              icon: "/static/images/copy.react.svg",
              items: [
                {
                  key: "move-to",
                  label: t("MoveTo"),
                  icon: "images/move.react.svg",
                  onClick: this.onMoveAction,
                  disabled: false,
                },
                {
                  key: "copy-to",
                  label: t("Translations:Copy"),
                  icon: "/static/images/copy.react.svg",
                  onClick: this.onCopyAction,
                  disabled: false,
                },
                {
                  key: "copy",
                  label: t("Common:Duplicate"),
                  icon: "/static/images/duplicate.react.svg",
                  onClick: () => this.onDuplicate(item, t),
                  disabled: false,
                },
              ],
            },
          ]
        : [
            {
              key: "move-to",
              label: t("MoveTo"),
              icon: "images/move.react.svg",
              onClick: this.onMoveAction,
              disabled: false,
            },
            {
              key: "copy-to",
              label: t("Translations:Copy"),
              icon: "/static/images/copy.react.svg",
              onClick: this.onCopyAction,
              disabled: false,
            },
            {
              key: "copy",
              label: t("Common:Duplicate"),
              icon: "/static/images/duplicate.react.svg",
              onClick: () => this.onDuplicate(item, t),
              disabled: false,
            },
          ];

    const optionsModel = [
      {
        key: "select",
        label: "Select",
        icon: "images/check-box.react.svg",
        onClick: () => this.onSelect(item),
        disabled: false,
      },
      {
        key: "open",
        label: t("Open"),
        icon: "images/folder.react.svg",
        onClick: () => this.onOpenFolder(item),
        disabled: false,
      },
      {
        key: "fill-form",
        label: t("Common:FillFormButton"),
        icon: "/static/images/form.fill.rect.svg",
        onClick: () => this.onClickLinkFillForm(item),
        disabled: false,
      },
      {
        key: "edit",
        label: t("Common:EditButton"),
        icon: "/static/images/access.edit.react.svg",
        onClick: () => this.onClickLinkEdit(item),
        disabled: false,
      },
      {
        key: "preview",
        label: t("Preview"),
        icon: "/static/images/eye.react.svg",
        onClick: () => this.onPreviewClick(item),
        disabled: false,
      },
      {
        key: "view",
        label: t("Common:View"),
        icon: "/static/images/eye.react.svg",
        onClick: (fileId) => this.onMediaFileClick(fileId, item),
        disabled: false,
      },
      {
        key: "make-form",
        label: t("Common:MakeForm"),
        icon: "/static/images/form.plus.react.svg",
        onClick: () => this.onClickMakeForm(item, t),
        disabled: false,
      },
      {
        key: "separator0",
        isSeparator: true,
      },
      {
        key: "edit-room",
        label: t("EditRoom"),
        icon: "images/settings.react.svg",
        onClick: () => this.onClickEditRoom(item),
        disabled: false,
      },
      {
        key: "invite-users-to-room",
        label: t("InviteUsers"),
        icon: "/static/images/person.react.svg",
        onClick: (e) => this.onClickInviteUsers(e),
        disabled: false,
        action: item.id,
      },
      {
        key: "room-info",
        label: t("Common:Info"),
        icon: "/static/images/info.outline.react.svg",
        onClick: () => this.onShowInfoPanel(item),
        disabled: false,
      },
      {
        key: "pin-room",
        label: t("Pin"),
        icon: "/static/images/pin.react.svg",
        onClick: (e) => this.onClickPin(e, item.id, t),
        disabled: false,
        "data-action": "pin",
        action: "pin",
      },
      {
        key: "unpin-room",
        label: t("Unpin"),
        icon: "/static/images/unpin.react.svg",
        onClick: (e) => this.onClickPin(e, item.id, t),
        disabled: false,
        "data-action": "unpin",
        action: "unpin",
      },
      {
        key: "sharing-settings",
        label: t("SharingPanel:SharingSettingsTitle"),
        icon: "/static/images/share.react.svg",
        onClick: this.onClickShare,
        disabled: !isShareable,
      },
      {
        key: "owner-change",
        label: t("Translations:OwnerChange"),
        icon: "images/file.actions.owner.react.svg",
        onClick: this.onOwnerChange,
        disabled: false,
      },
      {
        key: "link-for-portal-users",
        label: t("LinkForPortalUsers"),
        icon: "/static/images/invitation.link.react.svg",
        onClick: () => this.onClickLinkForPortal(item, t),
        disabled: false,
      },
      {
        key: "send-by-email",
        label: t("SendByEmail"),
        icon: "/static/images/mail.react.svg",
        disabled: true,
      },
      ...versionActions,
      {
        key: "show-info",
        label: t("InfoPanel:ViewDetails"),
        icon: "/static/images/info.outline.react.svg",
        onClick: () => this.onShowInfoPanel(item),
        disabled: false,
      },
      blockAction,
      {
        key: "separator1",
        isSeparator: true,
      },
      {
        key: "open-location",
        label: t("OpenLocation"),
        icon: "images/folder.location.react.svg",
        onClick: () => this.onOpenLocation(item),
        disabled: false,
      },
      {
        key: "mark-read",
        label: t("MarkRead"),
        icon: "images/tick.rounded.svg",
        onClick: () => this.onClickMarkRead(item),
        disabled: false,
      },
      {
        key: "mark-as-favorite",
        label: t("MarkAsFavorite"),
        icon: "images/favorites.react.svg",
        onClick: (e) => this.onClickFavorite(e, item.id, t),
        disabled: false,
        "data-action": "mark",
        action: "mark",
      },
      {
        key: "remove-from-favorites",
        label: t("RemoveFromFavorites"),
        icon: "images/favorites.react.svg",
        onClick: (e) => this.onClickFavorite(e, item.id, t),
        disabled: false,
        "data-action": "remove",
        action: "remove",
      },
      {
        key: "download",
        label: t("Common:Download"),
        icon: "images/download.react.svg",
        onClick: () => this.onClickDownload(item, t),
        disabled: false,
      },
      {
        key: "download-as",
        label: t("Translations:DownloadAs"),
        icon: "images/download-as.react.svg",
        onClick: this.onClickDownloadAs,
        disabled: false,
      },
      ...moveActions,
      {
        key: "restore",
        label: t("Common:Restore"),
        icon: "images/move.react.svg",
        onClick: this.onMoveAction,
        disabled: false,
      },
      {
        key: "rename",
        label: t("Rename"),
        icon: "images/rename.react.svg",
        onClick: () => this.onClickRename(item),
        disabled: false,
      },
      {
        key: "separator2",
        isSeparator: true,
      },
      {
        key: "unsubscribe",
        label: t("RemoveFromList"),
        icon: "images/remove.svg",
        onClick: this.onClickUnsubscribe,
        disabled: false,
      },
      {
        key: "change-thirdparty-info",
        label: t("Translations:ThirdPartyInfo"),
        icon: "/static/images/access.edit.react.svg",
        onClick: () => this.onChangeThirdPartyInfo(item.providerKey),
        disabled: false,
      },
      {
        key: "archive-room",
        label: t("Archived"),
        icon: "/static/images/room.archive.svg",
        onClick: (e) => this.onClickArchive(e, item, t),
        disabled: false,
        "data-action": "archive",
        action: "archive",
      },
      {
        key: "unarchive-room",
        label: t("Common:Restore"),
        icon: "images/subtract.react.svg",
        onClick: (e) => this.onClickArchive(e, item, t),
        disabled: false,
        "data-action": "unarchive",
        action: "unarchive",
      },
      {
        key: "delete",
        label: isRootThirdPartyFolder
          ? t("Common:Disconnect")
          : t("Common:Delete"),
        icon: "images/trash.react.svg",
        onClick: () => this.onClickDelete(item, t),
        disabled: false,
      },
    ];

    const options = this.filterModel(optionsModel, contextOptions);

    if (enablePlugins) {
      const pluginOptions = getContextMenuItems();

      if (pluginOptions) {
        pluginOptions.forEach((option) => {
          if (contextOptions.includes(option.key)) {
            const value = option.value;
            if (!value.onClick) {
              options.splice(value.position, 0, {
                key: option.key,
                ...value,
              });
            } else {
              options.splice(value.position, 0, {
                key: option.key,
                label: value.label,
                icon: value.icon,
                onClick: () => value.onClick(item),
              });
            }
          }
        });
      }
    }
    return options;
  };

  getGroupContextOptions = (t) => {
    const { personal } = this.authStore.settingsStore;
    const { selection } = this.filesStore;
    const { setDeleteDialogVisible } = this.dialogsStore;
    const {
      isRecycleBinFolder,
      isRoomsFolder,
      isArchiveFolder,
    } = this.treeFoldersStore;

    const {
      pinRooms,
      unpinRooms,
      moveRoomsToArchive,
      moveRoomsFromArchive,
      deleteRooms,
    } = this.filesActionsStore;

    if (isRoomsFolder || isArchiveFolder) {
      const isPinOption = selection.filter((item) => !item.pinned).length > 0;

      const pinOption = isPinOption
        ? {
            key: "pin-room",
            label: t("Pin"),
            icon: "/static/images/pin.react.svg",
            onClick: pinRooms,
            disabled: false,
          }
        : {
            key: "unpin-room",
            label: t("Unpin"),
            icon: "/static/images/unpin.react.svg",
            onClick: unpinRooms,
            disabled: false,
          };

      const archiveOptions = !isArchiveFolder
        ? {
            key: "archive-room",
            label: t("Archived"),
            icon: "/static/images/room.archive.svg",
            onClick: () => moveRoomsToArchive(t),
            disabled: false,
          }
        : {
            key: "unarchive-room",
            label: t("Common:Restore"),
            icon: "images/subtract.react.svg",
            onClick: () => moveRoomsFromArchive(t),
            disabled: false,
          };

      const options = [];

      if (!isArchiveFolder) {
        options.push(pinOption);
        options.push({
          key: "separator0",
          isSeparator: true,
        });
      }

      options.push(archiveOptions);

      if (isArchiveFolder) {
        options.push({
          key: "delete-rooms",
          label: t("Common:Delete"),
          icon: "images/trash.react.svg",
          onClick: () => deleteRooms(t),
        });
      }

      return options;
    }

    const downloadAs =
      selection.findIndex((k) => k.contextOptions.includes("download-as")) !==
      -1;

    const sharingItems =
      selection.filter(
        (k) => k.contextOptions.includes("sharing-settings") && k.canShare
      ).length && !personal;

    const favoriteItems = selection.filter((k) =>
      k.contextOptions.includes("mark-as-favorite")
    );

    const moveItems = selection.filter((k) =>
      k.contextOptions.includes("move-to")
    ).length;

    const copyItems = selection.filter((k) =>
      k.contextOptions.includes("copy-to")
    ).length;

    const restoreItems = selection.filter((k) =>
      k.contextOptions.includes("restore")
    ).length;

    const removeFromFavoriteItems = selection.filter((k) =>
      k.contextOptions.includes("remove-from-favorites")
    );

    const deleteItems = selection.filter((k) =>
      k.contextOptions.includes("delete")
    ).length;

    const isRootThirdPartyFolder = selection.some(
      (x) => x.providerKey && x.id === x.rootFolderId
    );

    const favoriteItemsIds = favoriteItems.map((item) => item.id);

    const removeFromFavoriteItemsIds = removeFromFavoriteItems.map(
      (item) => item.id
    );

    const options = [
      {
        key: "sharing-settings",
        label: t("SharingPanel:SharingSettingsTitle"),
        icon: "/static/images/share.react.svg",
        onClick: this.onClickShare,
        disabled: !sharingItems,
      },
      {
        key: "separator0",
        isSeparator: true,
        disabled: !sharingItems,
      },
      {
        key: "mark-as-favorite",
        label: t("MarkAsFavorite"),
        icon: "images/favorites.react.svg",
        onClick: (e) => this.onClickFavorite(e, favoriteItemsIds, t),
        disabled: !favoriteItems.length,
        "data-action": "mark",
        action: "mark",
      },
      {
        key: "remove-from-favorites",
        label: t("RemoveFromFavorites"),
        icon: "images/favorites.react.svg",
        onClick: (e) => this.onClickFavorite(e, removeFromFavoriteItemsIds, t),
        disabled: favoriteItems.length || !removeFromFavoriteItems.length,
        "data-action": "remove",
        action: "remove",
      },
      {
        key: "download",
        label: t("Common:Download"),
        icon: "images/download.react.svg",
        onClick: () =>
          this.filesActionsStore
            .downloadAction(t("Translations:ArchivingData"))
            .catch((err) => toastr.error(err)),
        disabled: false,
      },
      {
        key: "download-as",
        label: t("Translations:DownloadAs"),
        icon: "images/download-as.react.svg",
        onClick: this.onClickDownloadAs,
        disabled: !downloadAs,
      },
      {
        key: "move-to",
        label: t("MoveTo"),
        icon: "images/move.react.svg",
        onClick: this.onMoveAction,
        disabled: isRecycleBinFolder || !moveItems,
      },
      {
        key: "copy-to",
        label: t("Translations:Copy"),
        icon: "/static/images/copy.react.svg",
        onClick: this.onCopyAction,
        disabled: isRecycleBinFolder || !copyItems,
      },
      {
        key: "restore",
        label: t("Common:Restore"),
        icon: "images/move.react.svg",
        onClick: this.onMoveAction,
        disabled: !isRecycleBinFolder || !restoreItems,
      },
      {
        key: "separator1",
        isSeparator: true,
        disabled: !deleteItems || isRootThirdPartyFolder,
      },
      {
        key: "delete",
        label: t("Common:Delete"),
        icon: "images/trash.react.svg",
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

            this.filesActionsStore
              .deleteAction(translations)
              .catch((err) => toastr.error(err));
          }
        },
        disabled: !deleteItems || isRootThirdPartyFolder,
      },
    ];

    return options;
  };

  getModel = (item, t) => {
    const { selection } = this.filesStore;

    const { fileExst, contextOptions } = item;

    const contextOptionsProps =
      contextOptions && contextOptions.length > 0
        ? selection.length > 1
          ? this.getGroupContextOptions(t)
          : this.getFilesContextOptions(item, t)
        : [];

    return contextOptionsProps;
  };
}

export default ContextOptionsStore;
