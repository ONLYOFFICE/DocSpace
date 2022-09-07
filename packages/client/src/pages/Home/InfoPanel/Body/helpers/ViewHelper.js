import React from "react";

import Details from "../views/Details";
import Gallery from "../views/Gallery";
import History from "../views/History";
import Members from "../views/Members";
import NoItem from "../views/NoItem";
import SeveralItems from "../views/SeveralItems";

class ViewHelper {
  constructor(props) {
    this.defaultProps = props.defaultProps;
    this.membersProps = props.membersProps;
    this.historyProps = props.historyProps;
    this.detailsProps = props.detailsProps;
    this.galleryProps = props.galleryProps;
  }

  MembersView = () => {
    return <Members {...this.defaultProps} {...this.membersProps} />;
  };

  HistoryView = () => {
    return <History {...this.defaultProps} {...this.historyProps} />;
  };

  DetailsView = () => {
    return <Details {...this.defaultProps} {...this.detailsProps} />;
  };

  GalleryView = () => {
    return <Gallery {...this.defaultProps} {...this.galleryProps} />;
  };

  NoItemView = () => {
    return <NoItem {...this.defaultProps} />;
  };

  SeveralItemsView = () => {
    return <SeveralItems {...this.defaultProps} />;
  };
}

export default ViewHelper;
