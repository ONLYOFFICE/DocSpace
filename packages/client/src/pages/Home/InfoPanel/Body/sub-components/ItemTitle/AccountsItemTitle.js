import React, { useRef } from "react";
import { withTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import DefaultUserPhoto from "PUBLIC_DIR/images/default_user_photo_size_82-82.png";
import { Avatar, ContextMenuButton } from "@docspace/components";
import Badges from "@docspace/client/src/pages/AccountsHome/Section/Body/Badges";
import { StyledAccountsItemTitle } from "../../styles/accounts";
import { StyledTitle } from "../../styles/common";

const AccountsItemTitle = ({
  t,
  isSeveralItems,
  selection,
  getUserContextOptions,
  selectionLength,
}) => {
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

  return (
    <StyledAccountsItemTitle isPending={isPending} ref={itemTitleRef}>
      <Avatar
        className="avatar"
        role={selection.role ? selection.role : "user"}
        size={"big"}
        source={userAvatar}
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
