import React, { useCallback } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Link from "@docspace/components/link";
import LinkWithDropdown from "@docspace/components/link-with-dropdown";
import Avatar from "@docspace/components/avatar";

import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";

export default function withContent(WrappedContent) {
  const WithContent = (props) => {
    const {
      item,
      selectGroup,
      fetchProfile,
      history,
      checked,
      selectUser,
      deselectUser,

      theme,
      getModel,
    } = props;
    const { userName, mobilePhone, email, role, displayName, avatar } = item;

    const onContentRowSelect = (checked, user) =>
      checked ? selectUser(user) : deselectUser(user);

    const checkedProps = { checked };

    const element = (
      <Avatar size="min" role={role} userName={displayName} source={avatar} />
    );

    const getFormattedGroups = () => {
      let temp = [];
      const groups = item.groups;
      const linkColor =
        item.statusType === "pending"
          ? theme.peopleWithContent.pendingColor
          : theme.peopleWithContent.color;

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
            isTextOverflow
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
            className="link-with-dropdown-group"
            isTextOverflow
            containerMinWidth="120px"
            containerWidth="15%"
            directionY="both"
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

    const groups = getFormattedGroups();

    /*const redirectToProfile = () => {
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/accounts/view/${userName}`
        )
      );
    };*/

    /*const onUserNameClick = useCallback(
      (e) => {
        const timer = setTimeout(() => redirectToProfile(), 500);
        e.preventDefault();
        fetchProfile(userName).finally(() => {
          clearTimeout(timer);
          if (
            combineUrl(
              AppServerConfig.proxyURL,
              config.homepage,
              `/accounts/view/${userName}`
            ) !== window.location.pathname
          )
            redirectToProfile();
        });
      },
      [history, userName]
    );*/

    const onPhoneClick = () => window.open(`sms:${mobilePhone}`);
    const onEmailClick = () => window.open(`mailto:${email}`);

    const { t } = useTranslation([
      "People",
      "Common",
      "PeopleTranslations",
      "DeleteProfileEverDialog",
      "Translations",
      "Files",
      "ChangeUserTypeDialog",
    ]);

    const contextOptionsProps = {
      contextOptions: getModel(item, t),
    };

    return (
      <WrappedContent
        onContentRowSelect={onContentRowSelect}
        onPhoneClick={onPhoneClick}
        onEmailClick={onEmailClick}
        //onUserNameClick={onUserNameClick}
        groups={groups}
        checkedProps={checkedProps}
        element={element}
        contextOptionsProps={contextOptionsProps}
        {...props}
      />
    );
  };

  return inject(({ auth, peopleStore }, { item }) => {
    const { userStore } = auth;

    const { selectGroup } = peopleStore.selectedGroupStore;
    const { getTargetUser } = peopleStore.targetUserStore;
    const { selectionStore, contextOptionsStore } = peopleStore;

    const { getModel } = contextOptionsStore;

    const {
      selection,
      bufferSelection,
      setBufferSelection,
      selectUser,
      deselectUser,
    } = selectionStore;

    return {
      theme: auth.settingsStore.theme,

      currentUserId: userStore.user.id,
      selectGroup,
      fetchProfile: getTargetUser,
      checked: selection.some((el) => el.id === item.id),
      isSeveralSelection: selection.length > 1,
      isActive: bufferSelection?.id === item?.id,
      setBufferSelection,
      selectUser,
      deselectUser,
      getModel,
    };
  })(observer(WithContent));
}
