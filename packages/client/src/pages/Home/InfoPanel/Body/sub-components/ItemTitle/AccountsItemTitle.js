import React, { useRef } from "react";
import { withTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import DefaultUserPhoto from "PUBLIC_DIR/images/default_user_photo_size_82-82.png";
import { Avatar, ContextMenuButton } from "@docspace/components";
import Badge from "@docspace/components/badge";
import Badges from "@docspace/client/src/pages/Home/Section/AccountsBody/Badges";
import { StyledAccountsItemTitle } from "../../styles/accounts";
import { StyledTitle } from "../../styles/common";

import { SSO_LABEL } from "SRC_DIR/helpers/constants";

const AccountsItemTitle = ({
  t,
  isSeveralItems,
  selection,
  getUserContextOptions,
}) => {
  if (isSeveralItems) {
    return <></>;
  }

  const itemTitleRef = useRef();

  const isPending =
    selection.statusType === "pending" || selection.statusType === "disabled";

  const getData = () => {
    const newOptions = selection.options?.filter(
      (option) => option !== "details"
    );
    return getUserContextOptions(t, newOptions || [], selection);
  };
  const contextOptions = getData();

  const userAvatar = selection.hasAvatar ? selection.avatar : DefaultUserPhoto;
  const isSSO = selection.isSSO || false;

  return (
    <StyledAccountsItemTitle
      isPending={isPending}
      isSSO={isSSO}
      ref={itemTitleRef}
    >
      <Avatar
        className="avatar"
        role={selection.role ? selection.role : "user"}
        size={"big"}
        source={userAvatar}
      />
      <div className="info-panel__info-text">
        <div className="info-panel__info-wrapper">
          <Text
            className={"info-text__name"}
            noSelect
            title={selection.displayName}
            truncate
          >
            {isPending
              ? selection.email
              : selection.displayName?.trim()
              ? selection.displayName
              : selection.email}
          </Text>
          {isPending && (
            <Badges withoutPaid={true} statusType={selection.statusType} />
          )}
        </div>
        {!isPending && (
          <Text className={"info-text__email"} title={selection.email}>
            {selection.email}
          </Text>
        )}
        {isSSO && (
          <Badge
            className="sso-badge"
            label={SSO_LABEL}
            color={"#FFFFFF"}
            backgroundColor="#22C386"
            fontSize={"9px"}
            fontWeight={800}
            noHover
            lineHeight={"13px"}
          />
        )}
      </div>
      {!!contextOptions.length && (
        <ContextMenuButton
          id="info-accounts-options"
          className="context-button"
          getData={getData}
        />
      )}
    </StyledAccountsItemTitle>
  );
};

export default withTranslation([
  "People",
  "PeopleTranslations",
  "InfoPanel",
  "Common",
  "Translations",
  "DeleteProfileEverDialog",
])(AccountsItemTitle);
