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
    filesLength,
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

  const getCountOfMissingFilesTiles = (itemsLength) => {
    const division = itemsLength % countTilesInRow;
    return division ? countTilesInRow - division : 0;
  };

  const arrayTypes = [
    {
      type: "file",
      rowCount: Math.ceil(filesLength / countTilesInRow),
      rowGap: 14,
      countOfMissingTiles: getCountOfMissingFilesTiles(filesLength),
    },
    {
      type: "folder",
      rowCount: Math.ceil(foldersLength / countTilesInRow),
      rowGap: 12,
      countOfMissingTiles: getCountOfMissingFilesTiles(foldersLength),
    },
  ];

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
      countTilesInRow={countTilesInRow}
      isRooms={isRooms}
      folderHeaderHeight={35}
      defaultHeaderHeight={46}
      arrayTypes={arrayTypes}
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
    files,
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
    filesLength: files.length,
    isInfoPanelVisible,
  };
})(observer(SelectionArea));
