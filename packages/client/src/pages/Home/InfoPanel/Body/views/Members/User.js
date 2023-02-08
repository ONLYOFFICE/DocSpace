import React, { useState } from "react";
import AtReactSvgUrl from "PUBLIC_DIR/images/@.react.svg?url";
import { StyledUser } from "../../styles/members";
import Avatar from "@docspace/components/avatar";
import { ComboBox } from "@docspace/components";
import DefaultUserPhotoUrl from "PUBLIC_DIR/images/default_user_photo_size_82-82.png";
import toastr from "@docspace/components/toast/toastr";
import { isMobileOnly } from "react-device-detect";
import { decode } from "he";

const User = ({
  t,
  user,
  isExpect,
  membersHelper,
  currentMember,
  updateRoomMemberRole,
  selectionParentRoom,
  setSelectionParentRoom,
  changeUserType,
}) => {
  if (!selectionParentRoom) return null;
  if (!user.displayName && !user.email) return null;

  //const [userIsRemoved, setUserIsRemoved] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  //if (userIsRemoved) return null;

  const canChangeUserRole = user.canEditAccess;

  const fullRoomRoleOptions = membersHelper.getOptionsByRoomType(
    selectionParentRoom.roomType,
    canChangeUserRole
  );

  const userRole = membersHelper.getOptionByUserAccess(user.access);
  const userRoleOptions = fullRoomRoleOptions?.filter(
    (role) => role.key !== userRole.key
  );

  const updateRole = (option) => {
    return updateRoomMemberRole(selectionParentRoom.id, {
      invitations: [{ id: user.id, access: option.access }],
      notify: false,
      sharingMessage: "",
    })
      .then(() => {
        setIsLoading(false);
        const inRoomMembers = selectionParentRoom.members.inRoom;
        const expectedMembers = selectionParentRoom.members.expected;
        if (option.key === "remove") {
          setSelectionParentRoom({
            ...selectionParentRoom,
            members: {
              inRoom: inRoomMembers?.filter((m) => m.id !== user.id),
              expected: expectedMembers?.filter((m) => m.id !== user.id),
            },
          });
          //setUserIsRemoved(true);
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
      })
      .catch((err) => {
        toastr.error(err);
        setIsLoading(false);
      });
  };

  const abortCallback = () => {
    setIsLoading(false);
  };

  const onOptionClick = (option) => {
    const userType =
      option.key === "owner"
        ? "admin"
        : option.key === "roomAdmin"
        ? "manager"
        : "user";

    const successCallback = () => {
      updateRole(option);
    };

    setIsLoading(true);

    if (!changeUserType(userType, [user], successCallback, abortCallback)) {
      updateRole(option);
    }
  };

  const userAvatar = user.hasAvatar ? user.avatar : DefaultUserPhotoUrl;

  return (
    <StyledUser isExpect={isExpect} key={user.id}>
      <Avatar
        role="user"
        className="avatar"
        size="min"
        source={isExpect ? AtReactSvgUrl : userAvatar || ""}
        userName={isExpect ? "" : user.displayName}
      />

      <div className="name">
        {isExpect ? user.email : decode(user.displayName) || user.email}
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
              withBackdrop={isMobileOnly}
              size="content"
              modernView
              title={t("Common:Role")}
              manualWidth={"fit-content"}
              isLoading={isLoading}
              isMobileView={isMobileOnly}
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
