import React from "react";
import { inject, observer } from "mobx-react";

import { withTranslation } from "react-i18next";

import { Consumer } from "@appserver/components/utils/context";

import EmptyContainer from "./EmptyContainer";

import VirtualRoomsTile from "./TileView";
import VirtualRoomsTable from "./TableView";
import withLoader from "../../../../HOCs/withLoader";

const SectionBodyContent = ({ isEmpty, viewAs }) => {
  return (
    <Consumer>
      {(context) =>
        isEmpty ? (
          <EmptyContainer />
        ) : viewAs === "tile" ? (
          <VirtualRoomsTile sectionWidth={context.sectionWidth} />
        ) : viewAs === "row" ? (
          <></>
        ) : (
          <VirtualRoomsTable sectionWidth={context.sectionWidth} />
        )
      }
    </Consumer>
  );
};

export default inject(({ filesStore, roomsStore }) => {
  const { viewAs } = filesStore;

  const { rooms } = roomsStore;

  const isEmpty = rooms.length < 1;

  return { viewAs, isEmpty };
})(withTranslation([])(withLoader(observer(SectionBodyContent))()));
