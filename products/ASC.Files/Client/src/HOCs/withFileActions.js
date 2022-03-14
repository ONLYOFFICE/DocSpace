import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "@appserver/components/toast/toastr";
import { checkProtocol } from "../helpers/files-helpers";
import { AppServerConfig, FileStatus } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import config from "../../package.json";
import { isMobile } from "react-device-detect";

export default function withFileActions(WrappedFileItem) {
  class WithFileActions extends React.Component {
    constructor(props) {
      super(props);
    }

    onContentFileSelect = (checked, file) => {
      const { selectRowAction } = this.props;
      if (!file || file.id === -1) return;
      selectRowAction(checked, file);
    };

    fileContextClick = () => {
      const { onSelectItem, item } = this.props;
      const { id, isFolder } = item;

      id !== -1 && onSelectItem({ id, isFolder });
    };

    onHideContextMenu = () => {
      //this.props.setBufferSelection(null);
    };

    onDropZoneUpload = (files, uploadToFolder) => {
      const { t, dragging, setDragging, startUpload } = this.props;

      dragging && setDragging(false);
      startUpload(files, uploadToFolder, t);
    };

    onDrop = (items) => {
      const { fileExst, id } = this.props.item;

      if (!fileExst) {
        this.onDropZoneUpload(items, id);
      } else {
        this.onDropZoneUpload(items);
      }
    };

    onMouseDown = (e) => {
      const {
        draggable,
        setTooltipPosition,
        setStartDrag,
        isPrivacy,
        isTrashFolder,
        item,
        setBufferSelection,
        isActive,
        inProgress,
      } = this.props;

      const { isThirdPartyFolder } = item;

      const notSelectable = e.target.closest(".not-selectable");
      const isFileName = e.target.classList.contains("item-file-name");

      if (
        isPrivacy ||
        isTrashFolder ||
        (!draggable && !isFileName && !isActive) ||
        window.innerWidth < 1025 ||
        notSelectable ||
        isMobile ||
        isThirdPartyFolder ||
        inProgress
      ) {
        return e;
      }

      const mouseButton = e.which
        ? e.which !== 1
        : e.button
        ? e.button !== 0
        : false;
      const label = e.currentTarget.getAttribute("label");
      if (mouseButton || e.currentTarget.tagName !== "DIV" || label) {
        return e;
      }
      e.preventDefault();
      setTooltipPosition(e.pageX, e.pageY);
      setStartDrag(true);
      !isActive && setBufferSelection(null);
    };

    onMarkAsRead = (id) =>
      this.props.markAsRead([], [`${id}`], this.props.item);

    onMouseClick = (e) => {
      const { viewAs } = this.props;

      if (
        e.target.closest(".checkbox") ||
        e.target.tagName === "INPUT" ||
        e.target.tagName === "SPAN" ||
        e.target.tagName === "A" ||
        e.target.closest(".expandButton") ||
        e.target.closest(".badges") ||
        e.button !== 0 ||
        e.target.closest(".not-selectable")
      )
        return;

      if (viewAs === "tile") {
        if (e.target.closest(".edit-button") || e.target.tagName === "IMG")
          return;

        if (e.detail === 1) this.fileContextClick();
      } else {
        this.fileContextClick();
      }
    };
    onFilesClick = (e) => {
      const {
        isDesktop,
        //parentFolder,
        setIsLoading,
        fetchFiles,
        isMediaOrImage,
        canConvert,
        canWebEdit,
        canViewedDocs,
        item,
        isTrashFolder,
        isPrivacy,
        openDocEditor,
        //addExpandedKeys,
        setExpandedKeys,
        createNewExpandedKeys,
        setMediaViewerData,
        setConvertItem,
        setConvertDialogVisible,
        setNewBadgeCount,
      } = this.props;
      const {
        id,
        viewUrl,
        providerKey,
        fileStatus,
        encrypted,
        isFolder,
      } = item;
      if (encrypted && isPrivacy) return checkProtocol(item.id, true);

      if (isTrashFolder) return;
      if (
        (e && e.target.tagName === "INPUT") ||
        !!e.target.closest(".lock-file")
      )
        return;

      e.preventDefault();

      if (isFolder) {
        setIsLoading(true);
        //addExpandedKeys(parentFolder + "");

        fetchFiles(id, null, true, false)
          .then((data) => {
            const pathParts = data.selectedFolder.pathParts;
            const newExpandedKeys = createNewExpandedKeys(pathParts);
            setExpandedKeys(newExpandedKeys);

            setNewBadgeCount(item);
          })
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
          this.onMarkAsRead(id);

        if (canWebEdit || canViewedDocs) {
          let tab =
            !isDesktop && !isFolder
              ? window.open(
                  combineUrl(
                    AppServerConfig.proxyURL,
                    config.homepage,
                    "/doceditor"
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

    render() {
      const {
        item,
        isTrashFolder,
        draggable,
        allowShareIn,
        isPrivacy,
        actionType,
        actionExtension,
        actionId,
        sectionWidth,
        checked,
        dragging,
        isFolder,
        isDesktop,
        personal,
        canWebEdit,
        canViewedDocs,
      } = this.props;
      const { fileExst, access, id } = item;

      const isEdit =
        actionType !== null && actionId === id && fileExst === actionExtension;

      const isDragging = isFolder && access < 2 && !isTrashFolder && !isPrivacy;

      let className = isDragging ? " droppable" : "";
      if (draggable) className += " draggable";

      let value = !item.isFolder ? `file_${id}` : `folder_${id}`;
      value += draggable ? "_draggable" : `_${item.providerKey}`;

      const isShareable = allowShareIn && item.canShare;

      const isMobileView = sectionWidth < 500;

      const displayShareButton = isMobileView
        ? "26px"
        : !isShareable
        ? "38px"
        : "96px";

      const showShare =
        !isShareable ||
        isEdit ||
        (isPrivacy && (!isDesktop || !fileExst)) ||
        (personal && !canWebEdit && !canViewedDocs)
          ? false
          : true;

      const checkedProps = isEdit || id <= 0 ? false : checked;

      return (
        <WrappedFileItem
          onContentFileSelect={this.onContentFileSelect}
          fileContextClick={this.fileContextClick}
          onDrop={this.onDrop}
          onMouseDown={this.onMouseDown}
          onFilesClick={this.onFilesClick}
          onMouseClick={this.onMouseClick}
          onHideContextMenu={this.onHideContextMenu}
          getClassName={this.getClassName}
          className={className}
          isDragging={isDragging}
          value={value}
          displayShareButton={displayShareButton}
          isPrivacy={isPrivacy}
          showShare={showShare}
          checkedProps={checkedProps}
          dragging={dragging}
          isEdit={isEdit}
          {...this.props}
        />
      );
    }
  }

  return inject(
    (
      {
        auth,
        filesActionsStore,
        dialogsStore,
        treeFoldersStore,
        //selectedFolderStore,
        filesStore,
        uploadDataStore,
        mediaViewerDataStore,
        settingsStore,
        contextOptionsStore,
      },
      { item, t, history }
    ) => {
      const {
        selectRowAction,
        onSelectItem,
        markAsRead,
        setNewBadgeCount,
      } = filesActionsStore;
      const {
        setSharingPanelVisible,
        setConvertDialogVisible,
        setConvertItem,
      } = dialogsStore;
      const {
        isPrivacyFolder,
        isRecycleBinFolder,
        //addExpandedKeys,
        setExpandedKeys,
        createNewExpandedKeys,
      } = treeFoldersStore;
      const {
        dragging,
        setDragging,
        selection,
        setTooltipPosition,
        setStartDrag,
        fileActionStore,
        isFileSelected,
        setIsLoading,
        fetchFiles,
        openDocEditor,
        getFolderInfo,
        viewAs,
        bufferSelection,
        setBufferSelection,
        activeFiles,
        activeFolders,
      } = filesStore;
      const { startUpload } = uploadDataStore;
      const { type, extension, id } = fileActionStore;
      const { setMediaViewerData } = mediaViewerDataStore;

      const selectedItem = selection.find(
        (x) => x.id === item.id && x.fileExst === item.fileExst
      );

      const draggable =
        !isRecycleBinFolder && selectedItem && selectedItem.id !== id;

      const isFolder = selectedItem ? false : !item.isFolder ? false : true;

      const isMediaOrImage = settingsStore.isMediaOrImage(item.fileExst);
      const canWebEdit = settingsStore.canWebEdit(item.fileExst);
      const canConvert = settingsStore.canConvert(item.fileExst);
      const canViewedDocs = settingsStore.canViewedDocs(item.fileExst);

      const inProgress =
        activeFiles.findIndex((x) => x === item.id) !== -1 ||
        activeFolders.findIndex(
          (x) =>
            x === item.id &&
            (item.isFolder || (!item.fileExst && item.id === -1))
        ) !== -1;

      let isActive = false;

      if (
        bufferSelection &&
        bufferSelection.id === item.id &&
        bufferSelection.fileExst === item.fileExst &&
        !selection.length
      )
        isActive = true;

      return {
        t,
        item,
        selectRowAction,
        onSelectItem,
        setSharingPanelVisible,
        isPrivacy: isPrivacyFolder,
        dragging,
        setDragging,
        startUpload,
        draggable,
        setTooltipPosition,
        setStartDrag,
        history,
        isFolder,
        allowShareIn: filesStore.canShare,
        actionType: type,
        actionExtension: extension,
        actionId: id,
        checked: isFileSelected(item.id, item.parentId),
        //parentFolder: selectedFolderStore.parentId,
        setIsLoading,
        fetchFiles,
        isMediaOrImage,
        canWebEdit,
        canViewedDocs,
        canConvert,
        isTrashFolder: isRecycleBinFolder,
        openDocEditor,
        //addExpandedKeys,
        setExpandedKeys,
        createNewExpandedKeys,
        setMediaViewerData,
        getFolderInfo,
        markAsRead,
        viewAs,
        setConvertItem,
        setConvertDialogVisible,
        isDesktop: auth.settingsStore.isDesktopClient,
        personal: auth.settingsStore.personal,
        setNewBadgeCount,
        isActive,
        inProgress,
        setBufferSelection,
        getModel: contextOptionsStore.getModel,
      };
    }
  )(observer(WithFileActions));
}
