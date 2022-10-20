import ContextMenuButton from "@appserver/components/context-menu-button";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import React from "react";
import { StyledTitle } from "../../styles/styles";
import { StyledUserTypeHeader } from "../../styles/VirtualRoom/members";
import { fillingFormsVR } from "../mock_data";
import UserList from "./UserList";

const Members = ({ t, selfId }) => {
  const data = fillingFormsVR;

  return (
    <>
      <StyledTitle withBottomBorder>
        <img className="icon" src={data.icon} alt="thumbnail-icon" />
        <Text className="text">{data.title}</Text>
        <ContextMenuButton getData={() => {}} className="context-menu-button" />
      </StyledTitle>

      <StyledUserTypeHeader>
        <Text className="title">
          {t("Users in room")} : {data.members.inRoom.length}
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

      <UserList t={t} users={data.members.inRoom} selfId={selfId} />

      <StyledUserTypeHeader>
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

      <UserList t={t} users={data.members.expect} isExpect />
    </>
  );
};

export default Members;
