import React from "react";
import MembersLoader from "./views/MembersLoader";
import HistoryLoader from "./views/HistoryLoader";

const InfoPanelViewLoader = ({ view, data }) => {
  return view === "members" ? (
    <MembersLoader data={data} />
  ) : view === "history" ? (
    <HistoryLoader />
  ) : (
    <></>
  );
};

export default InfoPanelViewLoader;
