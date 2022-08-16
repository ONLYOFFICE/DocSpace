import React from "react";
import { withRouter } from "react-router";

import RowContent from "@docspace/components/row-content";
import Link from "@docspace/components/link";

import Badges from "../Badges";

const UserContent = ({
  item,
  sectionWidth,

  onUserNameClick,

  theme,
}) => {
  const { userName, displayName, email, statusType } = item;

  const nameColor =
    statusType === "pending"
      ? theme.peopleTableRow.pendingNameColor
      : theme.peopleTableRow.nameColor;
  const sideInfoColor =
    statusType === "pending"
      ? theme.peopleTableRow.pendingSideInfoColor
      : theme.peopleTableRow.sideInfoColor;

  return (
    <RowContent
      sideColor={sideInfoColor}
      sectionWidth={sectionWidth}
      nameColor={nameColor}
      sideInfoColor={sideInfoColor}
    >
      <Link
        containerWidth="28%"
        type="page"
        href={`/accounts/view/${userName}`}
        title={displayName}
        fontWeight={600}
        onClick={onUserNameClick}
        fontSize="15px"
        color={nameColor}
        isTextOverflow={true}
      >
        {displayName}
      </Link>
      <Badges statusType={statusType} />
      <Link
        containerMinWidth="140px"
        containerWidth="17%"
        type="page"
        title={email}
        fontSize="12px"
        fontWeight={400}
        color={sideInfoColor}
        isTextOverflow={true}
      >
        TODO: Type
      </Link>
      <Link
        containerMinWidth="140px"
        containerWidth="17%"
        type="page"
        title={email}
        fontSize="12px"
        fontWeight={400}
        color={sideInfoColor}
        isTextOverflow={true}
      >
        {email}
      </Link>
    </RowContent>
  );
};

export default withRouter(UserContent);
