import React from "react";
import { inject, observer } from "mobx-react";

import { withTranslation } from "react-i18next";

import { Consumer } from "@appserver/components/utils/context";

import EmptyContainer from "./EmptyContainer";

import VirtualRoomsTile from "./TileView";
import VirtualRoomsRow from "./RowView";
import VirtualRoomsTable from "./TableView";

import withLoader from "../../../../HOCs/withLoader";

const SectionBodyContent = ({
  isEmpty,
  viewAs,

  setSelection,
  setBufferSelection,
}) => {
  React.useEffect(() => {
    window.addEventListener("mousedown", onMouseDown);

    return () => {
      window.removeEventListener("mousedown", onMouseDown);
    };
  }, []);

  const onMouseDown = React.useCallback((e) => {
    if (
      (e.target.closest(".scroll-body") &&
        !e.target.closest(".rooms-item") &&
        !e.target.closest(".not-selectable") &&
        !e.target.closest(".info-panel") &&
        !e.target.closest(".table-container_group-menu")) ||
      e.target.closest(".files-main-button") ||
      e.target.closest(".add-button") ||
      e.target.closest(".search-input-block")
    ) {
      setSelection([]);
      setBufferSelection(null);
    }
  }, []);

  return (
    <Consumer>
      {(context) =>
        isEmpty ? (
          <EmptyContainer />
        ) : viewAs === "tile" ? (
          <VirtualRoomsTile sectionWidth={context.sectionWidth} />
        ) : viewAs === "row" ? (
          <VirtualRoomsRow sectionWidth={context.sectionWidth} />
        ) : (
          <VirtualRoomsTable sectionWidth={context.sectionWidth} />
        )
      }
    </Consumer>
  );
};

export default inject(({ filesStore, roomsStore }) => {
  const { viewAs } = filesStore;

  const { rooms, setSelection, setBufferSelection } = roomsStore;

  const isEmpty = rooms.length === 0;

  return { viewAs, isEmpty, setSelection, setBufferSelection };
})(withTranslation([])(withLoader(observer(SectionBodyContent))()));
