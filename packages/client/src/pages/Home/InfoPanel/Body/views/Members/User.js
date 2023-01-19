import React, { useState } from "react";

import { StyledUser } from "../../styles/members";
import Avatar from "@docspace/components/avatar";
import { ComboBox } from "@docspace/components";

const User = ({
  t,
  user,
  isExpect,
  membersHelper,
  currentMember,
  updateRoomMemberRole,
  selectionParentRoom,
  setSelectionParentRoom,
}) => {
  if (!selectionParentRoom) return null;
  if (!user.displayName && !user.email) return null;

  const [userIsRemoved, setUserIsRemoved] = useState(false);
  if (userIsRemoved) return null;

  const canChangeUserRole = user.canEditAccess;

  const fullRoomRoleOptions = membersHelper.getOptionsByRoomType(
    selectionParentRoom.roomType,
    canChangeUserRole
  );

  const userRole = membersHelper.getOptionByUserAccess(user.access);
  const userRoleOptions = fullRoomRoleOptions?.filter(
    (role) => role.key !== userRole.key
  );

  const onOptionClick = (option) => {
    updateRoomMemberRole(selectionParentRoom.id, {
      invitations: [{ id: user.id, access: option.access }],
      notify: false,
      sharingMessage: "",
    });

    const inRoomMembers = selectionParentRoom.members.inRoom;
    const expectedMembers = selectionParentRoom.members.expected;
    if (option.key === "remove") {
      setUserIsRemoved(true);
      setSelectionParentRoom({
        ...selectionParentRoom,
        members: {
          inRoom: inRoomMembers?.filter((m) => m.id !== user.id),
          expected: expectedMembers?.filter((m) => m.id !== user.id),
        },
      });
    } else {
      setSelectionParentRoom({
        ...selectionParentRoom,
        members: {
          inRoom: inRoomMembers?.map((m) =>
            m.id === user.id ? { ...m, access: option.access } : m
          ),
          expected: expectedMembers?.map((m) =>
            m.id === user.id ? { ...m, access: option.access } : m
          ),
        },
      });
    }
  };

  return (
    <StyledUser isExpect={isExpect} key={user.id}>
      <Avatar
        role="user"
        className="avatar"
        size="min"
        source={isExpect ? "/static/images/@.react.svg" : user.avatar || ""}
        userName={isExpect ? "" : user.displayName}
      />

      <div className="name">
        {isExpect ? user.email : user.displayName || user.email}
      </div>
      {currentMember?.id === user.id && (
        <div className="me-label">&nbsp;{`(${t("Common:MeLabel")})`}</div>
      )}

      {userRole && userRoleOptions && (
        <div className="role-wrapper">
          {canChangeUserRole ? (
            <ComboBox
              className="role-combobox"
              selectedOption={userRole}
              options={userRoleOptions}
              onSelect={onOptionClick}
              scaled={false}
              withBackdrop={false}
              size="content"
              modernView
              title={t("Common:Role")}
              manualWidth={"fit-content"}
            />
          ) : (
            <div className="disabled-role-combobox" title={t("Common:Role")}>
              {userRole.label}
            </div>
          )}
        </div>
      )}
    </StyledUser>
  );
};

export default User;
