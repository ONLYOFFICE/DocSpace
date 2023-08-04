import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Link from "@docspace/components/link";
import LinkWithDropdown from "@docspace/components/link-with-dropdown";
import Avatar from "@docspace/components/avatar";

export default function withContent(WrappedContent) {
  const WithContent = (props) => {
    const {
      item,
      selectGroup,
      checked,
      selectUser,
      deselectUser,
      setBufferSelection,

      theme,
      getModel,
      itemIndex,
    } = props;

    const { mobilePhone, email, role, displayName, avatar } = item;

    const onContentRowSelect = (checked, user) => {
      checked ? selectUser(user) : deselectUser(user);
    };

    const onContentRowClick = (checked, user, addToSelection = true) => {
      checked
        ? setBufferSelection(user, addToSelection)
        : setBufferSelection(null);
    };

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
      "RoomSelector",
    ]);

    const contextOptionsProps = {
      contextOptions: getModel(item, t),
    };

    let value = `folder_${item.id}`;
    value += "_false";
    value += `_index_${itemIndex}`;

    return (
      <WrappedContent
        onContentRowSelect={onContentRowSelect}
        onContentRowClick={onContentRowClick}
        onPhoneClick={onPhoneClick}
        onEmailClick={onEmailClick}
        groups={groups}
        checkedProps={checkedProps}
        element={element}
        contextOptionsProps={contextOptionsProps}
        value={value}
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
