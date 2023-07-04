import React, { useState, useEffect } from "react";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import SelectionAreaComponent from "@docspace/components/selection-area";

const SelectionArea = (props) => {
  const {
    dragging,
    viewAs,
    setSelections,
    getCountTilesInRow,
    isRooms,
    foldersLength,
    isInfoPanelVisible,
  } = props;

  const [countTilesInRow, setCountTilesInRow] = useState(getCountTilesInRow());

  useEffect(() => {
    setTilesCount();
    window.addEventListener("resize", onResize);

    return () => {
      window.removeEventListener("resize", onResize);
    };
  }, [isInfoPanelVisible]);

  const onResize = () => {
    setTilesCount();
  };

  const setTilesCount = () => {
    const newCount = getCountTilesInRow();
    if (countTilesInRow !== newCount) setCountTilesInRow(newCount);
  };

  const onMove = ({ added, removed, clear }) => {
    setSelections(added, removed, clear);
  };

  const selectableClass = viewAs === "tile" ? "files-item" : "window-item";
  const itemHeight = viewAs === "table" ? 49 : viewAs === "row" ? 59 : null;

  const countRowsOfFolders = Math.ceil(foldersLength / countTilesInRow);
  const division = foldersLength % countTilesInRow;
  const countOfMissingTiles = division ? countTilesInRow - division : 0;

  // const itemsContainer = document.getElementsByClassName(itemsContainerClass);
  // const folderHeaderHeight = itemsContainer[0]
  //   .getElementsByClassName("folder_header")[0]
  //   .parentElement.getBoundingClientRect().height;
  // const filesHeaderHeight = itemsContainer[0]
  //   .getElementsByClassName("files_header")[0]
  //   .parentElement.getBoundingClientRect().height;

  return isMobile || dragging ? (
    <></>
  ) : (
    <SelectionAreaComponent
      containerClass="section-scroll"
      scrollClass="section-scroll"
      itemsContainerClass="ReactVirtualized__Grid__innerScrollContainer"
      selectableClass={selectableClass}
      itemClass="files-item"
      onMove={onMove}
      viewAs={viewAs}
      itemHeight={itemHeight}
      countTilesInRow={countTilesInRow}
      isRooms={isRooms}
      countRowsOfFolders={countRowsOfFolders}
      countOfMissingTiles={countOfMissingTiles}
      folderTileGap={12}
      fileTileGap={14}
      foldersTileHeight={66}
      filesTileHeight={222}
      folderHeaderHeight={35}
      filesHeaderHeight={countRowsOfFolders ? 46 : 0}
    />
  );
};

export default inject(({ auth, filesStore, treeFoldersStore }) => {
  const {
    dragging,
    viewAs,
    setSelections,
    getCountTilesInRow,
    folders,
  } = filesStore;
  const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;
  const { isVisible: isInfoPanelVisible } = auth.infoPanelStore;

  const isRooms = isRoomsFolder || isArchiveFolder;

  return {
    dragging,
    viewAs,
    setSelections,
    getCountTilesInRow,
    isRooms,
    foldersLength: folders.length,
    isInfoPanelVisible,
  };
})(observer(SelectionArea));
