import React from "react";
import { ReactSVG } from "react-svg";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router-dom";

import DragAndDrop from "@appserver/components/drag-and-drop";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";

import Tile from "./FilesTile/sub-components/Tile";
import FilesContent from "./FilesContent";
import SimpleFilesRow from "./FilesRow/SimpleFilesRow";
import { EncryptedFileIcon } from "./sub-components/Icons";
const svgLoader = () => <div style={{ width: "24px" }}></div>;

const FileItem = (props) => {
  const {
    t,
    history,
    item,
    actionType,
    actionExtension,
    dragging,
    getContextOptions,
    checked,
    selectRowAction,
    onSelectItem,
    sectionWidth,
    isPrivacy,
    isRecycleBin,
    canShare,
    //isFolder, take from item
    draggable,
    isRootFolder,
    actionId,
    selectedFolderId,
    setSharingPanelVisible,
    setDragging,
    setStartDrag,
    startUpload,
    viewAs,
    setTooltipPosition,
  } = props;

  const {
    id,
    fileExst,
    shared,
    access,
    contextOptions,
    icon,
    providerKey,
    isFolder,
    contentLength,
  } = item;
  const getItemIcon = (isEdit) => {
    return (
      <>
        <ReactSVG
          className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
          src={icon}
          loading={svgLoader}
        />
        {isPrivacy && fileExst && <EncryptedFileIcon isEdit={isEdit} />}
      </>
    );
  };

  const onContentFileSelect = (checked, file) => {
    if (!file) return;

    selectRowAction(checked, file); // rename to selectFileAction ?
  };

  const onClickShare = () => {
    onSelectItem(item);
    setSharingPanelVisible(true);
  };

  const getSharedButton = (shared) => {
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
        onClick={onClickShare}
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

  const fileContextClick = () => {
    onSelectItem(item);
  };

  const onDropZoneUpload = (files, uploadToFolder) => {
    const folderId = uploadToFolder ? uploadToFolder : selectedFolderId;

    dragging && setDragging(false);
    startUpload(files, folderId, t);
  };

  const onDrop = (items) => {
    if (!fileExst) {
      onDropZoneUpload(items, item.id);
    } else {
      onDropZoneUpload(items, selectedFolderId);
    }
  };

  const onMouseDown = (e) => {
    if (!draggable) {
      return;
    }

    if (
      window.innerWidth < 1025 ||
      e.target.tagName === "rect" ||
      e.target.tagName === "path"
    ) {
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

  let value = fileExst || contentLength ? `file_${id}` : `folder_${id}`;
  value += draggable ? "_draggable" : "";

  const isThirdPartyFolder = providerKey && isRootFolder; //?

  const isMobile = sectionWidth < 500;

  const isEdit =
    !!actionType && actionId === id && fileExst === actionExtension;

  const contextOptionsProps =
    !isEdit && contextOptions && contextOptions.length > 0
      ? {
          contextOptions: getContextOptions(item, t, history),
        }
      : {};

  const checkedProps = isEdit || id <= 0 ? {} : { checked };
  const element = getItemIcon(isEdit || id <= 0);
  const displayShareButton = isMobile ? "26px" : !canShare ? "38px" : "96px";

  const isDragging = isFolder && access < 2 && !isRecycleBin;

  let className = isDragging ? " droppable" : "";
  if (draggable) className += " draggable";

  const sharedButton =
    !canShare || (isPrivacy && !fileExst) || isEdit || id <= 0 || isMobile
      ? null
      : getSharedButton(shared);

  return (
    <DragAndDrop
      className={className}
      onDrop={onDrop}
      onMouseDown={onMouseDown}
      dragging={dragging && isDragging}
      {...contextOptionsProps}
      value={value}
    >
      {viewAs === "tile" ? (
        <Tile
          key={id}
          item={item}
          isFolder={isFolder}
          element={element}
          onSelect={onContentFileSelect}
          rowContextClick={fileContextClick}
          {...checkedProps}
          {...contextOptionsProps}
        >
          <FilesContent item={item} viewAs={viewAs} />
        </Tile>
      ) : (
        <SimpleFilesRow
          sectionWidth={sectionWidth}
          key={id}
          data={item}
          element={element}
          contentElement={sharedButton}
          onSelect={onContentFileSelect}
          rowContextClick={fileContextClick}
          isPrivacy={isPrivacy}
          {...checkedProps}
          {...contextOptionsProps}
          contextButtonSpacerWidth={displayShareButton}
        >
          <FilesContent
            item={item}
            sectionWidth={sectionWidth}
            viewAs={viewAs}
          />
        </SimpleFilesRow>
      )}
    </DragAndDrop>
  );
};

export default inject(
  (
    {
      filesStore,
      treeFoldersStore,
      selectedFolderStore,
      dialogsStore,
      filesActionsStore,
      uploadDataStore,
      contextOptionsStore,
    },
    { item }
  ) => {
    const {
      type: actionType,
      extension: actionExtension,
      id,
    } = filesStore.fileActionStore;

    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

    const {
      setSharingPanelVisible,
      setChangeOwnerPanelVisible, // moved store
      setMoveToPanelVisible, // moved store
      setCopyPanelVisible, // moved store
    } = dialogsStore;

    const {
      selection,
      canShare,
      fileActionStore,
      dragging,
      setDragging,
      setStartDrag,
      setTooltipPosition,
      viewAs,
    } = filesStore;
    const { isRootFolder, id: selectedFolderId } = selectedFolderStore;

    const selectedItem = selection.find(
      (x) => x.id === item.id && x.fileExst === item.fileExst
    );

    //const isFolder = selectedItem ? false : item.fileExst ? false : true;
    const draggable =
      !isRecycleBinFolder && selectedItem && selectedItem.id !== id;

    const { selectRowAction, onSelectItem } = filesActionsStore;

    const { startUpload } = uploadDataStore;

    const { getContextOptions } = contextOptionsStore;

    return {
      dragging,
      actionType,
      actionExtension,
      isPrivacy: isPrivacyFolder,
      isRecycleBin: isRecycleBinFolder,
      isRootFolder,
      canShare,
      checked: selection.some((el) => el.id === item.id),
      //isFolder,
      draggable,

      isItemsSelected: !!selection.length,

      actionId: fileActionStore.id,
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      selectRowAction,
      selectedFolderId,
      setDragging,
      setStartDrag,
      startUpload,
      onSelectItem,
      getContextOptions,
      setTooltipPosition,
      viewAs,
    };
  }
)(withRouter(withTranslation("Home")(observer(FileItem))));
