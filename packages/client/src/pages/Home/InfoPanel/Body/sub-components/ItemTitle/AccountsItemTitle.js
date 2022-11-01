import React, { useRef } from "react";
import { withTranslation } from "react-i18next";

import Text from "@docspace/components/text";

import { Avatar } from "@docspace/components";
import Badges from "@docspace/client/src/pages/AccountsHome/Section/Body/Badges";
import { StyledAccountsItemTitle } from "../../styles/accounts";
import { StyledTitle } from "../../styles/common";
import ItemContextOptions from "./ItemContextOptions";

const AccountsItemTitle = ({
  t,
  isSeveralItems,
  selection,
  getUserContextOptions,
  selectionLength,
}) => {
  const itemTitleRef = useRef();

  const isPending =
    selection.statusType === "pending" || selection.statusType === "disabled";

  if (isSeveralItems) {
    return (
      <StyledTitle>
        <Avatar size={"min"} role={"user"} />
        <Text className="text" fontWeight={600} fontSize="16px">
          {`${t("InfoPanel:SelectedUsers")}: ${selectionLength}`}
        </Text>
      </StyledTitle>
    );
  }

  return (
    <StyledAccountsItemTitle isPending={isPending} ref={itemTitleRef}>
      <Avatar
        className="avatar"
        role={selection.role ? selection.role : "user"}
        size={"big"}
        source={selection.avatar}
      />
      <div className="info-panel__info-text">
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
        {!isPending && (
          <Text className={"info-text__email"} noSelect title={selection.email}>
            {selection.email}
          </Text>
        )}
        {isPending && (
          <Badges withoutPaid={true} statusType={selection.statusType} />
        )}
      </div>
      <ItemContextOptions
        t={t}
        isUser
        itemTitleRef={itemTitleRef}
        selection={selection}
      />
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
