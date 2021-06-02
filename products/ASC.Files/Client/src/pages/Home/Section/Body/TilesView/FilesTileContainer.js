import React from "react";

const FilesTileContainer = (props) => {
  return;
  <>
    {/*<TileContainer
      className="tileContainer"
      draggable
      useReactWindow={false}
      headingFolders={t("Translations:Folders")}
      headingFiles={t("Translations:Files")}
    >
      {items.map((item) => {
        const { checked, isFolder, value, contextOptions } = item;
        const isEdit =
          !!fileActionType &&
          editingId === item.id &&
          item.fileExst === fileActionExtension;
        const contextOptionsProps =
          !isEdit && contextOptions && contextOptions.length > 0
            ? {
                contextOptions: this.getFilesContextOptions(
                  contextOptions,
                  item
                ),
              }
            : {};
        const checkedProps = isEdit || item.id <= 0 ? {} : { checked };
        const element = this.getItemIcon(item, isEdit || item.id <= 0);

        let classNameProp =
          isFolder && item.access < 2 && !isRecycleBin
            ? { className: " droppable" }
            : {};

        if (item.draggable) classNameProp.className += " draggable";

        return (
          <DragAndDrop
            {...classNameProp}
            onDrop={this.onDrop.bind(this, item)}
            onMouseDown={this.onMouseDown}
            dragging={dragging && isFolder && item.access < 2}
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
              onSelect={this.onContentRowSelect}
              editing={editingId}
              viewAs={viewAs}
              {...checkedProps}
              {...contextOptionsProps}
              //needForUpdate={this.needForUpdate}
            >
              <FilesTileContent
                item={item}
                viewer={viewer}
                culture={culture}
                onEditComplete={this.onEditComplete}
                onMediaFileClick={this.onMediaFileClick}
                openDocEditor={this.openDocEditor}
              />
            </Tile>
          </DragAndDrop>
        );
      })}
    </TileContainer>*/}
  </>;
};

export default FilesTileContainer;
