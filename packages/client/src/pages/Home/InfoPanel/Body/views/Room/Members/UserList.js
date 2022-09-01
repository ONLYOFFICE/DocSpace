import React from "react";
import PropTypes from "prop-types";
import User from "./User";

import { StyledUserList } from "../../../styles/members";

const UserList = ({ t, users, selfId, isExpect }) => {
  return (
    <StyledUserList>
      {Object.values(users).map((user) => (
        <User
          t={t}
          key={user.sharedTo.id}
          selfId={selfId}
          user={user.sharedTo}
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
