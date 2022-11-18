import React from "react";
import { useHistory } from "react-router-dom";

import Link from "@docspace/components/link";
import { StyledUserNameLink } from "../../styles/history";
import Text from "@docspace/components/text";
const HistoryBlockUser = ({ user, withComma, openUser, isVisitor }) => {
  const username = user.displayName;
  const history = useHistory();

  const onUserClick = () => {
    openUser(user, history);
  };

  return (
    <StyledUserNameLink key={user.id} className="user">
      {isVisitor ? (
        <Text as="span" fontWeight={600}>
          {username}
        </Text>
      ) : (
        <Link className="username link" onClick={onUserClick}>
          {username}
        </Link>
      )}
      {withComma ? "," : ""}
      {withComma && <div className="space"></div>}
    </StyledUserNameLink>
  );
};

export default HistoryBlockUser;
