import React from "react";
import PropTypes from "prop-types";
import { StyledUser } from "../../../styles/VirtualRoom/members";
import Avatar from "@docspace/components/avatar";
import LinkWithDropdown from "@docspace/components/link-with-dropdown";

const User = ({ t, user, selfId, isExpect }) => {
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
        <LinkWithDropdown
          className="role"
          containerMinWidth="fit-content"
          directionX="right"
          directionY="bottom"
          fontSize="13px"
          fontWeight={600}
          hasScroll={true}
          withBackdrop={false}
          dropdownType="appearDashedAfterHover"
          isDisabled={user.role === "Owner"}
          data={[
            {
              key: "key1",
              label: "Room manager",
              onClick: () => {},
            },
            {
              key: "key2",
              label: "Co-worker",
              onClick: function noRefCheck() {},
            },
            {
              key: "key3",
              label: "Viewer",
              onClick: function noRefCheck() {},
            },
            {
              isSeparator: true,
              key: "key4",
            },
            {
              key: "key5",
              label: "Remove",
              onClick: function noRefCheck() {},
            },
          ]}
        >
          {user.role}
        </LinkWithDropdown>
      </div>
    </StyledUser>
  );
};

User.propTypes = { user: PropTypes.object };

export default User;
