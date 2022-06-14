import React from "react";
import elementResizeDetectorMaker from "element-resize-detector";

import TileContainer from "./sub-components/TileContainer";
import Tile from "./sub-components/Tile";

const containerPadding = 32;
const tagMaxWidth = 100;
const gridGap = 4;

const VirtualRoomsTile = ({ data, sectionWidth }) => {
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
  }, [firstTile, data]);

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
      {data.map((item, index) => {
        return index === 0 ? (
          <Tile
            key={item.key}
            columnCount={columnCount}
            {...item}
            ref={firstTile}
          />
        ) : (
          <Tile key={item.key} columnCount={columnCount} {...item} />
        );
      })}
    </TileContainer>
  );
};

export default VirtualRoomsTile;
