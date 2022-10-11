import React from "react";
import PropTypes from "prop-types";
import User from "./User";

import { StyledUserList } from "../../styles/VirtualRoom/members";

const UserList = ({ t, users, selfId, isExpect }) => {
  return (
    <StyledUserList>
      {users.map((user) => (
        <User
          t={t}
          key={user.id}
          selfId={selfId}
          user={user}
          isExpect={isExpect}
        />
      ))}
    </StyledUserList>
  );
};

UserList.propTypes = {
  users: PropTypes.array,
};

export default UserList;
