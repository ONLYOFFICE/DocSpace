import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "@appserver/components/toast/toastr";
import { checkProtocol } from "../helpers/files-helpers";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import config from "../../package.json";

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
      } = this.props;
      const notSelectable = e.target.classList.contains("not-selectable");

      if (!draggable || isPrivacy) return;

      if (window.innerWidth < 1025 || notSelectable) {
        return;
      }

      const mouseButton = e.which
        ? e.which !== 1
        : e.button
        ? e.button !== 0
        : false;
      const label = e.currentTarget.getAttribute("label");
      if (mouseButton || e.currentTarget.tagName !== "DIV" || label) {
        return;
      }
      setTooltipPosition(e.pageX, e.pageY);
      setStartDrag(true);
    };

    onMarkAsRead = (id) =>
      this.props.markAsRead([], [`${id}`], this.props.item);

    onMouseClick = (e) => {
      const { viewAs, isItemsSelected } = this.props;

      if (
        e.target.closest(".checkbox") ||
        e.target.tagName === "INPUT" ||
        e.target.tagName === "SPAN" ||
        e.target.tagName === "A" ||
        e.target.closest(".expandButton") ||
        e.target.closest(".badges") ||
        e.button !== 0 /* ||
        isItemsSelected */
      )
        return;

      if (viewAs === "tile") {
        if (e.target.closest(".edit-button") || e.target.tagName === "IMG")
          return;

        this.onFilesClick();
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
      } = this.props;
      const {
        id,
        fileExst,
        viewUrl,
        providerKey,
        contentLength,
        fileStatus,
        encrypted,
      } = item;
      if (encrypted && isPrivacy) return checkProtocol(item.id, true);

      if (isTrashFolder) return;
      if (e && e.target.tagName === "INPUT") return;

      if (!fileExst && !contentLength) {
        setIsLoading(true);
        //addExpandedKeys(parentFolder + "");

        fetchFiles(id)
          .then((data) => {
            const pathParts = data.selectedFolder.pathParts;
            const newExpandedKeys = createNewExpandedKeys(pathParts);
            setExpandedKeys(newExpandedKeys);

            this.setNewBadgeCount();
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

        if (fileStatus === 2) this.onMarkAsRead(id);

        if (canWebEdit || canViewedDocs) {
          let tab =
            !isDesktop && fileExst
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
          setMediaViewerData({ visible: true, id });
          return;
        }

        return window.open(viewUrl, "_blank");
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
      const { fileExst, access, contentLength, id, shared } = item;

      const isEdit =
        actionType !== null && actionId === id && fileExst === actionExtension;

      const isDragging = isFolder && access < 2 && !isTrashFolder && !isPrivacy;

      let className = isDragging ? " droppable" : "";
      if (draggable) className += " draggable";

      let value = fileExst || contentLength ? `file_${id}` : `folder_${id}`;
      value += draggable ? "_draggable" : "";

      const isShareable = allowShareIn && item.canShare;

      const isMobile = sectionWidth < 500;
      const displayShareButton = isMobile
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
        formatsStore,
        mediaViewerDataStore,
      },
      { item, t, history }
    ) => {
      const { selectRowAction, onSelectItem, markAsRead } = filesActionsStore;
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
      } = filesStore;
      const { startUpload } = uploadDataStore;
      const { type, extension, id } = fileActionStore;
      const { mediaViewersFormatsStore, docserviceStore } = formatsStore;
      const { setMediaViewerData } = mediaViewerDataStore;

      const selectedItem = selection.find(
        (x) => x.id === item.id && x.fileExst === item.fileExst
      );

      const draggable =
        !isRecycleBinFolder && selectedItem && selectedItem.id !== id;

      const isFolder = selectedItem
        ? false
        : item.fileExst //|| item.contentLength
        ? false
        : true;

      const isMediaOrImage = mediaViewersFormatsStore.isMediaOrImage(
        item.fileExst
      );

      const canWebEdit = docserviceStore.canWebEdit(item.fileExst);
      const canConvert = docserviceStore.canConvert(item.fileExst);
      const canViewedDocs = docserviceStore.canViewedDocs(item.fileExst);

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
        isItemsSelected: selection.length > 0,
      };
    }
  )(observer(WithFileActions));
}
