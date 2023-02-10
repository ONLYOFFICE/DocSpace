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

const elementResizeDetector = elementResizeDetectorMaker({
  strategy: "scroll",
  callOnAdd: false,
});

const FilesTileContainer = ({ filesList, t, sectionWidth, withPaging }) => {
  const tileRef = useRef(null);
  const timerRef = useRef(null);
  const [thumbSize, setThumbSize] = useState("");
  const [columnCount, setColumnCount] = useState(null);

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

      const { width } = node.getBoundingClientRect();

      const size = getThumbSize(width);

      const widthWithoutPadding = width - 32;

      const columns = Math.floor(widthWithoutPadding / 80);

      if (columns != columnCount) setColumnCount(columns);

      // console.log(
      //   `Body width: ${document.body.clientWidth} Tile width: ${width} ThumbSize: ${size}`
      // );

      if (size === thumbSize) return;

      setThumbSize(size);
    },
    [columnCount, thumbSize]
  );

  const onSetTileRef = React.useCallback((node) => {
    if (node) {
      clearTimeout(timerRef.current);
      timerRef.current = setTimeout(() => {
        onResize(node);

        if (tileRef?.current) elementResizeDetector.uninstall(tileRef.current);

        tileRef.current = node;

        elementResizeDetector.listenTo(node, onResize);
      }, 100);
    }
  }, []);

  return (
    <TileContainer
      className="tile-container"
      draggable
      useReactWindow={!withPaging}
      headingFolders={t("Translations:Folders")}
      headingFiles={t("Translations:Files")}
    >
      {filesList.map((item, index) => {
        return index % 11 == 0 ? (
          <FileTile
            id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
            key={`${item.id}_${index}`}
            item={item}
            itemIndex={index}
            sectionWidth={sectionWidth}
            selectableRef={onSetTileRef}
            thumbSize={thumbSize}
            columnCount={columnCount}
            withRef={true}
          />
        ) : (
          <FileTile
            id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
            key={`${item.id}_${index}`}
            item={item}
            itemIndex={index}
            sectionWidth={sectionWidth}
            thumbSize={thumbSize}
            columnCount={columnCount}
          />
        );
      })}
    </TileContainer>
  );
};

export default inject(({ auth, filesStore }) => {
  const { filesList } = filesStore;
  const { withPaging } = auth.settingsStore;

  return {
    filesList,
    withPaging,
  };
})(observer(FilesTileContainer));
