import React from "react";
import PropTypes from "prop-types";
import { StyledUser } from "../../styles/VirtualRoom/members";
import Avatar from "@appserver/components/avatar";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";

const User = ({ t, user, selfId }) => {
  return (
    <StyledUser key={user.id} canEdit>
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
      <div className="name">
        {user.displayName || user.email}
        {selfId === user.id && (
          <span className="secondary-info">{` (${t("Common:MeLabel")})`}</span>
        )}
      </div>
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
          dropdownType="alwaysDashed"
          data={[
            {
              key: "key1",
              label: "Button 1",
              onClick: () => {},
            },
            {
              key: "key2",
              label: "Button 2",
              onClick: function noRefCheck() {},
            },
            {
              isSeparator: true,
              key: "key3",
            },
            {
              key: "key4",
              label: "Button 3",
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
