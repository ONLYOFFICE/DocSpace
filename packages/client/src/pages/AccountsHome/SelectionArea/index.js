import React from "react";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import SelectionAreaComponent from "@docspace/components/selection-area";

const SelectionArea = (props) => {
  const { viewAs, setSelections } = props;

  const onMove = ({ added, removed, clear }) => {
    setSelections(added, removed, clear);
  };

  const itemHeight = viewAs === "table" ? 49 : 59;

  return isMobile ? (
    <></>
  ) : (
    <SelectionAreaComponent
      containerClass="section-scroll"
      scrollClass="section-scroll"
      itemsContainerClass="ReactVirtualized__Grid__innerScrollContainer"
      selectableClass="window-item"
      itemClass="user-item"
      onMove={onMove}
      viewAs={viewAs}
      itemHeight={itemHeight}
    />
  );
};

export default inject(({ peopleStore }) => {
  const { viewAs } = peopleStore;
  const { setSelections } = peopleStore.selectionStore;

  return {
    viewAs,
    setSelections,
  };
})(observer(SelectionArea));
