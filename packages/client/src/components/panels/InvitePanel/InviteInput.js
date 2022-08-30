import React, { useState, useCallback, useEffect } from "react";
import debounce from "lodash.debounce";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";

import { parseAddresses } from "@docspace/components/utils/email";
import { ShareAccessRights } from "@docspace/common/constants";

import {
  StyledInviteInput,
  StyledInviteInputContainer,
  StyledDropDown,
  StyledDropDownItem,
  SearchItemText,
} from "./StyledInvitePanel";

const InviteInput = ({
  invitePanelOptions,
  getShareUsers,
  setInviteItems,
  inviteItems,
  getUsersByQuery,
  t,
}) => {
  const [inputValue, setInputValue] = useState("");
  const [usersList, setUsersList] = useState([]);
  const [panelVisible, setPanelVisible] = useState(false);
  const [roomUsers, setRoomUsers] = useState([]);

  useEffect(() => {
    const { id } = invitePanelOptions;
    getShareUsers([id]).then((accesses) => {
      const users = accesses.map((user) => {
        return {
          id: user.sharedTo.id,
          email: user.sharedTo.email,
          access: user.access,
        };
      });
      setRoomUsers(users);
    });
  }, [invitePanelOptions]);

  const toUserItem = (email) => {
    const emails = parseAddresses(email);

    const uid = () =>
      String(Date.now().toString(32) + Math.random().toString(16)).replace(
        /\./g,
        ""
      );
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

  const searchByTerm = async (value) => {
    value = value.trim();

    if (value.length > 0) {
      const users = await getUsersByQuery(value);
      setUsersList(users);
    }
  };

  const debouncedSearch = useCallback(
    debounce((value) => searchByTerm(value), 500),
    []
  );

  const onChange = (e) => {
    const value = e.target.value;

    setPanelVisible(!!value.length);

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
      <StyledDropDownItem key={id} onClick={addUser} disabled={invited}>
        <Avatar size="min" role="user" source={avatarSmall} />
        <div>
          <SearchItemText primary disabled={invited}>
            {displayName}
          </SearchItemText>
          <SearchItemText>{email}</SearchItemText>
        </div>
        {invited && <SearchItemText info>{t("Invited")}</SearchItemText>}
      </StyledDropDownItem>
    );
  };

  const addItem = () => {
    const item = toUserItem(inputValue);

    item.access = ShareAccessRights.ReadOnly; //TODO: get from main selector;

    setInviteItems([...inviteItems, item]);
    setPanelVisible(false);
  };

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
      >
        {!!usersList.length ? (
          usersList.map((user) => getItemContent(user))
        ) : (
          <StyledDropDownItem onClick={addItem}>
            {t("Add")} «{inputValue}»
          </StyledDropDownItem>
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
