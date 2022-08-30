import React, { useState, useCallback } from "react";
import debounce from "lodash.debounce";

import Avatar from "@docspace/components/avatar";

import { parseAddresses } from "@docspace/components/utils/email";

import {
  StyledInviteInput,
  StyledInviteInputContainer,
  StyledDropDown,
  StyledDropDownItem,
  SearchItemText,
} from "./StyledInvitePanel";

const InviteInput = ({ onAddUser, getUsersByQuery }) => {
  const [inputValue, setInputValue] = useState("");
  const [usersList, setUsersList] = useState([]);
  const [panelVisible, setPanelVisible] = useState(false);

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

    setPanelVisible(value.length > 0);

    setInputValue(value);

    debouncedSearch(value);
  };

  const getItemContent = (item) => {
    const { avatar, displayName, email, id } = item;

    const addUser = () => {
      onAddUser && onAddUser(item);
      setPanelVisible(false);
    };

    return (
      <StyledDropDownItem key={id} onClick={addUser}>
        <Avatar size="min" role="user" source={avatar} />
        <div>
          <SearchItemText primary>{displayName}</SearchItemText>
          <SearchItemText>{email}</SearchItemText>
        </div>
        <SearchItemText info>Invited</SearchItemText>
      </StyledDropDownItem>
    );
  };

  const addItem = () => {
    const item = toUserItem(inputValue);

    onAddUser && onAddUser(item);

    setPanelVisible(false);
  };

  return (
    <StyledInviteInputContainer>
      <StyledInviteInput
        onChange={onChange}
        placeholder="Invite people by name or email"
        value={inputValue}
      />
      <StyledDropDown isDefaultMode={false} open={panelVisible} manualX="16px">
        {usersList.length ? (
          usersList?.map((user) => getItemContent(user))
        ) : (
          <StyledDropDownItem onClick={addItem}>
            Add «{inputValue}»
          </StyledDropDownItem>
        )}
      </StyledDropDown>
    </StyledInviteInputContainer>
  );
};

export default InviteInput;
