import React from "react";
import GalleryEmptyScreen from "./GalleryEmptyScreen";
import GalleryItem from "./GalleryItem";

const Gallery = ({ gallerySelected, personal, culture }) => {
  return !gallerySelected ? (
    <GalleryEmptyScreen />
  ) : (
    <GalleryItem
      selectedItem={gallerySelected}
      personal={personal}
      culture={culture}
    />
  );
};

export default Gallery;
