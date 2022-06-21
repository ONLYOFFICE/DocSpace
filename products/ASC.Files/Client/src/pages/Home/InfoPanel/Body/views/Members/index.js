import Avatar from "@appserver/components/avatar";
import ContextMenuButton from "@appserver/components/context-menu-button";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import React from "react";
import { StyledTitle } from "../../styles/styles";
import {
  StyledUser,
  StyledUserList,
  StyledUserTypeHeader,
} from "../../styles/VirtualRoom/members";
import { fillingFormsVR } from "../mock_data";

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

      <StyledUserList>
        {data.members.inRoom.map((user) => (
          <StyledUser key={user.id} canEdit>
            <Avatar
              role="user"
              className="avatar"
              size="min"
              source={
                user.avatar ||
                (user.displayName
                  ? ""
                  : user.email && "/static/images/@.react.svg")
              }
              userName={user.displayName}
            />
            <div className="name">
              {user.displayName || user.email}
              {selfId === user.id && (
                <span className="secondary-info">
                  {" (" + t("Common:MeLabel") + ")"}
                </span>
              )}
            </div>
          </StyledUser>
        ))}
      </StyledUserList>

      <StyledUserTypeHeader>
        <Text className="title">
          {t("Expect people")} : {data.members.expect.length}
        </Text>
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

      <StyledUserList>
        {data.members.expect.map((user) => (
          <StyledUser key={user.id} canEdit>
            <Avatar
              role="user"
              className="avatar"
              size="min"
              source={
                user.avatar ||
                (user.displayName
                  ? ""
                  : user.email && "/static/images/@.react.svg")
              }
              userName={user.displayName}
            />
            <Text truncate className="name">
              {user.displayName || user.email}
            </Text>
          </StyledUser>
        ))}
      </StyledUserList>
    </>
  );
};

export default Members;
