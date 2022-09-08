import React from "react";
import NoGalleryItem from "./NoGalleryItem";
import NoRoomItem from "./NoRoomItem";
import NoFileOrFolderItem from "./NoFileOrFolderItem";

const NoItem = ({ t, selection, isGallery }) => {
  if (isGallery) return <NoGalleryItem t={t} />;
  if (selection.isRoom) return <NoRoomItem t={t} />;
  return <NoFileOrFolderItem t={t} />;
};

export default NoItem;
