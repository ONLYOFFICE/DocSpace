import React from "react";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import DragAndDrop from "@appserver/components/drag-and-drop";
import { withRouter } from "react-router-dom";

import Tile from "./sub-components/Tile";
import FilesTileContent from "./FilesTileContent";

const FileTile = (props) => {
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
  } = props;
  const {
    id,
    isFolder,
    access,
    draggable,
    icon,
    fileExst,
    isPrivacy,
    actionId,
    contextOptions,
  } = item;

  const getItemIcon = (isEdit) => {
    const svgLoader = () => <div style={{ width: "24px" }}></div>;
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

  const onDrop = (items) => {
    if (!fileExst) {
      onDropZoneUpload(items, item.id);
    } else {
      onDropZoneUpload(items, selectedFolderId);
    }
  };

  const onContentRowSelect = (checked, file) => {
    if (!file) return;

    selectRowAction(checked, file);
  };

  let classNameProp =
    isFolder && access < 2 && !props.isRecycleBin
      ? { className: " dropable" }
      : {};

  if (draggable) classNameProp.className += " draggable";

  const isEdit =
    !!actionType && actionId === id && fileExst === actionExtension;

  const element = getItemIcon(isEdit || id <= 0, item);

  const contextOptionsProps =
    !isEdit && contextOptions && contextOptions.length > 0
      ? {
          contextOptions: getContextOptions(item, t, history),
        }
      : {};

  let value = fileExst ? `file_${id}` : `folder_${id}`;
  value += draggable ? "_draggable" : "";

  const checkedProps = isEdit || id <= 0 ? {} : { checked };
  console.log(isFolder);
  return (
    <DragAndDrop
      {...classNameProp}
      onDrop={onDrop} //this.onDrop.bind(this, item)}
      //onMouseDown={this.onMouseDown}
      dragging={dragging && isFolder && access < 2}
      key={`dnd-key_${id}`}
      {...contextOptionsProps}
      value={value}
      isFolder={isFolder}
    >
      <Tile
        key={id}
        item={item}
        isFolder={isFolder}
        element={element}
        onSelect={onContentRowSelect}
        //editing={editingId}
        //viewAs={viewAs}
        {...checkedProps}
        {...contextOptionsProps}
        //needForUpdate={this.needForUpdate}
      >
        <FilesTileContent
          item={item}
          //viewer={viewer}
          //culture={culture}
          //onEditComplete={this.onEditComplete} !!!
          //onMediaFileClick={this.onMediaFileClick} !!!
          //openDocEditor={this.openDocEditor}
        />
      </Tile>
    </DragAndDrop>
  );
};

export default inject(
  (
    { filesStore, initFilesStore, contextOptionsStore, filesActionsStore },
    { item }
  ) => {
    const {
      type: actionType,
      extension: actionExtension,
    } = filesStore.fileActionStore;

    const { dragging, setDragging } = initFilesStore;
    const { getContextOptions } = contextOptionsStore;

    const { selectRowAction } = filesActionsStore;

    const { selection } = filesStore;

    return {
      actionType,
      actionExtension,
      dragging,
      setDragging,
      getContextOptions,
      checked: selection.some((el) => el.id === item.id),
      selectRowAction,
    };
  }
)(withTranslation("Home")(observer(withRouter(FileTile))));
