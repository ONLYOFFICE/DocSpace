import React from "react";
import MembersLoader from "./views/MembersLoader";
import HistoryLoader from "./views/HistoryLoader";
import DetailsLoader from "./views/DetailsLoader";
import AccountsLoader from "./views/AccountsLoader";
import GalleryLoader from "./views/GalleryLoader";
import NoItemLoader from "./views/NoItemLoader";
import SeveralItemsLoader from "./views/SeveralItemsLoader";

const InfoPanelViewLoader = ({ view }) => {
  switch (view) {
    case "members":
      return <MembersLoader />;
    case "history":
      return <HistoryLoader />;
    case "details":
      return <DetailsLoader />;
    case "gallery":
      return <GalleryLoader />;
    case "accounts":
      return <AccountsLoader />;
    case "noItem":
      return <NoItemLoader />;
    case "severalItems":
      return <SeveralItemsLoader />;
    default:
      return <DetailsLoader />;
  }
};

export default InfoPanelViewLoader;
