import React, { useEffect, useRef, useCallback, useState } from "react";
import { inject, observer } from "mobx-react";
import elementResizeDetectorMaker from "element-resize-detector";
import TileContainer from "./sub-components/TileContainer";
import FileTile from "./FileTile";

const getThumbSize = (width) => {
  let imgWidth = 216;

  if (width >= 240 && width < 264) {
    imgWidth = 240;
  } else if (width >= 264 && width < 288) {
    imgWidth = 264;
  } else if (width >= 288 && width < 312) {
    imgWidth = 288;
  } else if (width >= 312 && width < 336) {
    imgWidth = 312;
  } else if (width >= 336 && width < 360) {
    imgWidth = 336;
  } else if (width >= 360 && width < 400) {
    imgWidth = 360;
  } else if (width >= 400 && width < 440) {
    imgWidth = 400;
  } else if (width >= 440) {
    imgWidth = 440;
  }

  return `${imgWidth}x300`;
};

const getTagsSize = (width) => {
  let tagsWidth = 180;

  if (width >= 240 && width < 264) {
    tagsWidth = 204;
  } else if (width >= 264 && width < 288) {
    tagsWidth = 228;
  } else if (width >= 288 && width < 312) {
    tagsWidth = 252;
  } else if (width >= 312 && width < 336) {
    tagsWidth = 276;
  } else if (width >= 336 && width < 360) {
    tagsWidth = 300;
  } else if (width >= 360 && width < 400) {
    tagsWidth = 324;
  } else if (width >= 400 && width < 440) {
    tagsWidth = 348;
  } else if (width >= 440) {
    tagsWidth = 372;
  }

  return tagsWidth;
};

const elementResizeDetector = elementResizeDetectorMaker({
  strategy: "scroll",
  callOnAdd: false,
});

const FilesTileContainer = ({
  filesList,
  t,
  sectionWidth,
  withPaging,
  getCountTilesInRow,
}) => {
  const tileRef = useRef(null);
  const timerRef = useRef(null);
  const containerRef = useRef(null);
  const [thumbSize, setThumbSize] = useState("");
  const [columnCount, setColumnCount] = useState(null);
  const [tagsWidth, setTagsWidth] = useState(null);

  useEffect(() => {
    return () => {
      if (!tileRef?.current) return;
      clearTimeout(timerRef.current);
      elementResizeDetector.uninstall(tileRef.current);
    };
  }, []);

  const onResize = useCallback(
    (node) => {
      if (!node) return;

      const containerWidth = containerRef?.current.offsetWidth;

      const tilesCount = getCountTilesInRow();

      const maxWidth = (containerWidth - (tilesCount - 1) * 16) / tilesCount;

      const { width } = node.getBoundingClientRect();

      const currentWidth = width > maxWidth || width === 0 ? maxWidth : width;

      const widthWithoutPadding = Math.floor(currentWidth) - 36;

      const columns = Math.floor(widthWithoutPadding / 100);

      const size = getThumbSize(currentWidth);

      const newTagsWidth = getTagsSize(currentWidth);

      if (columns !== columnCount) {
        setColumnCount(columns);
      }

      if (tagsWidth !== newTagsWidth) {
        setTagsWidth(newTagsWidth);
      }
      if (size !== thumbSize) {
        setThumbSize(size);
      }
    },
    [columnCount, tagsWidth, thumbSize, getCountTilesInRow]
  );

  const onSetTileRef = React.useCallback(
    (node) => {
      if (node) {
        clearTimeout(timerRef.current);
        timerRef.current = setTimeout(() => {
          onResize(node);

          if (tileRef?.current)
            elementResizeDetector.uninstall(tileRef.current);

          tileRef.current = node;

          elementResizeDetector.listenTo(node, onResize);
        }, 0);
      }
    },
    [columnCount, tagsWidth, thumbSize, getCountTilesInRow]
  );

  return (
    <TileContainer
      className="tile-container"
      draggable
      useReactWindow={!withPaging}
      headingFolders={t("Translations:Folders")}
      headingFiles={t("Translations:Files")}
      headingRooms={t("Common:Rooms")}
      containerRef={containerRef}
    >
      {filesList.map((item, index) => {
        return index % 11 == 0 ? (
          <FileTile
            id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
            key={`${item.id}_${index}`}
            item={item}
            sectionWidth={sectionWidth}
            selectableRef={onSetTileRef}
            thumbSize={thumbSize}
            columnCount={columnCount}
            withRef={true}
            tagsWidth={tagsWidth}
            idx={index}
          />
        ) : (
          <FileTile
            id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
            key={`${item.id}_${index}`}
            item={item}
            sectionWidth={sectionWidth}
            thumbSize={thumbSize}
            columnCount={columnCount}
            tagsWidth={tagsWidth}
          />
        );
      })}
    </TileContainer>
  );
};

export default inject(({ auth, filesStore }) => {
  const { filesList, getCountTilesInRow } = filesStore;
  const { withPaging } = auth.settingsStore;

  return {
    filesList,
    withPaging,
    getCountTilesInRow,
  };
})(observer(FilesTileContainer));
