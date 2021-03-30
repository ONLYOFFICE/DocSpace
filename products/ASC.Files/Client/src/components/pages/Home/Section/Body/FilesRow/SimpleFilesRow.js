import React from "react";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import DragAndDrop from "@appserver/components/drag-and-drop";
import Row from "@appserver/components/row";
import FilesRowContent from "./FilesRowContent";
import { withRouter } from "react-router-dom";

const StyledSimpleFilesRow = styled(Row)`
  margin-top: -2px;
  ${(props) =>
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
      }
  `}

  .share-button-icon {
    margin-right: 7px;
    margin-top: -1px;
  }

  .share-button:hover,
  .share-button-icon:hover {
    cursor: pointer;
    color: #657077;
    path {
      fill: #657077;
    }
  }
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media (max-width: 1312px) {
    .share-button {
      padding-top: 3px;
    }
  }

  .styled-element {
    margin-right: 7px;
  }
`;

const EncryptedFileIcon = styled.div`
  background: url("images/security.svg") no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: ${(props) => (props.isEdit ? "40px" : "12px")};
`;

const svgLoader = () => <div style={{ width: "24px" }}></div>;

const SimpleFilesRow = (props) => {
  const {
    t,
    item,
    sectionWidth,
    actionType,
    actionExtension,
    isPrivacy,
    isRecycleBin,
    dragging,
    checked,
    canShare,
    isFolder,
    draggable,
    isRootFolder,
    actionId,
    selectedFolderId,
    setSharingPanelVisible,
    setChangeOwnerPanelVisible,
    setMoveToPanelVisible,
    setCopyPanelVisible,
    selectRowAction,
    setDragging,
    startUpload,
    onSelectItem,
    history,
  } = props;
  console.log("render row");
  const {
    id,
    fileExst,
    shared,
    access,
    contextOptions,
    icon,
    providerKey,
  } = item;

  let value = fileExst ? `file_${id}` : `folder_${id}`;
  value += draggable ? "_draggable" : "";

  const isThirdPartyFolder = providerKey && isRootFolder;

  const onContentRowSelect = (checked, file) => {
    if (!file) return;

    selectRowAction(checked, file);
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

  const rowContextClick = () => {
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

  const isMobile = sectionWidth < 500;

  const isEdit =
    !!actionType && actionId === id && fileExst === actionExtension;
  const contextOptionsProps =
    !isEdit && contextOptions && contextOptions.length > 0
      ? {
          contextOptions: props.getContextOptions(item, t, history),
        }
      : {};

  const checkedProps = isEdit || id <= 0 ? {} : { checked };
  const element = getItemIcon(isEdit || id <= 0);
  const displayShareButton = isMobile ? "26px" : !canShare ? "38px" : "96px";
  let className = isFolder && access < 2 && !isRecycleBin ? " dropable" : "";
  if (draggable) className += " draggable";

  const sharedButton =
    !canShare || (isPrivacy && !fileExst) || isEdit || id <= 0 || isMobile
      ? null
      : getSharedButton(shared);

  return (
    <DragAndDrop
      className={className}
      onDrop={onDrop}
      //onMouseDown={this.onMouseDown}
      dragging={dragging && isFolder && access < 2}
      {...contextOptionsProps}
      value={value}
    >
      <StyledSimpleFilesRow
        sectionWidth={sectionWidth}
        key={id}
        data={item}
        element={element}
        contentElement={sharedButton}
        onSelect={onContentRowSelect}
        rowContextClick={rowContextClick}
        isPrivacy={isPrivacy}
        {...checkedProps}
        {...contextOptionsProps}
        contextButtonSpacerWidth={displayShareButton}
      >
        <FilesRowContent item={item} sectionWidth={sectionWidth} />
      </StyledSimpleFilesRow>
    </DragAndDrop>
  );
};

export default inject(
  (
    {
      auth,
      initFilesStore,
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
    const { dragging, setDragging } = initFilesStore;
    const { type, extension, id } = filesStore.fileActionStore;
    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

    const {
      setSharingPanelVisible,
      setChangeOwnerPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
    } = dialogsStore;

    const { selection, canShare, fileActionStore } = filesStore;

    const { isRootFolder, id: selectedFolderId } = selectedFolderStore;

    const selectedItem = selection.find(
      (x) => x.id === item.id && x.fileExst === item.fileExst
    );

    const isFolder = selectedItem ? false : item.fileExst ? false : true;
    const draggable =
      !isRecycleBinFolder && selectedItem && selectedItem.id !== id;

    const { selectRowAction, onSelectItem } = filesActionsStore;

    const { startUpload } = uploadDataStore;

    const { getContextOptions } = contextOptionsStore;

    return {
      dragging,
      actionType: type,
      actionExtension: extension,
      isPrivacy: isPrivacyFolder,
      isRecycleBin: isRecycleBinFolder,
      isRootFolder,
      canShare,
      checked: selection.some((el) => el.id === item.id),
      isFolder,
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
      startUpload,
      onSelectItem,
      getContextOptions,
    };
  }
)(withTranslation("Home")(observer(withRouter(SimpleFilesRow))));
