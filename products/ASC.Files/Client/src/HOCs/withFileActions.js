import React from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";

import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import toastr from "@appserver/components/toast/toastr";

import { EncryptedFileIcon } from "../components/Icons";
import { checkProtocol, createTreeFolders } from "../helpers/files-helpers";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import config from "../../package.json";

const svgLoader = () => <div style={{ width: "24px" }}></div>;
export default function withFileActions(WrappedFileItem) {
  class WithFileActions extends React.Component {
    constructor(props) {
      super(props);

      this.state = {
        isMouseDown: false,
      };
    }
    onContentFileSelect = (checked, file) => {
      const { selectRowAction } = this.props;
      if (!file || file.id === -1) return;
      selectRowAction(checked, file);
    };

    onClickShare = () => {
      const { onSelectItem, setSharingPanelVisible, item } = this.props;
      onSelectItem(item);
      setSharingPanelVisible(true);
    };

    fileContextClick = () => {
      const { onSelectItem, item } = this.props;

      item.id !== -1 && onSelectItem(item);
    };

    getSharedButton = (shared) => {
      const { t } = this.props;
      const color = shared ? "#657077" : "#a3a9ae";
      return (
        <Text
          className="share-button"
          as="span"
          title={t("Share")}
          fontSize="12px"
          fontWeight={600}
          color={color}
          display="inline-flex"
          onClick={this.onClickShare}
        >
          <IconButton
            className="share-button-icon"
            color={color}
            hoverColor="#657077"
            size={18}
            iconName="images/catalog.shared.react.svg"
          />
          {t("Share")}
        </Text>
      );
    };

    getItemIcon = (isEdit) => {
      const { item, isPrivacy, viewAs } = this.props;
      const { icon, fileExst } = item;
      return (
        <>
          <ReactSVG
            className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
            src={icon}
            loading={svgLoader}
          />
          {isPrivacy && fileExst && (
            <EncryptedFileIcon isEdit={isEdit && viewAs !== "tile"} />
          )}
        </>
      );
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

      this.setState({ isMouseDown: true });

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

    onMouseUpHandler = (e) => {
      const { isMouseDown } = this.state;
      const { viewAs, checked, item } = this.props;

      if (
        e.target.closest(".checkbox") ||
        e.target.tagName === "INPUT" ||
        e.target.tagName === "SPAN" ||
        e.target.tagName === "A" ||
        e.target.closest(".expandButton") ||
        e.target.closest(".badges") ||
        e.button !== 0
      )
        return;

      if (viewAs === "tile") {
        if (
          !isMouseDown ||
          e.target.closest(".edit-button") ||
          e.target.tagName === "IMG"
        )
          return;

        this.onFilesClick();
      } else {
        this.fileContextClick();
      }
      this.setState({ isMouseDown: false });
    };
    onFilesClick = (e) => {
      const {
        isDesktop,
        parentFolder,
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
        expandedKeys,
        addExpandedKeys,
        setExpandedKeys,
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
        if (!expandedKeys.includes(parentFolder + "")) {
          addExpandedKeys(parentFolder + "");
        }

        fetchFiles(id)
          .then((data) => {
            const pathParts = data.selectedFolder.pathParts;
            const newExpandedKeys = createTreeFolders(pathParts, expandedKeys);
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
        canShare,
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
      if (draggable) className += " draggable not-selectable";

      let value = fileExst || contentLength ? `file_${id}` : `folder_${id}`;
      value += draggable ? "_draggable" : "";

      const isMobile = sectionWidth < 500;
      const displayShareButton = isMobile
        ? "26px"
        : !canShare
        ? "38px"
        : "96px";

      const showShare = isPrivacy && (!isDesktop || !fileExst) ? false : true;

      const sharedButton =
        !canShare ||
        !showShare ||
        (personal && !canWebEdit && !canViewedDocs) ||
        isEdit ||
        id <= 0 ||
        isMobile
          ? null
          : this.getSharedButton(shared);

      const checkedProps = isEdit || id <= 0 ? {} : { checked };
      const element = this.getItemIcon(isEdit || id <= 0);

      return (
        <WrappedFileItem
          onContentFileSelect={this.onContentFileSelect}
          onClickShare={this.onClickShare}
          fileContextClick={this.fileContextClick}
          onDrop={this.onDrop}
          onMouseDown={this.onMouseDown}
          onFilesClick={this.onFilesClick}
          onMouseUp={this.onMouseUpHandler}
          getClassName={this.getClassName}
          className={className}
          isDragging={isDragging}
          value={value}
          displayShareButton={displayShareButton}
          isPrivacy={isPrivacy}
          sharedButton={sharedButton}
          checkedProps={checkedProps}
          element={element}
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
        selectedFolderStore,
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
        expandedKeys,
        addExpandedKeys,
        setExpandedKeys,
      } = treeFoldersStore;
      const { isRootFolder } = selectedFolderStore;
      const {
        dragging,
        setDragging,
        selection,
        setTooltipPosition,
        setStartDrag,
        fileActionStore,
        canShare,
        isFileSelected,
        filter,
        setIsLoading,
        fetchFiles,
        openDocEditor,
        getFolderInfo,
        viewAs,
      } = filesStore;
      const { startUpload } = uploadDataStore;
      const { type, extension, id } = fileActionStore;
      const {
        iconFormatsStore,
        mediaViewersFormatsStore,
        docserviceStore,
      } = formatsStore;
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
        isRootFolder,
        canShare,
        actionType: type,
        actionExtension: extension,
        actionId: id,
        checked: isFileSelected(item.id, item.parentId),
        filter,
        parentFolder: selectedFolderStore.parentId,
        setIsLoading,
        fetchFiles,
        isMediaOrImage,
        canWebEdit,
        canViewedDocs,
        canConvert,
        isTrashFolder: isRecycleBinFolder,
        openDocEditor,
        expandedKeys,
        addExpandedKeys,
        setExpandedKeys,
        setMediaViewerData,
        getFolderInfo,
        markAsRead,
        viewAs,
        setConvertItem,
        setConvertDialogVisible,
        isDesktop: auth.settingsStore.isDesktopClient,
        personal: auth.settingsStore.personal,
      };
    }
  )(observer(WithFileActions));
}
