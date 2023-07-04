import HistoryReactSvgUrl from "PUBLIC_DIR/images/history.react.svg?url";
import HistoryFinalizedReactSvgUrl from "PUBLIC_DIR/images/history-finalized.react.svg?url";
import MoveReactSvgUrl from "PUBLIC_DIR/images/move.react.svg?url";
import CheckBoxReactSvgUrl from "PUBLIC_DIR/images/check-box.react.svg?url";
import FolderReactSvgUrl from "PUBLIC_DIR/images/folder.react.svg?url";
import ReconnectSvgUrl from "PUBLIC_DIR/images/reconnect.svg?url";
import SettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import FileActionsOwnerReactSvgUrl from "PUBLIC_DIR/images/file.actions.owner.react.svg?url";
import FolderLocationReactSvgUrl from "PUBLIC_DIR/images/folder.location.react.svg?url";
import TickRoundedSvgUrl from "PUBLIC_DIR/images/tick.rounded.svg?url";
import FavoritesReactSvgUrl from "PUBLIC_DIR/images/favorites.react.svg?url";
import DownloadReactSvgUrl from "PUBLIC_DIR/images/download.react.svg?url";
import DownloadAsReactSvgUrl from "PUBLIC_DIR/images/download-as.react.svg?url";
import RenameReactSvgUrl from "PUBLIC_DIR/images/rename.react.svg?url";
import RemoveSvgUrl from "PUBLIC_DIR/images/remove.svg?url";
import TrashReactSvgUrl from "PUBLIC_DIR/images/trash.react.svg?url";
import LockedReactSvgUrl from "PUBLIC_DIR/images/locked.react.svg?url";
import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import DuplicateReactSvgUrl from "PUBLIC_DIR/images/duplicate.react.svg?url";
import FormFillRectSvgUrl from "PUBLIC_DIR/images/form.fill.rect.svg?url";
import AccessEditReactSvgUrl from "PUBLIC_DIR/images/access.edit.react.svg?url";
import EyeReactSvgUrl from "PUBLIC_DIR/images/eye.react.svg?url";
import FormPlusReactSvgUrl from "PUBLIC_DIR/images/form.plus.react.svg?url";
import PersonReactSvgUrl from "PUBLIC_DIR/images/person.react.svg?url";
import InfoOutlineReactSvgUrl from "PUBLIC_DIR/images/info.outline.react.svg?url";
import PinReactSvgUrl from "PUBLIC_DIR/images/pin.react.svg?url";
import UnpinReactSvgUrl from "PUBLIC_DIR/images/unpin.react.svg?url";
import UnmuteReactSvgUrl from "PUBLIC_DIR/images/unmute.react.svg?url";
import MuteReactSvgUrl from "PUBLIC_DIR/images/mute.react.svg?url";
import ShareReactSvgUrl from "PUBLIC_DIR/images/share.react.svg?url";
import InvitationLinkReactSvgUrl from "PUBLIC_DIR/images/invitation.link.react.svg?url";
import MailReactSvgUrl from "PUBLIC_DIR/images/mail.react.svg?url";
import RoomArchiveSvgUrl from "PUBLIC_DIR/images/room.archive.svg?url";
import { makeAutoObservable } from "mobx";
import copy from "copy-to-clipboard";
import saveAs from "file-saver";
import { isMobile } from "react-device-detect";
import config from "PACKAGE_FILE";
import toastr from "@docspace/components/toast/toastr";
import { ShareAccessRights } from "@docspace/common/constants";
import combineUrl from "@docspace/common/utils/combineUrl";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";
import { Events } from "@docspace/common/constants";
import { getContextMenuItems } from "SRC_DIR/helpers/plugins";
import { connectedCloudsTypeTitleTranslation } from "@docspace/client/src/helpers/filesUtils";
import { getOAuthToken } from "@docspace/common/utils";

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
  selectedFolderStore;

  constructor(
    authStore,
    dialogsStore,
    filesActionsStore,
    filesStore,
    mediaViewerDataStore,
    treeFoldersStore,
    uploadDataStore,
    versionHistoryStore,
    settingsStore,
    selectedFolderStore
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
    this.selectedFolderStore = selectedFolderStore;
  }

  onOpenFolder = (item) => {
    const { id, folderId, fileExst } = item;
    const locationId = !fileExst ? id : folderId;
    this.filesActionsStore.openLocationAction(locationId);
  };

  onClickLinkFillForm = (item) => {
    return this.gotoDocEditor(false, item);
  };

  onClickReconnectStorage = async (item, t) => {
    const { thirdPartyStore } = this.settingsStore;

    const { openConnectWindow, connectItems } = thirdPartyStore;

    const {
      setRoomCreation,
      setConnectItem,
      setConnectDialogVisible,
      setIsConnectDialogReconnect,
      setSaveAfterReconnectOAuth,
    } = this.dialogsStore;

    setIsConnectDialogReconnect(true);

    setRoomCreation(true);

    const provider = connectItems.find(
      (connectItem) => connectItem.providerName === item.providerKey
    );

    const itemThirdParty = {
      title: connectedCloudsTypeTitleTranslation(provider.providerName, t),
      customer_title: "NOTITLE",
      provider_key: provider.providerName,
      link: provider.oauthHref,
    };

    if (provider.isOauth) {
      let authModal = window.open(
        "",
        t("Common:Authorization"),
        "height=600, width=1020"
      );
      await openConnectWindow(provider.providerName, authModal)
        .then(getOAuthToken)
        .then((token) => {
          authModal.close();
          setConnectItem({
            ...itemThirdParty,
            token,
          });

          setSaveAfterReconnectOAuth(true);
        })
        .catch((err) => {
          if (!err) return;
          toastr.error(err);
        });
    } else {
      setConnectItem(itemThirdParty);
      setConnectDialogVisible(true);
    }
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
      let errorMessage = "";
      if (typeof err === "object") {
        errorMessage =
          err?.response?.data?.error?.message ||
          err?.statusText ||
          err?.message ||
          "";
      } else {
        errorMessage = err;
      }

      if (errorMessage.indexOf("password") == -1) {
        toastr.error(errorMessage, t("Common:Warning"));
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

  showVersionHistory = (id, security) => {
    const {
      fetchFileVersions,
      setIsVerHistoryPanel,
    } = this.versionHistoryStore;

    if (this.treeFoldersStore.isRecycleBinFolder) return;

    fetchFileVersions(id + "", security);
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

  onCopyLink = (item, t) => {
    const { href } = item;

    if (href) {
      copy(href);

      return toastr.success(t("Translations:LinkCopySuccess"));
    }

    const { canConvert } = this.settingsStore;

    const { getItemUrl } = this.filesStore;

    const needConvert = canConvert(item.fileExst);

    const canOpenPlayer =
      item.viewAccessability?.ImageView || item.viewAccessability?.MediaView;

    const url = getItemUrl(
      item.id,
      item.isRoom || item.isFolder,
      needConvert,
      canOpenPlayer
    );

    copy(url);

    toastr.success(t("Translations:LinkCopySuccess"));
  };

  onClickLinkEdit = (item) => {
    const { setConvertItem, setConvertDialogVisible } = this.dialogsStore;
    const canConvert =
      item.viewAccessability?.Convert && item.security?.Convert;

    if (canConvert) {
      setConvertItem({ ...item, isOpen: true });
      setConvertDialogVisible(true);
    } else {
      this.gotoDocEditor(false, item);
    }
  };

  onPreviewClick = (item) => {
    this.gotoDocEditor(true, item);
  };

  gotoDocEditor = (preview = false, item) => {
    const { isDesktopClient } = this.authStore.settingsStore;

    const { id, providerKey, fileExst } = item;

    const urlFormation = preview
      ? combineUrl(
          window.DocSpaceConfig?.proxy?.url,
          config.homepage,
          `/doceditor?fileId=${encodeURIComponent(id)}&action=view`
        )
      : null;

    let tab =
      !isDesktopClient && fileExst
        ? window.open(
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              config.homepage,
              `/doceditor`
            ),
            "_blank"
          )
        : null;

    this.filesStore.openDocEditor(id, providerKey, tab, urlFormation, preview);
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
    // localStorage.setItem("isFirstUrl", window.location.href);
    this.mediaViewerDataStore.saveFirstUrl(window.location.href);
    this.mediaViewerDataStore.changeUrl(itemId);
  };

  onClickDeleteSelectedFolder = (t, isRoom) => {
    const {
      setIsFolderActions,
      setDeleteDialogVisible,
      setIsRoomDelete,
    } = this.dialogsStore;
    const { confirmDelete } = this.settingsStore;
    const { deleteAction, deleteRoomsAction } = this.filesActionsStore;
    const { id: selectedFolderId } = this.selectedFolderStore;
    const {
      isThirdPartySelection,
      getFolderInfo,
      setBufferSelection,
    } = this.filesStore;

    setIsFolderActions(true);

    if (confirmDelete || isThirdPartySelection) {
      getFolderInfo(selectedFolderId).then((data) => {
        setBufferSelection(data);
        setIsRoomDelete(isRoom);
        setDeleteDialogVisible(true);
      });

      return;
    }

    let translations;

    if (isRoom) {
      translations = {
        successRemoveRoom: t("Files:RoomRemoved"),
        successRemoveRooms: t("Files:RoomsRemoved"),
      };

      deleteRoomsAction([selectedFolderId], translations).catch((err) =>
        toastr.error(err)
      );
    } else {
      translations = {
        deleteOperation: t("Translations:DeleteOperation"),
        deleteFromTrash: t("Translations:DeleteFromTrash"),
        deleteSelectedElem: t("Translations:DeleteSelectedElem"),
        FolderRemoved: t("Files:FolderRemoved"),
      };

      deleteAction(translations, [selectedFolderId], true).catch((err) =>
        toastr.error(err)
      );
    }
  };

  onClickDelete = (item, t) => {
    const { id, title, providerKey, rootFolderId, isFolder, isRoom } = item;

    const {
      setRemoveItem,
      setDeleteThirdPartyDialogVisible,
    } = this.dialogsStore;

    if (id === this.selectedFolderStore.id) {
      this.onClickDeleteSelectedFolder(t, isRoom);

      return;
    }

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

          if (options[index].items.length === 1) {
            options[index] = options[index].items[0];
          }
        }
      }
    }

    return options.filter((o) => !!o);
  };

  onShowInfoPanel = (item) => {
    const { setSelection, setIsVisible } = this.authStore.infoPanelStore;

    setSelection(item);
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

    const { isGracePeriod } = this.authStore.currentTariffStatusStore;
    const { isFreeTariff } = this.authStore.currentQuotaStore;

    if (isGracePeriod) {
      this.dialogsStore.setInviteUsersWarningDialogVisible(true);
    } else {
      this.dialogsStore.setInvitePanelOptions({
        visible: true,
        roomId: action ? action : e,
        hideSelector: false,
        defaultAccess: ShareAccessRights.ReadOnly,
      });
    }
  };

  onClickPin = (e, id, t) => {
    const data = (e.currentTarget && e.currentTarget.dataset) || e;
    const { action } = data;

    this.filesActionsStore.setPinAction(action, id, t);
  };

  onClickArchive = (e) => {
    const data = (e.currentTarget && e.currentTarget.dataset) || e;
    const { action } = data;
    const { isGracePeriod } = this.authStore.currentTariffStatusStore;
    const {
      setArchiveDialogVisible,
      setRestoreRoomDialogVisible,
      setInviteUsersWarningDialogVisible,
    } = this.dialogsStore;

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

  onSelect = (item) => {
    const { onSelectItem } = this.filesActionsStore;

    onSelectItem({ id: item.id, isFolder: item.isFolder }, true, false);
  };

  onShowEditingToast = (t) => {
    toastr.error(t("Files:DocumentEdited"));
  };

  onClickMute = (e, item, t) => {
    const data = (e.currentTarget && e.currentTarget.dataset) || e;
    const { action } = data;

    this.filesActionsStore.setMuteAction(action, item, t);
  };

  getRoomsRootContextOptions = (item, t) => {
    const { id, rootFolderId } = this.selectedFolderStore;
    const isRootRoom = item.isRoom && rootFolderId === id;

    if (!isRootRoom) return { pinOptions: [], muteOptions: [] };

    const pinOptions = [
      {
        id: "option_pin-room",
        key: "pin-room",
        label: t("PinToTop"),
        icon: PinReactSvgUrl,
        onClick: (e) => this.onClickPin(e, item.id, t),
        disabled: false,
        "data-action": "pin",
        action: "pin",
      },
      {
        id: "option_unpin-room",
        key: "unpin-room",
        label: t("Unpin"),
        icon: UnpinReactSvgUrl,
        onClick: (e) => this.onClickPin(e, item.id, t),
        disabled: false,
        "data-action": "unpin",
        action: "unpin",
      },
    ];

    const muteOptions = [
      {
        id: "option_unmute-room",
        key: "unmute-room",
        label: t("EnableNotifications"),
        icon: UnmuteReactSvgUrl,
        onClick: (e) => this.onClickMute(e, item, t),
        disabled: false,
        "data-action": "unmute",
        action: "unmute",
      },
      {
        id: "option_mute-room",
        key: "mute-room",
        label: t("DisableNotifications"),
        icon: MuteReactSvgUrl,
        onClick: (e) => this.onClickMute(e, item, t),
        disabled: false,
        "data-action": "mute",
        action: "mute",
      },
    ];

    return { pinOptions, muteOptions };
  };
  getFilesContextOptions = (item, t, isInfoPanel) => {
    const { contextOptions, isEditing } = item;

    const { enablePlugins } = this.authStore.settingsStore;

    const isRootThirdPartyFolder =
      item.providerKey && item.id === item.rootFolderId;

    const isShareable = item.canShare;

    const isMedia =
      item.viewAccessability?.ImageView || item.viewAccessability?.MediaView;

    const hasInfoPanel = contextOptions.includes("show-info");

    //const emailSendIsDisabled = true;
    const showSeparator0 = hasInfoPanel || !isMedia; // || !emailSendIsDisabled;

    const separator0 = showSeparator0
      ? {
          key: "separator0",
          isSeparator: true,
        }
      : false;

    const onlyShowVersionHistory =
      !contextOptions.includes("finalize-version") &&
      contextOptions.includes("show-version-history");

    const versionActions =
      !isMobile && !isMobileUtils() && !isTabletUtils()
        ? onlyShowVersionHistory
          ? [
              {
                id: "option_show-version-history",
                key: "show-version-history",
                label: t("ShowVersionHistory"),
                icon: HistoryReactSvgUrl,
                onClick: () => this.showVersionHistory(item.id, item.security),
                disabled: false,
              },
            ]
          : [
              {
                id: "option_version",
                key: "version",
                label: t("VersionHistory"),
                icon: HistoryFinalizedReactSvgUrl,
                items: [
                  {
                    id: "option_finalize-version",
                    key: "finalize-version",
                    label: t("FinalizeVersion"),
                    icon: HistoryFinalizedReactSvgUrl,
                    onClick: () =>
                      isEditing
                        ? this.onShowEditingToast(t)
                        : this.finalizeVersion(item.id, item.security),
                    disabled: false,
                  },
                  {
                    id: "option_version-history",
                    key: "show-version-history",
                    label: t("ShowVersionHistory"),
                    icon: HistoryReactSvgUrl,
                    onClick: () =>
                      this.showVersionHistory(item.id, item.security),
                    disabled: false,
                  },
                ],
              },
            ]
        : [
            {
              id: "option_finalize-version",
              key: "finalize-version",
              label: t("FinalizeVersion"),
              icon: HistoryFinalizedReactSvgUrl,
              onClick: () =>
                isEditing
                  ? this.onShowEditingToast(t)
                  : this.finalizeVersion(item.id),
              disabled: false,
            },
            {
              id: "option_version-history",
              key: "show-version-history",
              label: t("ShowVersionHistory"),
              icon: HistoryReactSvgUrl,
              onClick: () => this.showVersionHistory(item.id, item.security),
              disabled: false,
            },
          ];
    const moveActions =
      !isMobile && !isMobileUtils() && !isTabletUtils() && !isInfoPanel
        ? [
            {
              id: "option_move-or-copy",
              key: "move",
              label: t("MoveOrCopy"),
              icon: CopyReactSvgUrl,
              items: [
                {
                  id: "option_move-to",
                  key: "move-to",
                  label: t("Common:MoveTo"),
                  icon: MoveReactSvgUrl,
                  onClick: isEditing
                    ? () => this.onShowEditingToast(t)
                    : this.onMoveAction,
                  disabled: false,
                },
                {
                  id: "option_copy-to",
                  key: "copy-to",
                  label: t("Common:Copy"),
                  icon: CopyReactSvgUrl,
                  onClick: this.onCopyAction,
                  disabled: false,
                },
                {
                  id: "option_create-copy",
                  key: "copy",
                  label: t("Common:Duplicate"),
                  icon: DuplicateReactSvgUrl,
                  onClick: () => this.onDuplicate(item, t),
                  disabled: false,
                },
              ],
            },
          ]
        : [
            {
              id: "option_move-to",
              key: "move-to",
              label: t("Common:MoveTo"),
              icon: MoveReactSvgUrl,
              onClick: isEditing
                ? () => this.onShowEditingToast(t)
                : this.onMoveAction,
              disabled: false,
            },
            {
              id: "option_copy-to",
              key: "copy-to",
              label: t("Common:Copy"),
              icon: CopyReactSvgUrl,
              onClick: this.onCopyAction,
              disabled: false,
            },
            {
              id: "option_create-copy",
              key: "copy",
              label: t("Common:Duplicate"),
              icon: DuplicateReactSvgUrl,
              onClick: () => this.onDuplicate(item, t),
              disabled: false,
            },
          ];

    const { pinOptions, muteOptions } = this.getRoomsRootContextOptions(
      item,
      t
    );

    const withOpen = item.id !== this.selectedFolderStore.id;

    const optionsModel = [
      {
        id: "option_select",
        key: "select",
        label: t("Common:SelectAction"),
        icon: CheckBoxReactSvgUrl,
        onClick: () => this.onSelect(item),
        disabled: false,
      },
      withOpen && {
        id: "option_open",
        key: "open",
        label: t("Open"),
        icon: FolderReactSvgUrl,
        onClick: () => this.onOpenFolder(item),
        disabled: false,
      },
      {
        id: "option_fill-form",
        key: "fill-form",
        label: t("Common:FillFormButton"),
        icon: FormFillRectSvgUrl,
        onClick: () => this.onClickLinkFillForm(item),
        disabled: false,
      },
      {
        id: "option_edit",
        key: "edit",
        label: t("Common:EditButton"),
        icon: AccessEditReactSvgUrl,
        onClick: () => this.onClickLinkEdit(item),
        disabled: false,
      },
      {
        id: "option_preview",
        key: "preview",
        label: t("Common:Preview"),
        icon: EyeReactSvgUrl,
        onClick: () => this.onPreviewClick(item),
        disabled: false,
      },
      {
        id: "option_view",
        key: "view",
        label: t("Common:View"),
        icon: EyeReactSvgUrl,
        onClick: (fileId) => this.onMediaFileClick(fileId, item),
        disabled: false,
      },
      {
        id: "option_make-form",
        key: "make-form",
        label: t("Common:MakeForm"),
        icon: FormPlusReactSvgUrl,
        onClick: () => this.onClickMakeForm(item, t),
        disabled: false,
      },
      separator0,
      {
        id: "option_reconnect-storage",
        key: "reconnect-storage",
        label: t("Common:ReconnectStorage"),
        icon: ReconnectSvgUrl,
        onClick: () => this.onClickReconnectStorage(item, t),
        disabled: false,
      },
      {
        id: "option_edit-room",
        key: "edit-room",
        label: t("EditRoom"),
        icon: SettingsReactSvgUrl,
        onClick: () => this.onClickEditRoom(item),
        disabled: false,
      },
      {
        id: "option_invite-users-to-room",
        key: "invite-users-to-room",
        label: t("Common:InviteUsers"),
        icon: PersonReactSvgUrl,
        onClick: (e) => this.onClickInviteUsers(e),
        disabled: false,
        action: item.id,
      },
      ...versionActions,
      {
        id: "option_link-for-room-members",
        key: "link-for-room-members",
        label: t("LinkForRoomMembers"),
        icon: InvitationLinkReactSvgUrl,
        onClick: () => this.onCopyLink(item, t),
        disabled: false,
      },
      {
        id: "option_room-info",
        key: "room-info",
        label: t("Common:Info"),
        icon: InfoOutlineReactSvgUrl,
        onClick: () => this.onShowInfoPanel(item),
        disabled: false,
      },
      ...pinOptions,
      ...muteOptions,
      {
        id: "option_sharing-settings",
        key: "sharing-settings",
        label: t("SharingPanel:SharingSettingsTitle"),
        icon: ShareReactSvgUrl,
        onClick: this.onClickShare,
        disabled: !isShareable,
      },
      {
        id: "option_owner-change",
        key: "owner-change",
        label: t("Translations:OwnerChange"),
        icon: FileActionsOwnerReactSvgUrl,
        onClick: this.onOwnerChange,
        disabled: false,
      },
      {
        id: "option_link-for-portal-users",
        key: "link-for-portal-users",
        label: t("LinkForPortalUsers"),
        icon: InvitationLinkReactSvgUrl,
        onClick: () => this.onClickLinkForPortal(item, t),
        disabled: false,
      },
      // {
      //   id: "option_send-by-email",
      //   key: "send-by-email",
      //   label: t("SendByEmail"),
      //   icon: MailReactSvgUrl,
      //   disabled: emailSendIsDisabled,
      // },
      {
        id: "option_show-info",
        key: "show-info",
        label: t("Common:Info"),
        icon: InfoOutlineReactSvgUrl,
        onClick: () => this.onShowInfoPanel(item),
        disabled: false,
      },
      {
        id: "option_block-unblock-version",
        key: "block-unblock-version",
        label: t("UnblockVersion"),
        icon: LockedReactSvgUrl,
        onClick: () => this.lockFile(item, t),
        disabled: false,
      },
      {
        key: "separator1",
        isSeparator: true,
      },
      {
        id: "option_open-location",
        key: "open-location",
        label: t("OpenLocation"),
        icon: FolderLocationReactSvgUrl,
        onClick: () => this.onOpenLocation(item),
        disabled: false,
      },
      {
        id: "option_mark-read",
        key: "mark-read",
        label: t("MarkRead"),
        icon: TickRoundedSvgUrl,
        onClick: () => this.onClickMarkRead(item),
        disabled: false,
      },
      {
        id: "option_mark-as-favorite",
        key: "mark-as-favorite",
        label: t("MarkAsFavorite"),
        icon: FavoritesReactSvgUrl,
        onClick: (e) => this.onClickFavorite(e, item.id, t),
        disabled: false,
        "data-action": "mark",
        action: "mark",
      },
      {
        id: "option_remove-from-favorites",
        key: "remove-from-favorites",
        label: t("RemoveFromFavorites"),
        icon: FavoritesReactSvgUrl,
        onClick: (e) => this.onClickFavorite(e, item.id, t),
        disabled: false,
        "data-action": "remove",
        action: "remove",
      },
      {
        id: "option_download",
        key: "download",
        label: t("Common:Download"),
        icon: DownloadReactSvgUrl,
        onClick: () => this.onClickDownload(item, t),
        disabled: false,
      },
      {
        id: "option_download-as",
        key: "download-as",
        label: t("Translations:DownloadAs"),
        icon: DownloadAsReactSvgUrl,
        onClick: this.onClickDownloadAs,
        disabled: false,
      },
      ...moveActions,
      {
        id: "option_restore",
        key: "restore",
        label: t("Common:Restore"),
        icon: MoveReactSvgUrl,
        onClick: this.onMoveAction,
        disabled: false,
      },
      {
        id: "option_rename",
        key: "rename",
        label: t("Common:Rename"),
        icon: RenameReactSvgUrl,
        onClick: () => this.onClickRename(item),
        disabled: false,
      },
      {
        key: "separator2",
        isSeparator: true,
      },
      {
        id: "option_unsubscribe",
        key: "unsubscribe",
        label: t("RemoveFromList"),
        icon: RemoveSvgUrl,
        onClick: this.onClickUnsubscribe,
        disabled: false,
      },
      {
        id: "option_change-thirdparty-info",
        key: "change-thirdparty-info",
        label: t("Translations:ThirdPartyInfo"),
        icon: AccessEditReactSvgUrl,
        onClick: () => this.onChangeThirdPartyInfo(item.providerKey),
        disabled: false,
      },
      {
        id: "option_archive-room",
        key: "archive-room",
        label: t("MoveToArchive"),
        icon: RoomArchiveSvgUrl,
        onClick: (e) => this.onClickArchive(e),
        disabled: false,
        "data-action": "archive",
        action: "archive",
      },
      {
        id: "option_unarchive-room",
        key: "unarchive-room",
        label: t("Common:Restore"),
        icon: MoveReactSvgUrl,
        onClick: (e) => this.onClickArchive(e),
        disabled: false,
        "data-action": "unarchive",
        action: "unarchive",
      },
      {
        id: "option_delete",
        key: "delete",
        label: isRootThirdPartyFolder
          ? t("Common:Disconnect")
          : t("Common:Delete"),
        icon: TrashReactSvgUrl,
        onClick: () =>
          isEditing ? this.onShowEditingToast(t) : this.onClickDelete(item, t),
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

    if (options[0]?.isSeparator) {
      options.shift();
    }

    if (options[options.length - 1]?.isSeparator) {
      options.pop();
    }

    return options;
  };

  getGroupContextOptions = (t) => {
    const { personal } = this.authStore.settingsStore;
    const { selection, allFilesIsEditing } = this.filesStore;
    const { setDeleteDialogVisible } = this.dialogsStore;
    const {
      isRecycleBinFolder,
      isRoomsFolder,
      isArchiveFolder,
    } = this.treeFoldersStore;

    const { pinRooms, unpinRooms, deleteRooms } = this.filesActionsStore;

    if (isRoomsFolder || isArchiveFolder) {
      const isPinOption = selection.filter((item) => !item.pinned).length > 0;

      let canDelete;
      if (isRoomsFolder) {
        canDelete = selection.every((k) => k.contextOptions.includes("delete"));
      } else if (isArchiveFolder) {
        canDelete = selection.some((k) => k.contextOptions.includes("delete"));
      }

      const canArchiveRoom = selection.every((k) =>
        k.contextOptions.includes("archive-room")
      );

      const canRestoreRoom = selection.some((k) =>
        k.contextOptions.includes("unarchive-room")
      );

      let archiveOptions;

      const pinOption = isPinOption
        ? {
            key: "pin-room",
            label: t("PinToTop"),
            icon: PinReactSvgUrl,
            onClick: () => pinRooms(t),
            disabled: false,
          }
        : {
            key: "unpin-room",
            label: t("Unpin"),
            icon: UnpinReactSvgUrl,
            onClick: () => unpinRooms(t),
            disabled: false,
          };

      if (canArchiveRoom) {
        archiveOptions = {
          key: "archive-room",
          label: t("MoveToArchive"),
          icon: RoomArchiveSvgUrl,
          onClick: (e) => this.onClickArchive(e),
          disabled: false,
          "data-action": "archive",
          action: "archive",
        };
      }
      if (canRestoreRoom) {
        archiveOptions = {
          key: "unarchive-room",
          label: t("Common:Restore"),
          icon: MoveReactSvgUrl,
          onClick: (e) => this.onClickArchive(e),
          disabled: false,
          "data-action": "unarchive",
          action: "unarchive",
        };
      }

      const options = [];

      if (!isArchiveFolder) {
        options.push(pinOption);
      }

      if ((canArchiveRoom || canDelete) && !isArchiveFolder) {
        options.push({
          key: "separator0",
          isSeparator: true,
        });
      }

      options.push(archiveOptions);

      canDelete &&
        options.push({
          key: "delete-rooms",
          label: t("Common:Delete"),
          icon: TrashReactSvgUrl,
          onClick: () => deleteRooms(t),
        });

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
        icon: ShareReactSvgUrl,
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
        icon: FavoritesReactSvgUrl,
        onClick: (e) => this.onClickFavorite(e, favoriteItemsIds, t),
        disabled: !favoriteItems.length,
        "data-action": "mark",
        action: "mark",
      },
      {
        key: "remove-from-favorites",
        label: t("RemoveFromFavorites"),
        icon: FavoritesReactSvgUrl,
        onClick: (e) => this.onClickFavorite(e, removeFromFavoriteItemsIds, t),
        disabled: favoriteItems.length || !removeFromFavoriteItems.length,
        "data-action": "remove",
        action: "remove",
      },
      {
        key: "download",
        label: t("Common:Download"),
        icon: DownloadReactSvgUrl,
        onClick: () =>
          this.filesActionsStore
            .downloadAction(t("Translations:ArchivingData"))
            .catch((err) => toastr.error(err)),
        disabled: false,
      },
      {
        key: "download-as",
        label: t("Translations:DownloadAs"),
        icon: DownloadAsReactSvgUrl,
        onClick: this.onClickDownloadAs,
        disabled: !downloadAs,
      },
      {
        key: "move-to",
        label: t("Common:MoveTo"),
        icon: MoveReactSvgUrl,
        onClick: allFilesIsEditing
          ? () => this.onShowEditingToast(t)
          : this.onMoveAction,
        disabled: isRecycleBinFolder || !moveItems,
      },
      {
        key: "copy-to",
        label: t("Common:Copy"),
        icon: CopyReactSvgUrl,
        onClick: this.onCopyAction,
        disabled: isRecycleBinFolder || !copyItems,
      },
      {
        key: "restore",
        label: t("Common:Restore"),
        icon: MoveReactSvgUrl,
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
        icon: TrashReactSvgUrl,
        onClick: allFilesIsEditing
          ? () => this.onShowEditingToast(t)
          : () => {
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
