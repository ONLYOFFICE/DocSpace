import React, { useState, useCallback, useEffect } from "react";
import debounce from "lodash.debounce";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";
import DropDownItem from "@docspace/components/drop-down-item";

import { parseAddresses } from "@docspace/components/utils/email";
import { ShareAccessRights } from "@docspace/common/constants";

import {
  StyledInviteInput,
  StyledInviteInputContainer,
  StyledDropDown,
  SearchItemText,
} from "./StyledInvitePanel";

const InviteInput = ({
  invitePanelOptions,
  getShareUsers,
  setInviteItems,
  inviteItems,
  getUsersByQuery,
  t,
  roomUsers,
}) => {
  const [inputValue, setInputValue] = useState("");
  const [usersList, setUsersList] = useState([]);
  const [panelVisible, setPanelVisible] = useState(false);

  const toUserItem = (email) => {
    const emails = parseAddresses(email);
    const uid = () => Math.random().toString(36).slice(-6);

    return {
      email,
      id: uid(),
      displayName: email,
      errors: emails[0].parseErrors,
    };
  };

  const inRoom = (id) => {
    return roomUsers.some((user) => user.id === id);
  };

  const searchByQuery = async (value) => {
    const query = value.trim();

    if (query.length > 0) {
      const users = await getUsersByQuery(query);
      setUsersList(users);
    }
  };

  const debouncedSearch = useCallback(
    debounce((value) => searchByQuery(value), 300),
    []
  );

  const onChange = (e) => {
    const value = e.target.value;

    setPanelVisible(!!usersList.length || value.trim().length > 1);
    setInputValue(value);
    debouncedSearch(value);
  };

  const getItemContent = (item) => {
    const { avatarSmall, displayName, email, id } = item;

    const invited = inRoom(id);

    item.access = ShareAccessRights.ReadOnly; //TODO: get from main selector;

    const addUser = () => {
      setInviteItems([...inviteItems, item]);
      setPanelVisible(false);
    };

    return (
      <DropDownItem
        key={id}
        onClick={addUser}
        disabled={invited}
        height={48}
        className="list-item"
      >
        <Avatar size="min" role="user" source={avatarSmall} />
        <div>
          <SearchItemText primary disabled={invited}>
            {displayName}
          </SearchItemText>
          <SearchItemText>{email}</SearchItemText>
        </div>
        {invited && <SearchItemText info>{t("Invited")}</SearchItemText>}
      </DropDownItem>
    );
  };

  const addItem = () => {
    const item = toUserItem(inputValue);

    item.access = ShareAccessRights.ReadOnly; //TODO: get from main selector;

    setInviteItems([...inviteItems, item]);
    setPanelVisible(false);
  };

  const dropDownMaxHeight = usersList.length > 5 ? { maxHeight: 240 } : {};

  return (
    <StyledInviteInputContainer>
      <StyledInviteInput
        onChange={onChange}
        placeholder={t("SearchPlaceholder")}
        value={inputValue}
        onFocus={() => setPanelVisible(true)}
      />
      <StyledDropDown
        isDefaultMode={false}
        open={panelVisible}
        manualX="16px"
        showDisabledItems
        clickOutsideAction={() => setPanelVisible(false)}
        {...dropDownMaxHeight}
      >
        {!!usersList.length
          ? usersList.map((user) => getItemContent(user))
          : inputValue.length > 2 && (
              <DropDownItem onClick={addItem} height={48}>
                {t("Add")} «{inputValue}»
              </DropDownItem>
            )}
      </StyledDropDown>
    </StyledInviteInputContainer>
  );
};

export default inject(({ auth, peopleStore, filesStore, dialogsStore }) => {
  const { theme } = auth.settingsStore;
  const { getUsersByQuery } = peopleStore.usersStore;
  const { getShareUsers } = filesStore;
  const { invitePanelOptions, setInviteItems, inviteItems } = dialogsStore;

  return {
    getShareUsers,
    setInviteItems,
    inviteItems,
    getUsersByQuery,
    invitePanelOptions,
  };
})(observer(InviteInput));
