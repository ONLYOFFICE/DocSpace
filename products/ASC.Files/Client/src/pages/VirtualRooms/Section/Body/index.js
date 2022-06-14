import React from "react";
import { inject, observer } from "mobx-react";

import { Consumer } from "@appserver/components/utils/context";

import EmptyContainer from "./EmptyContainer";

import VirtualRoomsTile from "./TileView";
import VirtualRoomsTable from "./TableView";

const SectionBodyContent = ({ isEmpty, viewAs, data }) => {
  return (
    <Consumer>
      {(context) =>
        isEmpty ? (
          <EmptyContainer />
        ) : viewAs === "tile" ? (
          <VirtualRoomsTile sectionWidth={context.sectionWidth} data={data} />
        ) : viewAs === "row" ? (
          <></>
        ) : (
          <VirtualRoomsTable sectionWidth={context.sectionWidth} data={data} />
        )
      }
    </Consumer>
  );
};

export default inject(({ filesStore }) => {
  const { viewAs } = filesStore;

  return { viewAs };
})(observer(SectionBodyContent));
