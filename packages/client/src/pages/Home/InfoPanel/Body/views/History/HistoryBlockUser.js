import React from "react";
import { useHistory } from "react-router-dom";

import Link from "@docspace/components/link";
import { StyledUserNameLink } from "../../styles/history";
import { getUserRole } from "@docspace/client/src/helpers/people-helpers";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";

const HistoryBlockUser = ({
  user,
  withComma,
  selectUser,
  selfId,
  getUserById,
  getUserContextOptions,
  getStatusType,
  setSelectedFolder,
  setSelectedNode,
}) => {
  const username = user.displayName || user.email;
  const history = useHistory();

  const goToAccounts = (openSelfProfile = false) => {
    const path = [AppServerConfig.proxyURL, config.homepage, "/accounts"];
    if (openSelfProfile) path.push("/@self");

    setSelectedFolder(null);
    setSelectedNode(["accounts", "filter"]);
    history.push(combineUrl(...path));
  };

  const onUserClick = async () => {
    if (user.id === selfId) {
      goToAccounts(true);
      return;
    }

    goToAccounts(false);
    const fetchedUser = await getUserById(user.id);
    fetchedUser.role = getUserRole(fetchedUser);
    fetchedUser.statusType = getStatusType(fetchedUser);
    fetchedUser.options = getUserContextOptions(
      false,
      fetchedUser.isOwner,
      fetchedUser.statusType,
      fetchedUser.status
    );
    selectUser(fetchedUser);
  };

  return (
    <StyledUserNameLink key={user.id} className="user">
      {user.profileUrl ? (
        <Link className="username link" onClick={onUserClick}>
          {username}
        </Link>
      ) : (
        <div className="username text" onClick={onUserClick}>
          {username}
        </div>
      )}
      {withComma ? "," : ""}
      {withComma && <div className="space"></div>}
    </StyledUserNameLink>
  );
};

export default HistoryBlockUser;
