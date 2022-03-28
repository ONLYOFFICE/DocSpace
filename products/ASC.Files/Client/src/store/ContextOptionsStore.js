import { makeAutoObservable } from "mobx";
import copy from "copy-to-clipboard";
import saveAs from "file-saver";
import { isMobile } from "react-device-detect";
import history from "@appserver/common/history";
import config from "../../package.json";
import toastr from "studio/toastr";
import { FileAction, AppServerConfig } from "@appserver/common/constants";
import combineUrl from "@appserver/common/utils/combineUrl";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";

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
    this.filesActionsStore.openLocationAction(locationId, !fileExst);
  };

  onClickLinkFillForm = (item) => {
    return this.gotoDocEditor(false, item);
  };

  onClickMakeForm = (item, t) => {
    const {
      setConvertPasswordDialogVisible,
      setFormCreationInfo,
    } = dialogsStore;
    const { title, id, folderId, fileExst } = item;

    const newTitle =
      title.substring(0, title.length - fileExst.length) +
      this.settingsStore.extsWebRestrictedEditing[0];

    this.uploadDataStore.copyAsAction(id, newTitle, folderId).catch((err) => {
      console.log("err", err);
      const isPasswordError = new RegExp(/\(password\)*$/);

      if (isPasswordError.test(err)) {
        toastr.error(t("Translations:FileProtected"), t("Common:Warning"));
        setFormCreationInfo({
          newTitle,
          fromExst: fileExst,
          toExst: this.settingsStore.extsWebRestrictedEditing[0],
          fileInfo: item,
        });
        setConvertPasswordDialogVisible(true);
      }
    });
  };

  onOpenLocation = (item) => {
    const { parentId, folderId, fileExst } = item;
    const locationId = !fileExst ? parentId : folderId;
    this.filesActionsStore.openLocationAction(locationId, !fileExst);
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

    if (!this.authStore.settingsStore.isTabletView) {
      fetchFileVersions(id + "");
      setIsVerHistoryPanel(true);
    } else {
      history.push(
        combineUrl(AppServerConfig.proxyURL, config.homepage, `/${id}/history`)
      );
    }
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

  lockFile = (item) => {
    const { id, locked } = item;

    this.filesActionsStore
      .lockFileAction(id, !locked)
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
        : `${window.location.origin + config.homepage}/filter?folder=${id}`
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
    const { id, fileExst } = item;
    this.filesStore.fileActionStore.setAction({
      type: FileAction.Rename,
      extension: fileExst,
      id,
    });
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

    const { id, title, providerKey, rootFolderId, isFolder } = item;

    const isRootThirdPartyFolder = providerKey && id === rootFolderId;

    if (isRootThirdPartyFolder) {
      const splitItem = id.split("-");
      setRemoveItem({ id: splitItem[splitItem.length - 1], title });
      setDeleteThirdPartyDialogVisible(true);
      return;
    }

    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      successRemoveFile: t("FileRemoved"),
      successRemoveFolder: t("FolderRemoved"),
    };

    this.filesActionsStore.deleteItemAction(
      id,
      translations,
      !isFolder,
      providerKey
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

  getFilesContextOptions = (item, t) => {
    const { contextOptions } = item;
    const isRootThirdPartyFolder =
      item.providerKey && item.id === item.rootFolderId;

    const isShareable = item.canShare;

    const versionActions =
      !isMobile && !isMobileUtils() && !isTabletUtils()
        ? [
            {
              key: "version",
              label: t("VersionHistory"),
              icon: "images/history-finalized.react.svg",
              items: [
                {
                  key: "finalize-version",
                  label: t("FinalizeVersion"),
                  onClick: () => this.finalizeVersion(item.id),
                  disabled: false,
                },
                {
                  key: "show-version-history",
                  label: t("ShowVersionHistory"),
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
          ];

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
                  onClick: this.onMoveAction,
                  disabled: false,
                },
                {
                  key: "copy-to",
                  label: t("Translations:Copy"),
                  onClick: this.onCopyAction,
                  disabled: false,
                },
                {
                  key: "copy",
                  label: t("Duplicate"),
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
              label: t("Duplicate"),
              icon: "/static/images/copy.react.svg",
              onClick: () => this.onDuplicate(item, t),
              disabled: false,
            },
          ];

    const optionsModel = [
      {
        key: "open",
        label: t("Open"),
        icon: "images/catalog.folder.react.svg",
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
        key: "sharing-settings",
        label: t("SharingSettings"),
        icon: "/static/images/catalog.share.react.svg",
        onClick: this.onClickShare,
        disabled: !isShareable,
      },
      {
        key: "owner-change",
        label: t("Translations:OwnerChange"),
        icon: "/static/images/catalog.user.react.svg",
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
        key: "block-unblock-version",
        label: t("UnblockVersion"),
        icon: "images/lock.react.svg",
        onClick: () => this.lockFile(item),
        disabled: false,
      },
      {
        key: "separator1",
        isSeparator: true,
      },
      {
        key: "open-location",
        label: t("OpenLocation"),
        icon: "images/download-as.react.svg",
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
        key: "delete",
        label: isRootThirdPartyFolder
          ? t("Translations:DeleteThirdParty")
          : t("Common:Delete"),
        icon: "/static/images/catalog.trash.react.svg",
        onClick: () => this.onClickDelete(item, t),
        disabled: false,
      },
    ];

    const options = this.filterModel(optionsModel, contextOptions);

    return options;
  };

  getModel = (item, t) => {
    const { type, id, extension } = this.filesStore.fileActionStore;
    const { fileExst, contextOptions } = item;

    const isEdit = !!type && id === item.id && fileExst === extension;

    const contextOptionsProps =
      !isEdit && contextOptions && contextOptions.length > 0
        ? this.getFilesContextOptions(item, t)
        : [];

    return contextOptionsProps;
  };
}

export default ContextOptionsStore;
