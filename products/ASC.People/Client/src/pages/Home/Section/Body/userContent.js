import React, { useCallback } from "react";
import { withRouter } from "react-router";
import styled from "styled-components";

import RowContent from "@appserver/components/row-content";
import Link from "@appserver/components/link";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";

import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import SendClockIcon from "../../../../../public/images/send.clock.react.svg";
import CatalogSpamIcon from "../../../../../public/images/catalog.spam.react.svg";
import config from "../../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const StyledSendClockIcon = styled(SendClockIcon)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;
const StyledCatalogSpamIcon = styled(CatalogSpamIcon)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;
const getFormattedGroups = (user, selectGroup) => {
  let temp = [];
  const groups = user.groups;
  const linkColor = user.statusType === "pending" ? "#D0D5DA" : "#A3A9AE";

  if (!groups) temp.push({ key: 0, label: "" });

  groups &&
    groups.map((group) =>
      temp.push({
        key: group.id,
        label: group.name,
        onClick: () => selectGroup(group.id),
      })
    );

  if (temp.length <= 1) {
    return (
      <Link
        isTextOverflow={true}
        containerMinWidth="120px"
        containerWidth="15%"
        type="action"
        title={temp[0].label}
        fontSize="12px"
        fontWeight={400}
        color={linkColor}
        onClick={temp[0].onClick}
      >
        {temp[0].label}
      </Link>
    );
  } else {
    return (
      <LinkWithDropdown
        isTextOverflow={true}
        containerMinWidth="120px"
        containerWidth="15%"
        title={temp[0].label}
        fontSize="12px"
        fontWeight={400}
        color={linkColor}
        data={temp}
      >
        {temp[0].label}
      </LinkWithDropdown>
    );
  }
};

const UserContent = ({
  user,
  history,
  selectGroup,
  //widthProp,
  isMobile,
  sectionWidth,
  fetchProfile,
}) => {
  const { userName, displayName, title, mobilePhone, email, statusType } = user;
  const groups = getFormattedGroups(user, selectGroup);

  const redirectToProfile = () => {
    history.push(
      combineUrl(AppServerConfig.proxyURL, config.homepage, `/view/${userName}`)
    );
  };

  const onUserNameClick = useCallback(
    (e) => {
      const timer = setTimeout(() => redirectToProfile(), 500);
      e.preventDefault();
      fetchProfile(userName).finally(() => {
        clearTimeout(timer);
        if (
          combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            `/view/${userName}`
          ) !== window.location.pathname
        )
          redirectToProfile();
      });
    },
    [history, userName]
  );

  const onPhoneClick = useCallback(() => window.open(`sms:${mobilePhone}`), [
    mobilePhone,
  ]);

  const onEmailClick = useCallback(() => window.open(`mailto:${email}`), [
    email,
  ]);

  const nameColor = statusType === "pending" ? "#A3A9AE" : "#333333";
  const sideInfoColor = statusType === "pending" ? "#D0D5DA" : "#A3A9AE";

  return (
    <RowContent
      //widthProp={widthProp}
      isMobile={isMobile}
      sideColor={sideInfoColor}
      sectionWidth={sectionWidth}
    >
      <Link
        containerWidth="28%"
        type="page"
        href={`/products/people/view/${userName}`}
        title={displayName}
        fontWeight={600}
        onClick={onUserNameClick}
        fontSize="15px"
        color={nameColor}
        isTextOverflow={true}
      >
        {displayName}
      </Link>
      <>
        {statusType === "pending" && <StyledSendClockIcon size="small" />}
        {statusType === "disabled" && <StyledCatalogSpamIcon size="small" />}
      </>
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
