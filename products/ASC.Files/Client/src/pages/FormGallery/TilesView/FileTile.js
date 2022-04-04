import React from "react";
import { observer, inject } from "mobx-react";
import Tile from "./sub-components/Tile";
import { SimpleFilesTileContent } from "./StyledTileView";
import Link from "@appserver/components/link";
import { isDesktop } from "react-device-detect";

const FileTile = (props) => {
  const { item, getIcon, isSelected, setGallerySelected } = props;
  const { fileExst, title } = item;

  const src = getIcon(32, ".docxf");

  const { thumbnailUrl } = item;
  const element = <img className="react-svg-icon" src={src} />;

  return (
    <div ref={props.selectableRef}>
      <Tile
        key={item.id}
        item={item}
        thumbnail={thumbnailUrl}
        element={element}
        isSelected={isSelected}
        setGallerySelected={setGallerySelected}
      >
        <SimpleFilesTileContent
          //sideColor={theme.filesSection.tilesView.sideColor}
          isFile={fileExst}
        >
          <Link
            className="item-file-name"
            containerWidth="100%"
            type="page"
            title={title}
            fontWeight="600"
            fontSize={isDesktop ? "13px" : "14px"}
            target="_blank"
            //{...linkStyles} //TODO: OFORM
            //color={theme.filesSection.tilesView.color}
            isTextOverflow
          >
            {item.attributes.name_form}
          </Link>
        </SimpleFilesTileContent>
      </Tile>
    </div>
  );
};

export default inject(({ settingsStore, filesStore }, { item }) => {
  const { getIcon } = settingsStore;
  const { gallerySelected, setGallerySelected } = filesStore;

  const isSelected = item.id === gallerySelected;

  return {
    getIcon,
    isSelected,
    setGallerySelected,
  };
})(observer(FileTile));
