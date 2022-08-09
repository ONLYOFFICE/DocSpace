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

const FilesTileContainer = ({ filesList, t, sectionWidth }) => {
  const firstRef = useRef();
  const [thumbSize, setThumbSize] = useState("");
  const [columnCount, setColumnCount] = useState(null);

  useEffect(() => {
    if (!firstRef?.current) return;

    onResize(); //Rerender tiles here

    const elementResizeDetector = elementResizeDetectorMaker({
      strategy: "scroll",
      callOnAdd: false,
    });

    elementResizeDetector.listenTo(firstRef.current, onResize);

    return () => {
      if (!firstRef?.current) return;

      elementResizeDetector.uninstall(firstRef.current);
    };
  }, [firstRef, filesList]);

  const onResize = useCallback(() => {
    if (!firstRef?.current) return;
    const { width } = firstRef.current.getBoundingClientRect();

    const size = getThumbSize(width);

    const widthWithoutPadding = width - 32;

    const columns = Math.floor(widthWithoutPadding / 80);

    if (columns != columnCount) setColumnCount(columns);

    // console.log(
    //   `Body width: ${document.body.clientWidth} Tile width: ${width} ThumbSize: ${size}`
    // );

    if (size === thumbSize) return;

    setThumbSize(size);
  }, [firstRef]);

  return (
    <TileContainer
      className="tile-container"
      draggable
      useReactWindow
      headingFolders={t("Folders")}
      headingFiles={t("Files")}
    >
      {filesList.map((item, index) => {
        return index == 0 ? (
          <FileTile
            id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
            key={`${item.id}_${index}`}
            item={item}
            sectionWidth={sectionWidth}
            selectableRef={firstRef}
            thumbSize={thumbSize}
            columnCount={columnCount}
          />
        ) : (
          <FileTile
            id={`${item?.isFolder ? "folder" : "file"}_${item.id}`}
            key={`${item.id}_${index}`}
            item={item}
            sectionWidth={sectionWidth}
            thumbSize={thumbSize}
            columnCount={columnCount}
          />
        );
      })}
    </TileContainer>
  );
};

export default inject(({ filesStore }) => {
  const { filesList } = filesStore;

  return {
    filesList,
  };
})(observer(FilesTileContainer));
