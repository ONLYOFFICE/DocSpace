import React from "react";

import { StyledUser } from "../../styles/members";
import Avatar from "@docspace/components/avatar";
import { ComboBox } from "@docspace/components";

const User = ({
  t,
  user,
  isOwner,
  isAdmin,
  selfId,
  isExpect,
  changeUserType,
}) => {
  if (!user.displayName && !user.email) return null;

  const roles = {
    admin: {
      key: "admin",
      title: t("People:Administrator"),
      label: t("People:Administrator"),
      action: "admin",
    },
    manager: {
      key: "manager",
      title: t("People:Manager"),
      label: t("People:Manager"),
      action: "manager",
    },
    user: {
      key: "user",
      title: t("Common:User"),
      label: t("Common:User"),
      action: "user",
    },
  };

  const getUserRole = () => {
    if (user.isOwner) return roles.admin;
    if (user.isAdmin) return roles.manager;
    return roles.user;
  };

  const getUserOptions = () => {
    let options = [];
    if (isOwner) options.push(roles.admin);
    if (isAdmin) options.push(roles.manager);
    options.push(roles.user);
    return options;
  };

  const getNotAuthorizedToEdit = () => {
    if (selfId === user.id) return true;
    if (isOwner) return false;
    if (!isAdmin) return true;
  };

  const userRole = getUserRole();
  const userOptions = getUserOptions();
  const userNotAuthorizedToEdit = getNotAuthorizedToEdit();

  const onTypeChange = React.useCallback(
    ({ action }) => {
      changeUserType(action, [user], t, false);
    },
    [user, changeUserType, t]
  );

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
        source={
          user.avatar ||
          (user.displayName ? "" : user.email && "/static/images/@.react.svg")
        }
        userName={user.displayName}
      />

      <div className="name">{user.displayName || user.email}</div>
      {selfId === user.id && (
        <div className="me-label">&nbsp;{`(${t("Common:MeLabel")})`}</div>
      )}

      <div className="role-wrapper">
        {userNotAuthorizedToEdit ? (
          <div className="disabled-role-combobox">{userRole.label}</div>
        ) : (
          <ComboBox
            className="role-combobox"
            selectedOption={userRole}
            options={userOptions}
            onSelect={onTypeChange}
            scaled={false}
            withBackdrop={false}
            size="content"
            displaySelectedOption
            modernView
          />
        )}
      </div>
    </StyledUser>
  );
};

export default User;
