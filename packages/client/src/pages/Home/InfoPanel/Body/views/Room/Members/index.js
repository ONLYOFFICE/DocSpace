import React, { useState, useEffect } from "react";

import UserList from "./UserList";
import { StyledUserTypeHeader } from "../../../styles/members";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";

const Members = ({ t, selfId, selectedItem, getShareUsers }) => {
  const [members, setMembers] = useState([]);

  useEffect(async () => {
    if (selectedItem.members) {
      console.log("hasMembers");
      setMembers(selectedItem.members);
      return;
    }

    const getShareUsersData = selectedItem.isFolder
      ? [[selectedItem.id], []]
      : [[], [selectedItem.id]];
    const fetchedMembers = await getShareUsers(...getShareUsersData);
    setMembers(fetchedMembers);
    selectedItem.members = fetchedMembers;

    console.log("members", fetchedMembers);
  }, [selectedItem]);

  return (
    <>
      <StyledUserTypeHeader>
        <Text className="title">
          {t("Users in room")} : {members.length}
        </Text>
        <IconButton
          className={"icon"}
          title={t("Common:AddUsers")}
          iconName="/static/images/person+.react.svg"
          isFill={true}
          onClick={() => {}}
          size={16}
          color={"#316DAA"}
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
