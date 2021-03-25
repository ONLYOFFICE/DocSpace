import React from "react";
import { inject, observer } from "mobx-react";
import { Consumer } from "@appserver/components/utils/context";
import TileContainer from "./TileContainer";
import FilesTileContent from "./FilesTileContent";
import Tile from "./Tile";
import DragAndDrop from "@appserver/components/drag-and-drop";

const FilesTileContainer = ({ t, filesList, fileActionType, ...props }) => {
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
                    contextOptions: props.getFilesContextOptions(item),
                  }
                : {};
            const checkedProps = isEdit || item.id <= 0 ? {} : { checked };
            const element = props.getItemIcon(
              isEdit || item.id <= 0,
              item.icon,
              item.fileExst
            );

            let classNameProp =
              isFolder && item.access < 2 && !isRecycleBin
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

export default inject(({ filesStore }) => {
  const { filesList, fileActionStore } = filesStore;
  const { type: fileActionType } = fileActionStore;

  return { filesList, fileActionType };
})(observer(FilesTileContainer));
