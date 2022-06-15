import React from "react";
import Members from "./Members";
import History from "./History";
import { inject, observer } from "mobx-react";

const VirtualRoomInfoPanel = ({ t, roomState, selfId, personal, culture }) => {
  return (
    <>
      {roomState === "members" ? (
        <Members t={t} selfId={selfId} />
      ) : roomState === "history" ? (
        <History t={t} personal={personal} culture={culture} />
      ) : (
        <>details</>
      )}
    </>
  );
};

export default inject(({ auth }) => {
  const selfId = auth.userStore.user.id;
  return { selfId };
})(observer(VirtualRoomInfoPanel));
