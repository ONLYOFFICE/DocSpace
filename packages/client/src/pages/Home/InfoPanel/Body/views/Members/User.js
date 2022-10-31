import React, { useState } from "react";

import { StyledUser } from "../../styles/members";
import Avatar from "@docspace/components/avatar";
import { ComboBox } from "@docspace/components";
import MembersHelper from "../../helpers/MembersHelper";
import { ShareAccessRights } from "@docspace/common/constants";

const User = ({
  t,
  user,
  isExpect,
  membersHelper,
  roomId,
  roomType,
  currentMember,
  updateRoomMemberRole,
}) => {
  if (!user.displayName && !user.email) return null;

  const fullRoomRoleOptions = membersHelper.getOptionsByRoomType(roomType);

  const [userRole, setUserRole] = useState(
    membersHelper.getOptionByUserAccess(user.access)
  );
  const [userRoleOptions, setUserRoleOptions] = useState(
    fullRoomRoleOptions.filter((role) => role.key !== userRole.key)
  );

  const onRoomRoleChange = (option) => {
    setUserRole(option);
    setUserRoleOptions(
      fullRoomRoleOptions.filter((role) => role.key !== option.key)
    );

    updateRoomMemberRole(roomId, {
      invitations: [
        {
          id: user.id,
          access: option.access,
        },
      ],
      notify: false,
      sharingMessage: "",
    });
  };

  return (
    <StyledUser
      isExpect={isExpect}
      key={user.id}
      canEditRole={user.role !== "Owner"}
    >
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
      {currentMember.id === user.id && (
        <div className="me-label">&nbsp;{`(${t("Common:MeLabel")})`}</div>
      )}

      {userRole && userRoleOptions && (
        <div className="role-wrapper">
          {currentMember?.access === ShareAccessRights.FullAccess ||
          currentMember?.access === ShareAccessRights.RoomManager ? (
            <ComboBox
              className="role-combobox"
              selectedOption={userRole}
              options={userRoleOptions}
              onSelect={onRoomRoleChange}
              scaled={false}
              withBackdrop={false}
              size="content"
              modernView
            />
          ) : (
            <div className="disabled-role-combobox">{userRole.label}</div>
          )}
        </div>
      )}
    </StyledUser>
  );
};

export default User;
