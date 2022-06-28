import React from "react";
import { inject, observer } from "mobx-react";
import elementResizeDetectorMaker from "element-resize-detector";

import TileContainer from "./sub-components/TileContainer";
import Tile from "./sub-components/Tile";

const containerPadding = 32;
const tagMaxWidth = 100;
const gridGap = 4;

const VirtualRoomsTile = ({ rooms, sectionWidth }) => {
  const [columnCount, setColumnCount] = React.useState(null);

  const firstTile = React.useRef(null);

  React.useEffect(() => {
    if (!firstTile?.current) return;

    onResize();

    const elementResizeDetector = elementResizeDetectorMaker({
      strategy: "scroll",
      callOnAdd: false,
    });

    elementResizeDetector.listenTo(firstTile.current, onResize);

    return () => {
      if (!firstTile?.current) return;

      elementResizeDetector.uninstall(firstTile.current);
    };
  }, [firstTile, rooms]);

  const onResize = React.useCallback(() => {
    if (firstTile?.current) {
      const { width } = firstTile.current.getBoundingClientRect();

      const widthWithoutPadding = width - containerPadding;

      const columns = Math.floor(widthWithoutPadding / (tagMaxWidth + gridGap));

      if (columns != columnCount) setColumnCount(columns);
    }
  }, [firstTile, columnCount]);

  return (
    <TileContainer>
      {rooms.map((room, index) => {
        return index === 0 ? (
          <Tile
            key={room.id}
            columnCount={columnCount}
            {...room}
            item={room}
            ref={firstTile}
          />
        ) : (
          <Tile key={room.id} columnCount={columnCount} item={room} {...room} />
        );
      })}
    </TileContainer>
  );
};

export default inject(({ roomsStore }) => {
  const { rooms } = roomsStore;

  return { rooms };
})(observer(VirtualRoomsTile));
