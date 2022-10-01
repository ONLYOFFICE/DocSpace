import React from "react";
import { withTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import ContextMenuButton from "@docspace/components/context-menu-button";

import { Avatar } from "@docspace/components";
import Badges from "@docspace/client/src/pages/AccountsHome/Section/Body/Badges";
import { StyledAccountsItemTitle } from "../../styles/accounts";
import { StyledTitle } from "../../styles/common";

const AccountsItemTitle = ({
  t,
  isSeveralItems,
  selection,
  getUserContextOptions,
}) => {
  const isPending = selection.statusType === "pending";

  const getData = () => {
    const newOptions = selection.options.filter(
      (option) => option !== "details"
    );
    return getUserContextOptions(t, newOptions, selection);
  };

  if (isSeveralItems) {
    return (
      <StyledTitle>
        <Avatar size={"min"} role={"user"} />
        <Text className="text" fontWeight={600} fontSize="16px">
          {`${t("InfoPanel:SelectedUsers")}: ${selection.length}`}
        </Text>
      </StyledTitle>
    );
  }

  return (
    <StyledAccountsItemTitle isPending={isPending}>
      <Avatar
        className="avatar"
        role={selection.role}
        size={"big"}
        source={selection.avatar}
      />
      <div className="info-panel__info-text">
        <Text
          className={"info-text__name"}
          noSelect
          title={selection.displayName}
        >
          {isPending ? selection.email : selection.displayName}
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
      <ContextMenuButton className="context-button" getData={getData} />
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
