import React from "react";

import Link from "@docspace/components/link";
import { StyledUserNameLink } from "../../styles/history";

const HistoryBlockUser = ({ user, withComma }) => {
  const username = user.displayName || user.email;

  return (
    <StyledUserNameLink key={user.id} className="user">
      {user.profileUrl ? (
        <Link className="username link" href={user.profileUrl}>
          {username}
        </Link>
      ) : (
        <div className="username text">{username}</div>
      )}
      {withComma ? "," : ""}
      {withComma && <div className="space"></div>}
    </StyledUserNameLink>
  );
};

export default HistoryBlockUser;
