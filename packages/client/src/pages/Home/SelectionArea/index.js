import React, { useState, useEffect } from "react";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import SelectionAreaComponent from "@docspace/components/selection-area";

const SelectionArea = ({
  dragging,
  viewAs,
  setSelections,
  getCountTilesInRow,
}) => {
  if (isMobile || dragging) return <></>;

  const [countTilesInRow, setCountTilesInRow] = useState(getCountTilesInRow());

  useEffect(() => {
    setTilesCount();
    window.addEventListener("resize", onResize);

    return () => {
      window.removeEventListener("resize", onResize);
    };
  });

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

  return (
    <SelectionAreaComponent
      containerClass="section-scroll"
      scrollClass="section-scroll"
      itemsContainerClass="ReactVirtualized__Grid__innerScrollContainer"
      selectableClass={selectableClass}
      onMove={onMove}
      viewAs={viewAs}
      itemHeight={itemHeight}
      countTilesInRow={countTilesInRow}
    />
  );
};

export default inject(({ filesStore }) => {
  const { dragging, viewAs, setSelections, getCountTilesInRow } = filesStore;

  return {
    dragging,
    viewAs,
    setSelections,
    getCountTilesInRow,
  };
})(observer(SelectionArea));
