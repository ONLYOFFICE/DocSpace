import React, { useState, useEffect } from "react";

import ContextMenuButton from "@docspace/components/context-menu-button";
import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import { StyledTitle } from "../../../styles/styles";
import { StyledUserTypeHeader } from "../../../styles/VirtualRoom/members";
import UserList from "./UserList";

const Members = ({ t, selfId, selectedItem, getShareUsers }) => {
  const [members, setMembers] = useState([]);

  useEffect(async () => {
    const fetchedMembers = await getShareUsers(
      [selectedItem.parentId],
      [selectedItem.id]
    );
    setMembers(fetchedMembers);
    console.log("members", fetchedMembers);

    console.log(selectedItem.logo);
  }, []);

  return (
    <>
      <StyledTitle withBottomBorder>
        <img
          className="icon is-room"
          src={
            selectedItem.logo && selectedItem.logo.big
              ? selectedItem.logo.big
              : selectedItem.icon
          }
          alt="thumbnail-icon"
        />
        <Text className="text">{selectedItem.title}</Text>
        <ContextMenuButton getData={() => {}} className="context-menu-button" />
      </StyledTitle>

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
