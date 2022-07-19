import React from "react";
import PropTypes from "prop-types";
import User from "./User";

import { StyledUserList } from "../../styles/VirtualRoom/members";

const UserList = ({ t, users, selfId }) => {
  return (
    <StyledUserList>
      {users.map((user) => (
        <User t={t} key={user.id} selfId={selfId} user={user} />
      ))}
    </StyledUserList>
  );
};

UserList.propTypes = {
  users: PropTypes.array,
};

export default UserList;
