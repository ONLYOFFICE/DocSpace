import React, { useState, useCallback, useEffect, useRef } from "react";
import debounce from "lodash.debounce";
import { inject, observer } from "mobx-react";

import Avatar from "@docspace/components/avatar";
import TextInput from "@docspace/components/text-input";
import DropDownItem from "@docspace/components/drop-down-item";
import toastr from "@docspace/components/toast/toastr";

import { parseAddresses } from "@docspace/components/utils/email";

import { AddUsersPanel } from "../../index";
import { getAccessOptions } from "../utils";

import AccessSelector from "./AccessSelector";

import {
  StyledSubHeader,
  StyledLink,
  StyledInviteInput,
  StyledInviteInputContainer,
  StyledDropDown,
  SearchItemText,
} from "../StyledInvitePanel";

const InviteInput = ({
  defaultAccess,
  getUsersByQuery,
  hideSelector,
  inviteItems,
  onClose,
  roomId,
  roomType,
  setInviteItems,
  roomUsers,
  t,
}) => {
  const [inputValue, setInputValue] = useState("");
  const [usersList, setUsersList] = useState([]);
  const [searchPanelVisible, setSearchPanelVisible] = useState(false);
  const [addUsersPanelVisible, setAddUsersPanelVisible] = useState(false);

  const [selectedAccess, setSelectedAccess] = useState(defaultAccess);

  const inputsRef = useRef();
  const searchRef = useRef();

  const inRoom = (id) => {
    return roomUsers.some((user) => user.sharedTo.id === id);
  };

  const toUserItems = (query) => {
    const addresses = parseAddresses(query);
    const uid = () => Math.random().toString(36).slice(-6);

    if (addresses.length > 1) {
      return addresses.map((address) => {
        return {
          email: address.email,
          id: uid(),
          access: selectedAccess,
          displayName: address.email,
          errors: address.parseErrors,
        };
      });
    }

    return {
      email: addresses[0].email,
      id: uid(),
      access: selectedAccess,
      displayName: addresses[0].email,
      errors: addresses[0].parseErrors,
    };
  };

  const searchByQuery = async (value) => {
    const query = value.trim();

    if (!!query.length) {
      const users = await getUsersByQuery(query);
      setUsersList(users);
    } else {
      closeInviteInputPanel();
    }
  };

  const debouncedSearch = useCallback(
    debounce((value) => searchByQuery(value), 300),
    []
  );

  const onChange = (e) => {
    const value = e.target.value;
    const clearValue = value.trim();

    if ((!!usersList.length || clearValue.length > 1) && !searchPanelVisible) {
      openInviteInputPanel();
    }

    setInputValue(value);
    debouncedSearch(clearValue);
  };

  const removeExist = (items) => {
    const filtered = items.reduce((unique, o) => {
      !unique.some((obj) => obj.email === o.email) && unique.push(o);
      return unique;
    }, []);

    if (items.length > filtered.length)
      toastr.warning("Some users have already been added");

    return filtered;
  };

  const getItemContent = (item) => {
    const { avatar, displayName, email, id } = item;

    const invited = inRoom(id);

    item.access = selectedAccess;

    const addUser = () => {
      const items = removeExist([item, ...inviteItems]);
      setInviteItems(items);
      closeInviteInputPanel();
    };

    return (
      <DropDownItem
        key={id}
        onClick={addUser}
        disabled={invited}
        height={48}
        className="list-item"
      >
        <Avatar size="min" role="user" source={avatar} />
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

  const addEmail = () => {
    const items = toUserItems(inputValue);

    const newItems =
      items.length > 1 ? [...items, ...inviteItems] : [items, ...inviteItems];

    const filtered = removeExist(newItems);

    setInviteItems(filtered);
    closeInviteInputPanel();
  };

  const addItems = (users) => {
    const items = [...users, ...inviteItems];

    const filtered = removeExist(items);

    setInviteItems(filtered);
    closeInviteInputPanel();
  };

  const dropDownMaxHeight = usersList.length > 5 ? { maxHeight: 240 } : {};

  const openUsersPanel = () => {
    setInputValue("");
    setAddUsersPanelVisible(true);
  };

  const closeUsersPanel = () => {
    setAddUsersPanelVisible(false);
  };

  const openInviteInputPanel = (e) => {
    setSearchPanelVisible(true);
  };

  const closeInviteInputPanel = (e) => {
    if (e?.target.tagName.toUpperCase() == "INPUT") return;

    setInputValue("");
    setUsersList([]);

    setSearchPanelVisible(false);
  };

  const foundUsers = usersList.map((user) => getItemContent(user));

  const accessOptions = getAccessOptions(t, roomType);

  const onSelectAccess = (item) => {
    setSelectedAccess(item.access);
  };

  const onKeyPress = (e) => {
    if (e.key === "Enter" && !!!usersList.length && inputValue.length > 2) {
      addEmail();
    }
  };

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);
    return () => document.removeEventListener("keyup", onKeyPress);
  });

  return (
    <>
      <StyledSubHeader>
        {t("IndividualInvitation")}
        <StyledLink
          fontWeight="600"
          type="action"
          isHovered
          onClick={openUsersPanel}
        >
          {t("СhooseFromList")}
        </StyledLink>
      </StyledSubHeader>

      <StyledInviteInputContainer ref={inputsRef}>
        <StyledInviteInput ref={searchRef}>
          <TextInput
            scale
            onChange={onChange}
            placeholder={t("SearchPlaceholder")}
            value={inputValue}
            onFocus={openInviteInputPanel}
          />
        </StyledInviteInput>
        <StyledDropDown
          width={searchRef?.current?.offsetWidth}
          isDefaultMode={false}
          open={searchPanelVisible}
          manualX="16px"
          showDisabledItems
          clickOutsideAction={closeInviteInputPanel}
          {...dropDownMaxHeight}
        >
          {!!usersList.length
            ? foundUsers
            : inputValue.length > 2 && (
                <DropDownItem
                  style={{ width: "inherit" }}
                  textOverflow
                  onClick={addEmail}
                  height={48}
                >
                  {t("Add")} «{inputValue}»
                </DropDownItem>
              )}
        </StyledDropDown>

        {!hideSelector && (
          <AccessSelector
            t={t}
            roomType={roomType}
            defaultAccess={defaultAccess}
            onSelectAccess={onSelectAccess}
            containerRef={inputsRef}
          />
        )}

        {addUsersPanelVisible && (
          <AddUsersPanel
            onParentPanelClose={onClose}
            onClose={closeUsersPanel}
            visible={addUsersPanelVisible}
            shareDataItems={roomUsers}
            tempDataItems={inviteItems}
            setDataItems={addItems}
            accessOptions={accessOptions}
            isMultiSelect
            isEncrypted={true}
            defaultAccess={selectedAccess}
          />
        )}
      </StyledInviteInputContainer>
    </>
  );
};

export default inject(({ auth, peopleStore, filesStore, dialogsStore }) => {
  const { theme } = auth.settingsStore;
  const { getUsersByQuery } = peopleStore.usersStore;
  const { invitePanelOptions, setInviteItems, inviteItems } = dialogsStore;

  return {
    setInviteItems,
    inviteItems,
    getUsersByQuery,
    roomId: invitePanelOptions.roomId,
    hideSelector: invitePanelOptions.hideSelector,
    defaultAccess: invitePanelOptions.defaultAccess,
  };
})(observer(InviteInput));
