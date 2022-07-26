import React from "react";
import { withRouter } from "react-router";
//import styled from "styled-components";

import RowContent from "@docspace/components/row-content";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import Badges from "../../../../../components/PeopleBadges";

const UserContent = ({
  item,
  sectionWidth,
  onPhoneClick,
  onEmailClick,
  onUserNameClick,
  groups,
  theme,
}) => {
  const { userName, displayName, title, mobilePhone, email, statusType } = item;

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
      {title ? (
        <Text
          containerMinWidth="120px"
          containerWidth="20%"
          as="div"
          color={sideInfoColor}
          fontSize="12px"
          fontWeight={600}
          title={title}
          truncate={true}
        >
          {title}
        </Text>
      ) : (
        <Box containerMinWidth="120px" containerWidth="20%"></Box>
      )}
      {groups}
      <Link
        containerMinWidth="60px"
        containerWidth="15%"
        type="page"
        title={mobilePhone}
        fontSize="12px"
        fontWeight={400}
        color={sideInfoColor}
        onClick={onPhoneClick}
        isTextOverflow={true}
      >
        {mobilePhone}
      </Link>
      <Link
        containerMinWidth="140px"
        containerWidth="17%"
        type="page"
        title={email}
        fontSize="12px"
        fontWeight={400}
        color={sideInfoColor}
        onClick={onEmailClick}
        isTextOverflow={true}
      >
        {email}
      </Link>
    </RowContent>
  );
};

export default withRouter(UserContent);
