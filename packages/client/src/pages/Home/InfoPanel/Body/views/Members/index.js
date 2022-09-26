import React, { useState, useEffect } from "react";

import UserList from "./UserList";
import { StyledUserTypeHeader } from "../../styles/members";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";

const Members = ({
  t,
  selfId,

  selection,

  selectionParentRoom,
  setSelectionParentRoom,

  getRoomMembers,
}) => {
  const [members, setMembers] = useState(null);
  const [showLoader, setShowLoader] = useState(false);
  const [loaderData, setLoaderData] = useState({
    membersCount: 4,
    pendingMembersCount: 4,
  });

  const fetchMembers = async (roomId) => {
    let timerId;
    if (members) timerId = setTimeout(() => setShowLoader(true), 1500);

    const fetchedMembers = await getRoomMembers(roomId);

    clearTimeout(timerId);

    setMembers(fetchedMembers);
    setSelectionParentRoom({
      ...selection,
      members: fetchedMembers,
    });
    setLoaderData({
      membersCount: Math.min(fetchedMembers.length, 4),
      pendingMembersCount: Math.min(0, 4),
    });
    setShowLoader(false);
  };

  useEffect(async () => {
    if (!selectionParentRoom) return;
    if (selectionParentRoom.members) {
      setMembers(selectionParentRoom.members);
      return;
    }
    fetchMembers(selectionParentRoom.id);
  }, [selectionParentRoom]);

  useEffect(async () => {
    if (!selection.isRoom) return;
    if (selectionParentRoom && selectionParentRoom.id === selection.id) return;
    fetchMembers(selection.id);
  }, [selection]);

  if (!members || showLoader)
    return <Loaders.InfoPanelViewLoader view="members" data={loaderData} />;
  return (
    <>
      <StyledUserTypeHeader>
        <Text className="title">
          {t("Users in room")} : {members?.length}
        </Text>
        <IconButton
          className={"icon"}
          title={t("Common:AddUsers")}
          iconName="/static/images/person+.react.svg"
          isFill={true}
          onClick={() => {}}
          size={16}
          color={"#A3A9AE"}
        />
      </StyledUserTypeHeader>

      <UserList t={t} users={members} selfId={selfId} />

      {/* <StyledUserTypeHeader>
        <Text className="title">{`${t("Expect people")}:`}</Text>
        <IconButton
          className={"icon"}
          title={t("Repeat invitation")}
          iconName="/static/images/e-mail+.react.svg"
          isFill={true}
          onClick={() => {}}
          size={16}
          color={"#316DAA"}
        />
      </StyledUserTypeHeader>

      <UserList t={t} users={data.members.expect} isExpect /> */}
    </>
  );
};

export default Members;
