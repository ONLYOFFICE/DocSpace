import React, { useState, useEffect } from "react";

import UserList from "./UserList";
import { StyledUserTypeHeader } from "../../../styles/members";

import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";

const Members = ({ t, selfId, selection, setSelection, getShareUsers }) => {
  const [isLoading, setIsLoading] = useState(false);

  useEffect(async () => {
    console.log(selection);
    console.log("MEMBERS USE STATE");
    if (selection.members) {
      console.log("NO NEED TO REFETCH");
      return;
    }

    console.log("NEED TO REFETCH");
    setIsLoading(true);
    const getShareUsersData = selection.isFolder
      ? [[selection.id], []]
      : [[], [selection.id]];
    const fetchedMembers = await getShareUsers(...getShareUsersData);
    setSelection({ ...selection, members: fetchedMembers });
    setIsLoading(false);

    console.log("members", fetchedMembers);
  }, [selection.members]);

  if (!selection.members || isLoading)
    return <Loaders.InfoPanelMemberListLoader />;

  return (
    <>
      <StyledUserTypeHeader>
        <Text className="title">
          {t("Users in room")} : {selection.members.length}
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

      <UserList t={t} users={selection.members} selfId={selfId} />

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
