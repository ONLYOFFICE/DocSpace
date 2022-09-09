import React from "react";
import NoGalleryItem from "./NoGalleryItem";
import NoRoomItem from "./NoRoomItem";
import NoFileOrFolderItem from "./NoFileOrFolderItem";

const NoItem = ({ t, isGallery, isRoomCategory, isFileCategory }) => {
  if (isFileCategory) return <NoFileOrFolderItem t={t} />;
  if (isRoomCategory) return <NoRoomItem t={t} />;
  if (isGallery) return <NoGalleryItem t={t} />;
  return null;
};

export default NoItem;
