import React from "react";
import { useHistory } from "react-router-dom";

import Link from "@docspace/components/link";
import { StyledUserNameLink } from "../../styles/history";

const HistoryBlockUser = ({ user, withComma, openUser, isVisitor }) => {
  const username = user.displayName;
  const history = useHistory();

  const onUserClick = () => {
    openUser(user, history);
  };

  const onClickProp = isVisitor ? {} : { onClick: onUserClick };

  return (
    <StyledUserNameLink key={user.id} className="user" isVisitor={isVisitor}>
      <Link className="username link" {...onClickProp}>
        {username}
      </Link>
      {withComma ? "," : ""}
      {withComma && <div className="space"></div>}
    </StyledUserNameLink>
  );
};

export default HistoryBlockUser;
