import React from "react";
import NoGalleryItem from "./NoGalleryItem";
import NoRoomItem from "./NoRoomItem";
import NoFileOrFolderItem from "./NoFileOrFolderItem";

const NoItem = ({ t, isGallery, isRoomCategory, isFileCategory }) => {
  if (isGallery) return <NoGalleryItem t={t} />;
  if (isFileCategory) return <NoFileOrFolderItem t={t} />;
  if (isRoomCategory) return <NoRoomItem t={t} />;
  return null;
};

export default NoItem;
