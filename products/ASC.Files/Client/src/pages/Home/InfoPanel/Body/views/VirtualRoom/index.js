import React from "react";
import Members from "./Members";

const VirtualRoomInfoPanel = ({ t, roomState }) => {
  return (
    <>
      {roomState === "members" ? (
        <Members t={t} />
      ) : roomState === "history" ? (
        <>history</>
      ) : (
        <>details</>
      )}
    </>
  );
};

export default VirtualRoomInfoPanel;
