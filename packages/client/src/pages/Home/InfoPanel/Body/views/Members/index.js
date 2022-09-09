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

  useEffect(async () => {
    if (!selectionParentRoom) return;
    if (selectionParentRoom.members) {
      setMembers(selectionParentRoom.members);
      return;
    }
    const fetchedMembers = await getRoomMembers(selectionParentRoom.id);
    setMembers(fetchedMembers);
    setSelectionParentRoom({
      ...selectionParentRoom,
      members: fetchedMembers,
    });
  }, [selectionParentRoom]);

  useEffect(async () => {
    if (!selection.isRoom) return;
    const fetchedMembers = await getRoomMembers(selection.id);
    setMembers(fetchedMembers);
  }, [selection]);

  if (!members) return <Loaders.InfoPanelMemberListLoader />;

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
