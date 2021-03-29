import React from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import { Consumer } from "@appserver/components/utils/context";
import { withTranslation } from "react-i18next";
import TileContainer from "./TileContainer";
import FilesTileContent from "./FilesTileContent";
import Tile from "./Tile";
import DragAndDrop from "@appserver/components/drag-and-drop";

const FilesTileContainer = ({ t, filesList, fileActionType, ...props }) => {
  const getItemIcon = (isEdit, item) => {
    const svgLoader = () => <div style={{ width: "24px" }}></div>;
    console.log;
    return (
      <>
        <ReactSVG
          className={`react-svg-icon${isEdit ? " is-edit" : ""}`}
          src={item.icon}
          loading={svgLoader}
        />
        {item.isPrivacy && item.fileExst && (
          <EncryptedFileIcon isEdit={isEdit} />
        )}
      </>
    );
  };

  return (
    <Consumer>
      {(context) => (
        <TileContainer
          className="tileContainer"
          draggable
          useReactWindow={false}
          headingFolders={t("Folders")}
          headingFiles={t("Files")}
        >
          {filesList.map((item) => {
            const { checked, isFolder, value, contextOptions } = item;
            const isEdit =
              !!fileActionType &&
              editingId === item.id &&
              item.fileExst === fileActionExtension;
            const contextOptionsProps =
              !isEdit && contextOptions && contextOptions.length > 0
                ? {
                    contextOptions: props.getContextOptions(item, t),
                  }
                : {};
            const checkedProps = isEdit || item.id <= 0 ? {} : { checked };
            const element = getItemIcon(isEdit || item.id <= 0, item);

            let classNameProp =
              isFolder && item.access < 2 && !props.isRecycleBin
                ? { className: " dropable" }
                : {};

            if (item.draggable) classNameProp.className += " draggable";

            return (
              <DragAndDrop
                {...classNameProp}
                //onDrop={this.onDrop.bind(this, item)}
                //onMouseDown={this.onMouseDown}
                //dragging={dragging && isFolder && item.access < 2}
                key={`dnd-key_${item.id}`}
                {...contextOptionsProps}
                value={value}
                isFolder={isFolder}
              >
                <Tile
                  key={item.id}
                  item={item}
                  isFolder={!item.fileExst}
                  element={element}
                  //onSelect={this.onContentRowSelect}
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
                    //onEditComplete={this.onEditComplete}
                    //onMediaFileClick={this.onMediaFileClick}
                    //openDocEditor={this.openDocEditor}
                  />
                </Tile>
              </DragAndDrop>
            );
          })}
        </TileContainer>
      )}
    </Consumer>
  );
};

export default inject(
  ({ filesStore, contextOptionsStore, treeFoldersStore }) => {
    const { filesList, fileActionStore } = filesStore;
    const { type: fileActionType } = fileActionStore;
    const { getContextOptions } = contextOptionsStore;
    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

    return {
      filesList,
      fileActionType,
      getContextOptions,
      isPrivacy: isPrivacyFolder,
      isRecycleBin: isRecycleBinFolder,
    };
  }
)(withTranslation("Home")(observer(FilesTileContainer)));
