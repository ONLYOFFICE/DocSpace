import React from "react";
import { useHistory } from "react-router-dom";

import Link from "@docspace/components/link";
import { StyledUserNameLink } from "../../styles/history";

const HistoryBlockUser = ({ user, withComma, openUser }) => {
  const username = user.displayName || user.email;
  const history = useHistory();

  const onUserClick = () => {
    openUser(user.id, history);
  };

  return (
    <StyledUserNameLink key={user.id} className="user">
      <Link className="username link" onClick={onUserClick}>
        {username}
      </Link>
      {withComma ? "," : ""}
      {withComma && <div className="space"></div>}
    </StyledUserNameLink>
  );
};

export default HistoryBlockUser;
