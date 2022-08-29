import React, { useState, useCallback, useEffect, useRef } from "react";

import debounce from "lodash.debounce";

import Avatar from "@docspace/components/avatar";

import {
  StyledInviteInput,
  StyledInviteInputContainer,
  StyledDropDown,
  StyledDropDownItem,
  SearchItemText,
} from "./StyledInvitePanel";

const InviteInput = ({ onAddUser, getUsersList }) => {
  const [inputValue, setInputValue] = useState("");
  const [usersList, setUsersList] = useState([]);
  const [panelVisible, setPanelVisible] = useState(false);

  const toUserItem = (email) => {
    const uid = () =>
      String(Date.now().toString(32) + Math.random().toString(16)).replace(
        /\./g,
        ""
      );
    return {
      email,
      id: uid(),
      displayName: email,
    };
  };

  const searchByTerm = async (value) => {
    const users = await getUsersList();
    setUsersList(users);

    //const user = toUserItem(value);
    //onAddUser && onAddUser(user);
  };

  const debouncedSearch = useCallback(
    debounce((value) => searchByTerm(value), 1000),
    []
  );

  const onChange = (e) => {
    const value = e.target.value;

    setPanelVisible(value.length > 0);

    setInputValue(value);

    debouncedSearch(value);
  };

  const getItemContent = (item) => {
    const { avatarSmall, displayName, email, id } = item;

    return (
      <StyledDropDownItem key={id}>
        <Avatar size="min" role="user" source={avatarSmall} />
        <div>
          <SearchItemText primary>{displayName}</SearchItemText>
          <SearchItemText>{email}</SearchItemText>
        </div>
        <SearchItemText info>Invited</SearchItemText>
      </StyledDropDownItem>
    );
  };

  return (
    <StyledInviteInputContainer>
      <StyledInviteInput
        onChange={onChange}
        placeholder="Invite people by name or email"
        value={inputValue}
      />
      <StyledDropDown isDefaultMode={false} open={panelVisible} manualX="16px">
        {usersList?.map((user) => getItemContent(user))}
      </StyledDropDown>
    </StyledInviteInputContainer>
  );
};

export default InviteInput;
