import React, { useCallback } from "react";
import { inject, observer } from "mobx-react";
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
      isAdmin,
      theme,
    } = props;
    const { userName, mobilePhone, email, role, displayName, avatar } = item;

    const onContentRowSelect = (checked, user) =>
      checked ? selectUser(user) : deselectUser(user);

    const checkedProps = isAdmin ? { checked } : {};

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

    const redirectToProfile = () => {
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/accounts/view/${userName}`
        )
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
              `/accounts/view/${userName}`
            ) !== window.location.pathname
          )
            redirectToProfile();
        });
      },
      [history, userName]
    );

    const onPhoneClick = () => window.open(`sms:${mobilePhone}`);
    const onEmailClick = () => window.open(`mailto:${email}`);

    return (
      <WrappedContent
        onContentRowSelect={onContentRowSelect}
        onPhoneClick={onPhoneClick}
        onEmailClick={onEmailClick}
        onUserNameClick={onUserNameClick}
        groups={groups}
        checkedProps={checkedProps}
        element={element}
        isAdmin={isAdmin}
        {...props}
      />
    );
  };

  return inject(({ auth, peopleStore }, { item }) => {
    const { isAdmin, userStore } = auth;

    const { selectGroup } = peopleStore.selectedGroupStore;
    const { getTargetUser } = peopleStore.targetUserStore;
    const { selectionStore } = peopleStore;

    const {
      selection,
      setSelection,
      selectUser,
      deselectUser,
    } = selectionStore;

    return {
      theme: auth.settingsStore.theme,
      isAdmin,
      currentUserId: userStore.user.id,
      selectGroup,
      fetchProfile: getTargetUser,
      checked: selection.some((el) => el.id === item.id),
      selectUser,
      deselectUser,
      setSelection,
    };
  })(observer(WithContent));
}
