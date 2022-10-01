import React from "react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders/index.js";

import NoGalleryItem from "./NoGalleryItem";
import NoRoomItem from "./NoRoomItem";
import NoFileOrFolderItem from "./NoFileOrFolderItem";
import NoAccountsItem from "./NoAccountsItem";

const NoItem = ({ t, isAccounts, isGallery, isRooms, isFiles }) => {
  if (isAccounts) return <NoAccountsItem t={t} />;
  if (isGallery) return <NoGalleryItem t={t} />;
  if (isFiles) return <NoFileOrFolderItem t={t} />;
  if (isRooms) return <NoRoomItem t={t} />;
  return null;
};

export default withTranslation(["InfoPanel", "FormGallery"])(
  withLoader(NoItem)(<Loaders.InfoPanelViewLoader view="noItem" />)
);
